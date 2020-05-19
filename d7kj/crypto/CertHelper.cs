using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace d7kj.crypto
{
    public class CertHelper
    {

        string pfxPath = @"E:\开发辅助项目\cert\taisuyuanqing.pfx";
        string cerPath = @"E:\开发辅助项目\cert\taisuyuanqing.cer";

        #region *从文件中读取公钥和私钥

        #region +从文件中读取公钥
        /// <summary>
        /// 从文件中读取公钥
        /// </summary>
        /// <returns></returns>
        public string GetPlulicKey()
        {
            try
            {
                X509Certificate2 x509 = new X509Certificate2(cerPath); 
                return x509.PublicKey.Key.ToXmlString(false);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        #endregion

        #region +从私钥文件中读取私钥
        /// <summary>
        /// 从私钥文件中读取私钥
        /// </summary>
        public void GetPrivateKey()
        {
            try
            {
                X509Certificate2 x509 = new X509Certificate2(pfxPath, "123456", X509KeyStorageFlags.Exportable);
                var publicKey = x509.PublicKey.Key.ToXmlString(false);
                var privateKey = x509.PrivateKey.ToXmlString(true);

                var str = "逍遥帝君 15210253 xiaoyaodijun";

                var result = RSAHelper.RSAEncrypt(publicKey, str);
                var content = RSAHelper.RSADecrypt(privateKey, result);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #endregion



        #region + 从证书库中获取证书
        /// <summary>
        /// 从证书库中获取证书
        /// </summary>
        /// <param name="subjectName">证书名字</param>
        /// <returns></returns>
        public X509Certificate2 GetCertificateFromStore(string subjectName)
        {
            try
            {
                subjectName = "CN=" + subjectName;
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadWrite);
                X509Certificate2Collection storecollection = (X509Certificate2Collection)store.Certificates;
                foreach (X509Certificate2 x509 in storecollection)
                {
                    if (x509.Subject == subjectName)
                    {
                        return x509;
                    }
                }
                store.Close();
                store = null;
                storecollection = null;
                return null;

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region *导出证书文件

        #region +从证书库中导出私钥文件
        /// <summary>     
        /// 从WINDOWS证书存储区的个人MY区找到主题为subjectName的证书，     
        /// 并导出为pfx文件，同时为其指定一个密码     
        /// 并将证书从个人区删除(如果isDelFromstor为true)     
        /// </summary>     
        /// <param name="subjectName">证书主题，不包含CN=</param>     
        /// <param name="pfxFileName">pfx文件名</param>     
        /// <param name="password">pfx文件密码</param>     
        /// <param name="isDelFromStore">是否从存储区删除</param>     
        /// <returns></returns>     
        public static bool ExportToPfxFile(string subjectName, string pfxFileName,
            string password, bool isDelFromStore)
        {
            subjectName = "CN=" + subjectName;
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            X509Certificate2Collection storecollection = (X509Certificate2Collection)store.Certificates;
            foreach (X509Certificate2 x509 in storecollection)
            {
                if (x509.Subject == subjectName)
                {

                    byte[] pfxByte = x509.Export(X509ContentType.Pfx, password);
                    using (FileStream fileStream = new FileStream(pfxFileName, FileMode.Create))
                    {
                        // Write the data to the file, byte by byte.     
                        for (int i = 0; i < pfxByte.Length; i++)
                            fileStream.WriteByte(pfxByte[i]);
                        // Set the stream position to the beginning of the file.     
                        fileStream.Seek(0, SeekOrigin.Begin);
                        // Read and verify the data.     
                        for (int i = 0; i < fileStream.Length; i++)
                        {
                            if (pfxByte[i] != fileStream.ReadByte())
                            {
                                fileStream.Close();
                                return false;
                            }
                        }
                        fileStream.Close();
                    }
                    if (isDelFromStore == true)
                        store.Remove(x509);
                }
            }
            store.Close();
            store = null;
            storecollection = null;
            return true;
        }
        #endregion

        #region + 从证书库中导出公钥文件
        /// <summary>
        /// 从证书库中导出公钥文件
        /// </summary>
        /// <param name="subjectName">证书名字</param>
        /// <param name="cerFileName">存放公钥的文件路径</param>
        public void ExportToCerFile(string subjectName, string cerFileName)
        {
            try
            {
                subjectName = "CN=" + subjectName;
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                X509Certificate2Collection storecollection = (X509Certificate2Collection)store.Certificates;
                foreach (X509Certificate2 x509 in storecollection)
                {
                    if (x509.Subject != subjectName) continue;

                    byte[] cerByte = x509.Export(X509ContentType.Cert);
                    using (FileStream fileStream = new FileStream(cerFileName, FileMode.Create))
                    {
                        // Write the data to the file, byte by byte.     
                        for (int i = 0; i < cerByte.Length; i++)
                            fileStream.WriteByte(cerByte[i]);
                        // Set the stream position to the beginning of the file.     
                        fileStream.Seek(0, SeekOrigin.Begin);
                        // Read and verify the data.     
                        for (int i = 0; i < fileStream.Length; i++)
                        {
                            if (cerByte[i] != fileStream.ReadByte())
                            {
                                fileStream.Close();

                            }
                        }
                        fileStream.Close();
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion 

        #endregion

        #region + 从证书库中获取私钥和公钥
        /// <summary>
        /// 从证书库中获取私钥和公钥
        /// </summary>
        /// <param name="subjectName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string GetPrivateKeyFromStore(string subjectName, string password)
        {
            try
            {
                subjectName = "CN=" + subjectName;
                //CurrentUser=当前用户   LocalMachine=本地计算机
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadWrite);
                X509Certificate2Collection storecollection = (X509Certificate2Collection)store.Certificates;
                foreach (X509Certificate2 x509 in storecollection)
                {
                    if (x509.Subject != subjectName) continue;
                    x509.Export(X509ContentType.Pfx, password);
                    var privateKey = x509.PrivateKey.ToXmlString(true);
                    var publicKey = x509.PublicKey.Key.ToXmlString(false);

                    var str = "逍遥帝君 15210253 xiaoyaodijun";

                    var result = RSAHelper.RSAEncrypt(publicKey, str);
                    var content = RSAHelper.RSADecrypt(privateKey, result);
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion
    }
}
