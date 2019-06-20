using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TestFileUpload.Models;
using TestFileUpload.Repository;
using TestFileUpload.Utility;

namespace TestFileUpload.Controllers
{
    public class ImageUploadController : Controller
    {
        InsertImageData obj = new InsertImageData();
        // GET: ImageUpload
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase image)
        {
            try
            {
                if (image == null)
                {
                    ModelState.AddModelError("", "Please upload image.");
                }

                bool extnAllowed = false;
                var allowedExtensions = CloudConfigurationManager.GetSetting("AllowedExtensions");
                var allowedFileSize = Convert.ToInt64(CloudConfigurationManager.GetSetting("AllowedFileSize"));
                var imgExtension = Path.GetExtension(image.FileName);

                var splitExtn = allowedExtensions.Split(',');
                foreach (var extn in splitExtn)
                {
                    if (extn.ToLower() == imgExtension.ToLower())
                    {
                        extnAllowed = true;
                    }
                }

                if (extnAllowed == false)
                {
                    ModelState.AddModelError("", "Only image files are allowed.");
                }

                var imageSize = image.ContentLength;
                if (imageSize > allowedFileSize)
                {
                    ModelState.AddModelError("", "File too large, Please upload file below 4 MB only.");
                }

                if (!ModelState.IsValid)
                {
                    return View("Index");
                }

                var imageUrl = await UploadImageAsync(image);

                var tags = await AzureComputerVisionApi.AnalyzeUrlAsync(imageUrl);

                var imgnamesplit = imageUrl.Split('/');
                var imageName = imgnamesplit[imgnamesplit.Length - 1];

                string imageTags = string.Empty;
                foreach (var tag in tags.Description.Tags)
                {
                    if (imageTags == "")
                    {
                        imageTags = tag;
                    }
                    else
                    {
                        imageTags += "," + tag;
                    }
                }

                ImageModel model = new ImageModel();
                model.Name = imageName;
                model.Path = imageUrl;
                model.ExternalKey = tags.RequestId;
                model.tags = imageTags;
                
                int imageId = obj.AddImage(model);

                TempData["imageId"] = imageId;
                return RedirectToAction("UploadedImage");
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }
        }

        private async Task<string> UploadImageAsync(HttpPostedFileBase imageToUpload)
        {
            string imageFullPath = null;
            if (imageToUpload == null || imageToUpload.ContentLength == 0)
            {
                return null;
            }
            try
            {
                CloudStorageAccount cloudStorageAccount = AzureConnection.GetConnectionString();
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("sampleimage");

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(
                        new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        }
                        );
                }

                string imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageToUpload.FileName);

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);
                cloudBlockBlob.Properties.ContentType = imageToUpload.ContentType;
                await cloudBlockBlob.UploadFromStreamAsync(imageToUpload.InputStream);

                imageFullPath = cloudBlockBlob.Uri.ToString();
            }
            catch (Exception ex)
            {

            }
            return imageFullPath;
        }
        
        public ActionResult UploadedImage()
        {
            ImageModel result = new ImageModel();
            if (TempData["imageId"] != null)
            {
                var imageId = Convert.ToInt32(TempData["imageId"]);
                result = obj.GetImageByImageId(imageId);
            }
            
            return View(result);
        }
    }
}