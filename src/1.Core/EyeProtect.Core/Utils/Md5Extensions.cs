using System;
using System.Security.Cryptography;
using System.Text;

namespace EyeProtect.Core.Utils
{
    public static class Md5Extensions
    {
        /// <summary>
        /// 对字符串进行MD5加密
        /// </summary>
        /// <param name="strToEncode">要加密的字符串</param>
        /// <returns></returns>
        public static string Md5(string strToEncode)
        {
            MD5 md5 = MD5.Create();
            byte[] md5Byte = md5.ComputeHash(Encoding.UTF8.GetBytes(strToEncode));
            return BitConverter.ToString(md5Byte).Replace("-", "");
        }

        /// <summary>
        /// 计算指定数据的哈希值
        /// </summary>
        /// <param name="md5"></param>
        /// <param name="data">要加密的字符串</param>
        /// <param name="encoding">指定字符编码</param>
        /// <param name="format">格式</param>
        public static string ComputeHash(this MD5 md5, string data, Encoding encoding, string format = "x2")
        {
            var bytes = md5.ComputeHash(encoding.GetBytes(data));
            return bytes.ToHexString(format);
        }


        /// <summary>
        /// 计算密码的哈希值
        /// </summary>
        public static string ComputePasswordHash(this MD5 md5, string password, string securityStamp)
        {
            return md5.ComputeHash($"{password}{securityStamp}", Encoding.UTF8);
        }


        /// <summary>
        /// 转为十六进制字符串
        /// </summary>
        public static string ToHexString(this byte[] bytes, string format = "x2")
        {
            var sign = new StringBuilder();
            foreach (var b in bytes)
            {
                sign.Append(b.ToString(format ?? "x2"));
            }
            return sign.ToString();
        }

        public static MD5 md5Hash = null;
        public static SHA1 sha1Hash = null;

        /// <summary>
        /// 初始化对象
        /// </summary>
        private static void GetHashType()
        {
            if (md5Hash == null)
                md5Hash = MD5.Create();
            if (sha1Hash == null)
                sha1Hash = SHA1.Create();
        }

        /// <summary>
        /// MD5\SHA1加密
        /// </summary>
        public static string GetHashEncryptionCode(string input, ushort type = 1)
        {
            byte[] data = null;
            GetHashType();
            if (type == 1)
                data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            else if (type == 2)
                data = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// SHA1加密方式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SHAEncrypt(string input)
        {
            byte[] data = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
