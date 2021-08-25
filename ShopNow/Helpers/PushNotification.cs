using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;



namespace ShopNow.Helpers
{
    public  class PushNotification
    {


        public static string SendbydeviceId(string body, string title,string sound, string deviceId = "")
        {

            //var channel = new NotificationChannel(CHANNEL_ID,
            //                             "FCM Notifications",
            //                             NotificationImportance.Default)
            //{

            //    Description = "Firebase Cloud Messages appear in this channel"
            //};

            //var notificationManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
            //notificationManager.CreateNotificationChannel(channel);

            string res = "";
            try
            {

                var applicationID = "AAAASx4c4GY:APA91bEYyUEFT9F1XhO44epVtF0Mxq2SNbqIZUSQ3Xroov65JF9TzH7v9TghwG4JiWVa8HgqJVJnfklHIqhFuCQfW9T8b8TzrOOMYJd9eh2H1HcJFg06Vnjqz0aJk1tCSSuUL9BeUrsD";

                var senderId = "322627756134";

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";

                tRequest.ContentType = "application/json";

                var data = new

                {

                    to = deviceId,

                    notification = new

                    {

                        body = body,

                        title = title,
                        sound = sound
                    }
                };

                var serializer = new JavaScriptSerializer();

                var json = serializer.Serialize(data);

                Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));

                tRequest.ContentLength = byteArray.Length;


                using (Stream dataStream = tRequest.GetRequestStream())
                {

                    dataStream.Write(byteArray, 0, byteArray.Length);


                    using (WebResponse tResponse = tRequest.GetResponse())
                    {

                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {

                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {

                                String sResponseFromServer = tReader.ReadToEnd();

                                res = sResponseFromServer;

                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {

                res = ex.Message;

            }

            return res;

        }


        public static string SendBulk(string body, string title, string sound, string[] deviceId)
        {
            string res = "";
            try
            {
                var applicationID = "AAAASx4c4GY:APA91bEYyUEFT9F1XhO44epVtF0Mxq2SNbqIZUSQ3Xroov65JF9TzH7v9TghwG4JiWVa8HgqJVJnfklHIqhFuCQfW9T8b8TzrOOMYJd9eh2H1HcJFg06Vnjqz0aJk1tCSSuUL9BeUrsD";
                var senderId = "322627756134";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    registration_ids = deviceId,
                    notification = new
                    {
                        body = body,
                        title = title,
                        sound = sound
                    }
                };
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                res = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;

        }
    }
}