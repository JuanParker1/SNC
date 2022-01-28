using Amazon.S3;
using Amazon.S3.Model;
using ShopNow.Models;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Helpers;

namespace ShopNow.Helpers
{
    public class UploadContent
    {
        public void UploadFiles(Stream imgstream, string name, string key, string secretkey, string type)
        {
            var bucketRegion = Amazon.RegionEndpoint.APSouth1;   // Your bucket region
            var s3 = new AmazonS3Client(key, secretkey, bucketRegion);
            var putRequest = new PutObjectRequest();

            putRequest.InputStream = imgstream;
            // key will be the name of the image in your bucket
            putRequest.Key = name.Trim();
            if (type == "image")
            {
                putRequest.ContentType = "image/jpeg";
                using (System.Drawing.Image img = System.Drawing.Image.FromStream(imgstream))
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("height");
                    dt.Columns.Add("width");
                    dt.Columns.Add("type");
                    dt.Rows.Add(113, 113, "Small");
                    dt.Rows.Add(670, 670, "Medium");
                    dt.Rows.Add(1500, 1500, "Large");
                    foreach (DataRow dr in dt.Rows)
                    {
                        int originalWidth = img.Width;
                        int originalHeight = img.Height;
                        int sourceWidth = img.Width;
                        //Get the image current height  
                        int sourceHeight = img.Height;
                        float nPercent = 0;
                        float nPercentW = 0;
                        float nPercentH = 0;
                        //Calulate  width with new desired size  
                        nPercentW = ((float)Convert.ToInt32(dr[1]) / (float)sourceWidth);
                        //Calculate height with new desired size  
                        nPercentH = ((float)Convert.ToInt32(dr[0]) / (float)sourceHeight);

                        if (nPercentH < nPercentW)
                            nPercent = nPercentH;
                        else
                            nPercent = nPercentW;
                        //New Width  
                        int destWidth = (int)(sourceWidth * nPercent);
                        //New Height  
                        int destHeight = (int)(sourceHeight * nPercent);

                        using (Bitmap b = new Bitmap(img, destWidth, destHeight))
                        {
                            b.SetResolution(96, 96);
                            using (MemoryStream ms2 = new MemoryStream())
                            {
                                b.Save(ms2, System.Drawing.Imaging.ImageFormat.Png);
                                //b.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg);
                                putRequest.InputStream = ms2;
                                if (Convert.ToString(dr[2]) == "Small")
                                    putRequest.BucketName = "shopnowchat.com/Small";
                                else if (Convert.ToString(dr[2]) == "Medium")
                                    putRequest.BucketName = "shopnowchat.com/Medium";
                                else
                                    putRequest.BucketName = "shopnowchat.com/Large";

                                 PutObjectResponse putResponse = s3.PutObject(putRequest);
                            }
                        }
                    }


                }
            }
            else if (type == "pdf")
            {
                putRequest.BucketName = "shopnowchat.com/Uploads";
                putRequest.ContentType = "application/pdf";
                PutObjectResponse putResponse = s3.PutObject(putRequest);
            }
            else
            {
                putRequest.BucketName = "shopnowchat.com/Uploads";
                putRequest.ContentType = "text/plain";
                PutObjectResponse putResponse = s3.PutObject(putRequest);
            }
            //PutObjectResponse putResponse = s3.PutObject(putRequest);


        }

        public void DeleteFiles(string filename, string key, string secretkey)
        {
            var bucketRegion = Amazon.RegionEndpoint.APSouth1;   // Your bucket region
            var s3 = new AmazonS3Client(key, secretkey, bucketRegion);
            for (int i = 0; i <= 2; i++)
            {
                string bucketName = "";
                if (i == 0)
                    bucketName = "shopnowchat.com/Small";
                else if(i==1)
                    bucketName = "shopnowchat.com/Medium";
                else
                    bucketName = "shopnowchat.com/Large";
                var deleteObjectRequest = new DeleteObjectRequest { BucketName = bucketName, Key = filename };
                s3.DeleteObject(deleteObjectRequest);
            }
        }
        public void UploadMediumFile(Stream imgstream, string name, string key, string secretkey, string type)
        {
            var bucketRegion = Amazon.RegionEndpoint.APSouth1;   // Your bucket region
            var s3 = new AmazonS3Client(key, secretkey, bucketRegion);
            var putRequest = new PutObjectRequest();

            putRequest.InputStream = imgstream;
            // key will be the name of the image in your bucket
            putRequest.Key = name.Trim();
            if (type == "image")
            {
                putRequest.ContentType = "image/jpeg";
                putRequest.BucketName = "shopnowchat.com/Medium";
                PutObjectResponse putResponse = s3.PutObject(putRequest);
            }
            else if (type == "pdf")
            {
                putRequest.BucketName = "shopnowchat.com/Uploads";
                putRequest.ContentType = "application/pdf";
                PutObjectResponse putResponse = s3.PutObject(putRequest);
            }
            else
            {
                putRequest.BucketName = "shopnowchat.com/Uploads";
                putRequest.ContentType = "text/plain";
                PutObjectResponse putResponse = s3.PutObject(putRequest);
            }
        }

        /// Uploading and resizing an image, Currently it is used to upload member profile pic, provider service banner image and category image

