using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.Helpers
{
    public class Token
    {
        public static string Generate()
        {
            Guid guid = Guid.NewGuid();
            string token = Convert.ToBase64String(guid.ToByteArray()).Replace("=", "").Replace("+", "");
            return token;
        }

        public static bool Check(string token = "")
        {
            if (Helpers.Token.List.Any(i => i.Value == token))
            {
                var temp = Helpers.Token.List.Single(i => i.Value == token);
                if (temp.Expiry <= DateTime.Now)
                {
                    Helpers.Token.List.Remove(temp);
                    return false;
                }
                else
                {
                    Helpers.Token.List.Remove(temp);
                    return true;
                }
            }
            else
                return false;
        }

        public static List<Token> List = new List<Token>();

        public Token(string value)
        {
            this.Value = value;
            this.Expiry = DateTime.Now.AddSeconds(60);
        }

        public string Value { get; set; }

        public DateTime Expiry { get; set; }
    }

    public class DRC
    {
        private const int _numlength = 5;
        private const int _alplength = 3;
        public static string Generate(string prefix)
        {
            

            //string code = prefix + Convert.ToInt64(DateTime.Now.ToString("yyMMddHHmmssfff")).ToString("X2").PadLeft(13, '0');
            //string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //string numeric = "0123456789";
            //string hicode = new string(Enumerable.Repeat(alpha, _alplength).Select(s => s[_randomizer.Next(s.Length)]).ToArray());
            //string lowcode = new string(Enumerable.Repeat(numeric, _numlength).Select(s => s[_randomizer.Next(s.Length)]).ToArray());

            //prefix = string.IsNullOrEmpty(prefix) ? new string(Enumerable.Repeat(alpha, _alplength).Select(s => s[_randomizer.Next(3)]).ToArray()) : prefix;

            // return prefix + hicode + lowcode;
            return prefix + RandomStringUtils.RandomStringUtils.RandomAlphabetic(10);
        }
        
        public static string Format(string code)
        {
            code = code.Replace("_", string.Empty);
            string formatted;
            formatted = System.Text.RegularExpressions.Regex.Replace(code, @"^(...)(...)(.....)$", "$1-$2-$3");
            return formatted;

        }

        private const int _Delnumlengths = 6;
        public static string GenerateDelivaryBoy()
        {
            string num = "0123456789";
            string lowcode = new string(Enumerable.Repeat(num, _Delnumlengths).Select(s => s[_randomizer.Next(s.Length)]).ToArray());

            return lowcode;
        }

        private const int _numlengths = 6;
        public static string GenerateOTP()
        {          
            string numeric = "0123456789";         
            string lowcode = new string(Enumerable.Repeat(numeric, _numlengths).Select(s => s[_randomizer.Next(s.Length)]).ToArray());

            return  lowcode;
        }
        private const int _alplengths = 4;
        private const int _numlen = 2;
        private const int _sp1 = 1;
        public static string GeneratePassword()
        {
            string special = "!@#$%&*?";
            string numeric = "0123456789";
            string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string hicode = new string(Enumerable.Repeat(alpha, _alplengths).Select(s => s[_randomizer.Next(s.Length)]).ToArray());
            string lowcode = new string(Enumerable.Repeat(numeric, _numlen).Select(s => s[_randomizer.Next(s.Length)]).ToArray());
            string precode = new string(Enumerable.Repeat(special, _sp1).Select(s => s[_randomizer.Next(s.Length)]).ToArray());
            string postcode = new string(Enumerable.Repeat(special, _sp1).Select(s => s[_randomizer.Next(s.Length)]).ToArray());

            return hicode + precode + lowcode + postcode;
        }
        private const int _num = 6;
        public static string GenerateSNIN()
        {
            string prefix = "MSN";
            string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numeric = "0123456789";
            prefix = string.IsNullOrEmpty(prefix) ? new string(Enumerable.Repeat(alpha, _alplength).Select(s => s[_randomizer.Next(3)]).ToArray()) : prefix;
            string lowcode = new string(Enumerable.Repeat(numeric, _num).Select(s => s[_randomizer.Next(s.Length)]).ToArray());
            return prefix + lowcode;
        }
        private static Random _randomizer = new Random();

    }
}