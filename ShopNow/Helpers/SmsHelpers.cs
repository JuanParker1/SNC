using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using ShopNow.Models;
using ShopNow.ViewModels;
using System.Net;


namespace ShopNow.Filters
{
    public class SendSMS
    {
        private static string SID = "joyra";
        private static string token = "9b214e6d834d00e010b53aa867d70a8df7820f744934a1a1";
        private static string key = "5f1c4f42b61292051ae1c05ae07cb9a31680b5d123628bbf";
        //   private static string token = "aa85596d3fa9eb24a98099e251898f961662d2d2";


        public SendSMS()
        {

        }

        //public SendSMS(string SID, string token)
        //{
        //    this.SID = SID;
        //    this.token = token;
        //}

        //public static string execute(string from, string to, string Body)
        //{
        //    Dictionary<string, string> postValues = new Dictionary<string, string>();
        //    postValues.Add("From", from);
        //    postValues.Add("To", to);
        //    postValues.Add("Body", Body);

        //    String postString = "";

        //    foreach (KeyValuePair<string, string> postValue in postValues)
        //    {
        //        postString += postValue.Key + "=" + HttpUtility.UrlEncode(postValue.Value) + "&";
        //    }
        //    postString = postString.TrimEnd('&');
        //    /*
        // * Allow self signed certificates and such
        // */
        //    ServicePointManager.ServerCertificateValidationCallback = delegate
        //    {
        //        return true;
        //    };
        //   // string smsURL = "https://twilix.exotel.in/v1/Accounts/joyra/Sms/send";
        //    string smsURL = "https://api.exotel.in/v1/Accounts/joyra/Sms/send";
        //    HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(smsURL);
        //   // objRequest.Credentials = new NetworkCredential(this.SID, this.token);
        //   objRequest.Credentials = new NetworkCredential(SID, token);
        //    objRequest.Method = "POST";
        //    objRequest.ContentLength = postString.Length;
        //    objRequest.ContentType = "application/x-www-form-urlencoded";
        //    // post data is sent as a stream                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
        //    StreamWriter opWriter = null;
        //    opWriter = new StreamWriter(objRequest.GetRequestStream());
        //    opWriter.Write(postString);
        //    opWriter.Close();

        //    // returned values are returned as a stream, then read into a string                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
        //    HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
        //    string postResponse = null;
        //    using (StreamReader responseStream = new StreamReader(objResponse.GetResponseStream()))
        //    {
        //        postResponse = responseStream.ReadToEnd();
        //        responseStream.Close();
        //    }
        //    return (postResponse);
        //}
        public static string execute(string from, string to, string Body)
        {
            Dictionary<string, string> postValues = new Dictionary<string, string>();
            postValues.Add("From", from);
            postValues.Add("To", to);
            postValues.Add("Body", Body);

            String postString = "";

            foreach (KeyValuePair<string, string> postValue in postValues)
            {
                postString += postValue.Key + "=" + WebUtility.UrlEncode(postValue.Value) + "&";
            }
            postString = postString.TrimEnd('&');

            ServicePointManager.ServerCertificateValidationCallback = delegate {
                return true;
            };
            string smsURL = "http://api.exotel.in/v1/Accounts/"+SID+"/Sms/send";
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(smsURL);
            objRequest.Credentials = new NetworkCredential(key, token);
            objRequest.Method = "POST";
            objRequest.ContentLength = postString.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";

            StreamWriter opWriter = null;
            opWriter = new StreamWriter(objRequest.GetRequestStream());
            opWriter.Write(postString);
            opWriter.Close();


            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            string postResponse = null;
            using (StreamReader responseStream = new StreamReader(objResponse.GetResponseStream()))
            {
                postResponse = responseStream.ReadToEnd();
                responseStream.Close();
            }

            return (postResponse);
        }

        public static void Main(string[] args)
        {
            //SendSMS s = new SendSMS ("YourExotelSID", "YourExotelToken");
            //string response = s.execute ("Your Exotel VN", "Customer's Phone no", "Message to send");
            //Console.WriteLine (response);
        }
    }

}