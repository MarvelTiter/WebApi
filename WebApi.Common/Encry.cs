using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WebApi.Common
{
    public class Encry
    {
        public static string MD5Encry(string orignal)
        {
            MD5 md5 = MD5.Create();
            byte[] buffers = md5.ComputeHash(Encoding.Default.GetBytes(orignal));

            string result = "";
            for (int i = 0; i < buffers.Length; i++)
            {
                result += buffers[i].ToString("x2");
            }
            return result;
        }
    }
}
