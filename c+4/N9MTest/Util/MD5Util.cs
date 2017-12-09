using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Util
{
    /// <summary>
    /// Md5
    /// </summary>
    public class MD5Util
    {
        public static string MD5Encrypt(string pToEncrypt, string sKey)
        {
            HMACMD5 hmacmd = new HMACMD5(Encoding.Default.GetBytes(sKey));
            byte[] inArray = hmacmd.ComputeHash(Encoding.Default.GetBytes(pToEncrypt));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < inArray.Length; i++)
                sb.Append(inArray[i].ToString("X2"));
            hmacmd.Clear();
            return sb.ToString();
        }

        public static string MD5Decrypt(string pToDecrypt, string sKey)
        {
            return "";
        }
    }
}