        public void UploadImage(HttpPostedFileBase originalImage, string imagePrefix, string rootPath, HttpServerUtilityBase server, sncEntities _db, string memberId, string productId = "", string categoryId = "")
        {
            bool existsOriginal = System.IO.Directory.Exists(server.MapPath("~/" + rootPath + "Original/"));
            if (!existsOriginal)
                System.IO.Directory.CreateDirectory(server.MapPath("~/" + rootPath + "Original/"));
            originalImage.SaveAs(server.MapPath("~/" + rootPath + "Original/" + imagePrefix + originalImage.FileName));

           // Large size for service banner image
            if (productId != "")
                {
                    WebImage img1 = new WebImage(server.MapPath("~/" + rootPath + "Original/" + imagePrefix + originalImage.FileName));
                    img1.Resize(1500, 1500);
                    bool exists = System.IO.Directory.Exists(server.MapPath("~/" + rootPath + "Large/"));
                    if (!exists)
                        System.IO.Directory.CreateDirectory(server.MapPath("~/" + rootPath + "Large/"));
                    img1.Save(Path.Combine(server.MapPath("~/" + rootPath + "Large/" + imagePrefix + originalImage.FileName)));
                }

            WebImage img2 = new WebImage(server.MapPath("~/" + rootPath + "Original/" + imagePrefix + originalImage.FileName));
            img2.Resize(670, 670);
            bool exists2 = System.IO.Directory.Exists(server.MapPath("~/" + rootPath + "Medium/"));
            if (!exists2)
                System.IO.Directory.CreateDirectory(server.MapPath("~/" + rootPath + "Medium/"));
            img2.Save(Path.Combine(server.MapPath("~/" + rootPath + "Medium/" + imagePrefix + originalImage.FileName)));

            WebImage img3 = new WebImage(server.MapPath("~/" + rootPath + "Original/" + imagePrefix + originalImage.FileName));
            img3.Resize(113, 113);
            bool exists3 = System.IO.Directory.Exists(server.MapPath("~/" + rootPath + "Small/"));
            if (!exists3)
                System.IO.Directory.CreateDirectory(server.MapPath("~/" + rootPath + "Small/"));
            img3.Save(Path.Combine(server.MapPath("~/" + rootPath + "Small/" + imagePrefix + originalImage.FileName)));
            //System.IO.File.Delete(server.MapPath("~/" + rootPath + "Small/" + imagePrefix + originalImage.FileName));
            //System.IO.File.Delete(server.MapPath("~/" + rootPath + "Large/" + imagePrefix + originalImage.FileName));
            //System.IO.File.Delete(server.MapPath("~/" + rootPath + "Original/" + imagePrefix + originalImage.FileName));
        }

        public void UploadStaticPageImage(HttpPostedFileBase originalImage, string rootPath, string imagePrefix, HttpServerUtilityBase server)
        {
            originalImage.SaveAs(server.MapPath("~/" + rootPath + imagePrefix + originalImage.FileName));
        }

        /// Save an image at specified path 

        public string UploadImage1(HttpPostedFileBase image, string imageSubPath, HttpServerUtilityBase server, sncEntities _db, int memberId = 0)
        {
            string ImgPath = imageSubPath + memberId + "_" + image.FileName;
            var imgnew = server.MapPath(ImgPath);
            bool exists = System.IO.Directory.Exists(server.MapPath(imageSubPath));
            if (!exists)
                System.IO.Directory.CreateDirectory(server.MapPath(imageSubPath));

            ResizeAndUploadImageByHttpPostedFileBase(image, (imageSubPath + "/Large/"), new Size(750, 300), memberId + "_", image.FileName, server);
            ResizeAndUploadImageByHttpPostedFileBase(image, (imageSubPath + "/Medium/"), new Size(234, 156), memberId + "_", image.FileName, server);
            return ImgPath;
        }



        public static bool ResizeAndUploadImageByHttpPostedFileBase(HttpPostedFileBase originalImage, string targetPath, Size size, string imgPrefix, string outputImageName, HttpServerUtilityBase server)
        {
            bool success = false;
            try
            {
                bool exists = System.IO.Directory.Exists(server.MapPath(targetPath));
                if (!exists)
                    System.IO.Directory.CreateDirectory(server.MapPath(targetPath));

                WebImage img = new WebImage(originalImage.InputStream);
                if (img.Width > size.Width)
                    img.Resize(size.Width, size.Height);
                img.Save(Path.Combine(targetPath, imgPrefix + outputImageName));

                success = true;
            }
            catch (Exception)
            {
                // LogFileWrite(ex);
            }
            return success;
        }

        public static bool ResizeAndUploadImage(string originalImage, string targetPath, Size size, string outputImageName)
        {
            bool success = false;
            try
            {
                System.Drawing.Image img_Original = System.Drawing.Image.FromFile(originalImage);
                Image image = ResizeImage(img_Original, size);
                image.Save(Path.Combine(targetPath, outputImageName));
                image.Dispose();
                success = true;
            }
            catch (Exception)
            {
                // LogFileWrite(ex);
            }
            return success;
        }
        private static Image ResizeImage(Image imgPhoto, Size size)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            int Height = size.Height;
            int Width = size.Width;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.White);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);
            grPhoto.Dispose();
            return bmPhoto;
        }

        public static Image Base64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }


        public void UploadMemberProfilePic()
        {

        }
    }
}
