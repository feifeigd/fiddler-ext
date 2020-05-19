using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace d7kj.crypto
{
    public class RSAHelper
    {
        /// RSA加密 
        /// </summary> 
        /// <param name="xmlPublicKey">公钥</param> 
        /// <param name="m_strEncryptString"></param> 
        /// <returns></returns> 
        public static string RSAEncrypt(string xmlPublicKey, string m_strEncryptString)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(xmlPublicKey);
            byte[] bytes = new UnicodeEncoding().GetBytes(m_strEncryptString);
            return Convert.ToBase64String(provider.Encrypt(bytes, false));
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="xmlPrivateKey">私钥</param>
        /// <param name="m_strDecryptString"></param>
        /// <returns></returns>
        public static string RSADecrypt(string xmlPrivateKey, string m_strDecryptString)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(xmlPrivateKey);
            byte[] rgb = Convert.FromBase64String(m_strDecryptString);
            byte[] bytes = provider.Decrypt(rgb, false);
            return new UnicodeEncoding().GetString(bytes);
        }

    }
}
