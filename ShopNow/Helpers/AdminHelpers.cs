using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

namespace ShopNow.Helpers
{
   
    public class AdminHelpers 
    {
       // public static string key = "shopnow789123456";
       // public static string pass = "TrendJoyra123456";
        public static string pass = "Aryoj987654";
        //Admin Security

        public static string ECode(string input)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(pass);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string DCode(string input)
        {
            input = input.Replace(" ", "+");
            int mod4 = input.Length % 4;
            if (mod4 > 0)
            {
                input += new string('=', 4 - mod4);
            }
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(pass);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        public static string ECodeInt(int input)
        {
            string s = Convert.ToString(input) + "f";
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(s);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static int DCodeInt(string input)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(input);
            string previousDecode = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            previousDecode = previousDecode.Remove(previousDecode.Length - 1, 1);
            return Convert.ToInt32(previousDecode);
        }

        public static string ECodeLong(long input)
        {
            string s = Convert.ToString(input) + "f";
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(s);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static long DCodeLong(string input)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(input);
            string previousDecode = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            previousDecode = previousDecode.Remove(previousDecode.Length - 1, 1);
            return Convert.ToInt64(previousDecode);
        }
    }
}