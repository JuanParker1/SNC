using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

namespace ShopNow.Helpers
{
   
    public class AdminHelpers 
    {
       public static string key = "shopnow789123456";
        public static string pass = "TrendJoyra123456";
        //Admin Security
        public static string SecureData(string input)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(input);
            Aes kgen = Aes.Create("AES");
            kgen.Mode = CipherMode.ECB;
            kgen.Key = keyArray;
            ICryptoTransform cTransform = kgen.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            string result=Convert.ToBase64String(resultArray, 0, resultArray.Length);
            string final= Encrypt(result, pass);

            return final;
        }

        public static string DeSecureData(string input)
        {

            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);

            byte[] toEncryptArray = Convert.FromBase64String(input.Replace(" ", "+"));
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            // rDel.Padding = PaddingMode.Zeros;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            string result = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            // string result = UTF8Encoding.UTF8.GetString(resultArray).ToString();        
            string final = Decrypt(result, pass);
            return final;
        }


        public static string Encrypt(string input, string pass)
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
        public static string Decrypt(string input, string pass)
        {

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
            int encrptInt = input + 2;
            byte[] b = BitConverter.GetBytes(encrptInt);
            string str = Convert.ToBase64String(b);
            return str;
        }
        public static int DCodeInt(string input)
        {
            byte[] b = Convert.FromBase64String(input);

            // Return the decoded int.
            int i = BitConverter.ToInt32(b, 0);
            return i-2;
        }

        internal static object ECodeInt(long id)
        {
            throw new NotImplementedException();
        }
    }
}