using System;
using System.Security.Cryptography;
using System.Text;
using Talk.Extensions;

namespace Host.Common
{
    public static class EncryptDecryptExtension
    {

        //可在配置文件配置自己的DES3Key - 必须16位
        private static readonly string des3key = ConfigurationManager.GetTryConfig("DES3Key", "73495773n~@^v&B6");

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"></param>        
        /// <returns></returns>
        public static string DES3Encrypt(this string data)
        {
            byte[] inputArray = Encoding.UTF8.GetBytes(data);
            var tripleDES = TripleDES.Create();
            var byteKey = Encoding.UTF8.GetBytes(des3key);
            byte[] allKey = new byte[24];
            Buffer.BlockCopy(byteKey, 0, allKey, 0, 16);
            Buffer.BlockCopy(byteKey, 0, allKey, 16, 8);
            tripleDES.Key = allKey;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DES3Decrypt(this string data)
        {
            byte[] inputArray = Convert.FromBase64String(data);
            var tripleDES = TripleDES.Create();
            var byteKey = Encoding.UTF8.GetBytes(des3key);
            byte[] allKey = new byte[24];
            Buffer.BlockCopy(byteKey, 0, allKey, 0, 16);
            Buffer.BlockCopy(byteKey, 0, allKey, 16, 8);
            tripleDES.Key = allKey;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }
    }
}
