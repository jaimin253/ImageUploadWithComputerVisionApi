using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TestFileUpload.Models;

namespace TestFileUpload.Repository
{
    public class InsertImageData
    {
        private SqlConnection con;
        //To Handle connection related activities
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            con = new SqlConnection(constr);
        }

        public int AddImage(ImageModel image)
        {
            bool isInserted = false;

            try
            {
                connection();
                SqlCommand com = new SqlCommand("sp_jd_insertImage", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Name", image.Name);
                com.Parameters.AddWithValue("@Path", image.Path);
                com.Parameters.AddWithValue("@ExternalKey", image.ExternalKey);
                com.Parameters.AddWithValue("@Tags", image.tags);

                //output imageid

                var p = new SqlParameter
                {
                    ParameterName = "ImageId",
                    DbType = System.Data.DbType.Int32,
                    Direction = System.Data.ParameterDirection.Output
                };

                com.Parameters.Add(p);

                con.Open();
                com.ExecuteNonQuery();
                int imageId = Convert.ToInt32(com.Parameters["ImageId"].Value);
                con.Close();
                
                return imageId;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return 0;
            }
        }

        public ImageModel GetImageByImageId(int imageId)
        {
            ImageModel outputModel = new ImageModel();
            try
            {
                connection();
                SqlCommand cmd = new SqlCommand("sp_jd_getImageByImageId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ImageId", imageId);

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    outputModel.ImageId = Convert.ToInt32(dt.Rows[0]["ImageId"]);
                    outputModel.Name = Convert.ToString(dt.Rows[0]["Name"]);
                    outputModel.Path = Convert.ToString(dt.Rows[0]["Path"]);
                    outputModel.ExternalKey = Convert.ToString(dt.Rows[0]["ExternalKey"]);
                    outputModel.tags = Convert.ToString(dt.Rows[0]["tags"]);
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return outputModel;
        }
    }
}