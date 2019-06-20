using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace TestFileUpload.Utility
{
    public static class AzureComputerVisionApi
    {
        public static async Task<ImageAnalysis> AnalyzeUrlAsync(string imageUrl)
        {
            const string subscriptionKey = "6441d5caf02749a4be3f241f2f998870";
            const string endPoint = "https://westus2.api.cognitive.microsoft.com/vision/v1.0/analyze";

            ImageAnalysis result = new ImageAnalysis();
            var errors = new List<string>();
            try
            {
                HttpClient client = new HttpClient(); 
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);   
                string requestParameters = "visualFeatures=Categories,Description,Color";    
                string uri = endPoint + "?" + requestParameters;
                HttpResponseMessage response = new HttpResponseMessage();   
                byte[] byteData = GetImageAsByteArray(imageUrl);

                //using (ByteArrayContent content = new ByteArrayContent(byteData))
                //{   
                //    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                //    try
                //    {
                //        response = await client.PostAsync(uri, content);
                //    }
                //    catch (Exception ex)
                //    {
                //        ex.Message.ToString();
                //    }
                //}

                ByteArrayContent content = new ByteArrayContent(byteData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                try
                {
                    response = await client.PostAsync(uri, content).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

                var t = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    result = JsonConvert.DeserializeObject<ImageAnalysis>(t, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Include,
                        Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs earg) {
                            errors.Add(earg.ErrorContext.Member.ToString());
                            earg.ErrorContext.Handled = true;
                        }
                    });
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return result;
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            WebClient myWebClient = new WebClient();
            byte[] myDataBuffer = myWebClient.DownloadData(imageFilePath);
            return myDataBuffer;
        }
    }
}