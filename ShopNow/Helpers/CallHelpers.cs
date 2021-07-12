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


    public class ConnectCall
    {
        //private static string SID = "joyra";
        private static string token = "9b214e6d834d00e010b53aa867d70a8df7820f744934a1a1";

        private static string key = "5f1c4f42b61292051ae1c05ae07cb9a31680b5d123628bbf";


        //public ConnectCall(string SID, string key, string token)
        //{
        //    this.SID = SID;
        //    this.key = key;
        //    this.token = token;
        //}

        public static string connectCustomerToAgent(string from, string to, string callerID = "04448134440",
                                              string callType = "trans", string timeLimit = null,
                                              string timeOut = null, string statusCallback = null)
        {
            Dictionary<string, string> postValues = new Dictionary<string, string>();
            postValues.Add("From", from);
            postValues.Add("To", to);
            postValues.Add("CallerID", callerID);
            postValues.Add("CallType", callType);
            if (timeLimit != null)
            {
                postValues.Add("TimeLimit", timeLimit);
            }
            if (timeOut != null)
            {
                postValues.Add("TimeOut", timeOut);
            }

            if (statusCallback != null)
            {
                postValues.Add("StatusCallback", statusCallback);
            }

            String postString = "";

            foreach (KeyValuePair<string, string> postValue in postValues)
            {
                postString += postValue.Key + "=" + HttpUtility.UrlEncode(postValue.Value) + "&";
            }
            postString = postString.TrimEnd('&');

            return (sendRequest(postString));

        }

        public string connectCustomerToApp(string from, string url, string callerID,
                                             string callType, string timeLimit = null,
                                             string timeOut = null, string statusCallback = null,
                                             string customfield = null)
        {
            Dictionary<string, string> postValues = new Dictionary<string, string>();
            postValues.Add("From", from);
            postValues.Add("Url", url);
            postValues.Add("CallerID", callerID);
            postValues.Add("CallType", callType);
            if (timeLimit != null)
            {
                postValues.Add("TimeLimit", timeLimit);
            }
            if (timeOut != null)
            {
                postValues.Add("TimeOut", timeOut);
            }

            if (statusCallback != null)
            {
                postValues.Add("StatusCallback", statusCallback);
            }
            if (customfield != null)
            {
                postValues.Add("CustomField", customfield);
            }

            String postString = "";

            foreach (KeyValuePair<string, string> postValue in postValues)
            {
                postString += postValue.Key + "=" + HttpUtility.UrlEncode(postValue.Value) + "&";
            }
            postString = postString.TrimEnd('&');

            return (sendRequest(postString));
        }

        private static string sendRequest(string postString)
        {
            try
            {
                /*
                * Allow self signed certificates and such
                */
                ServicePointManager.ServerCertificateValidationCallback = delegate
                {
                    return true;
                };
                string smsURL = "https://twilix.exotel.in/v1/Accounts/joyra/Calls/connect";
                HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(smsURL);
                objRequest.Credentials = new NetworkCredential(key, token);
                objRequest.Method = "POST";
                objRequest.ContentLength = postString.Length;
                objRequest.ContentType = "application/x-www-form-urlencoded";
                // post data is sent as a stream                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
                StreamWriter opWriter = null;
                opWriter = new StreamWriter(objRequest.GetRequestStream());
                opWriter.Write(postString);
                opWriter.Close();

                // returned values are returned as a stream, then read into a string                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                string postResponse = null;
                //  using (StreamReader responseStream = new StreamReader(objResponse.GetResponseStream()))
                //  {
                //    postResponse = responseStream.ReadToEnd();
                //    responseStream.Close();
                //}
                objResponse.Close();
                return (postResponse);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
              
            }
            return "";
        }
        public static void Main(string[] args)
        {
            //		ConnectCall c = new ConnectCall ("YourExotelSID", "YourExotelToken");
            //		string response = c.connectCustomerToAgent("Customer's no", "Agent's no", "Your Exotel VN", "trans");
            //		Console.WriteLine(response);
            //		string response = c.connectCustomerToApp("Customer's no", "http://my.exotel.in/exoml/start/<app id>","Your Exotel VN","trans");
            //		Console.WriteLine(response);
        }
    }

}