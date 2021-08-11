using System.Configuration;
using System.Web.Configuration;

namespace ShopNow.Base
{
    public static  class BaseClass
    { 
        public static string smallImage = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Small/"; 
        public static string mediumImage = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Medium/"; 
        public static string largeImage = "https://s3.ap-south-1.amazonaws.com/shopnowchat.com/Large/";
        public static string razorpaykey = WebConfigurationManager.AppSettings["razorKey"];
        public static string razorpaySecretkey = WebConfigurationManager.AppSettings["razorSecretKey"];
    }
}