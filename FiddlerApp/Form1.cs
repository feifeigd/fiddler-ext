using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Fiddler;

namespace FiddlerApp
{
    public partial class Form1 : Form
    {
        static Proxy oSecureEndpoint;
        static string sSecureEndpointHostname = "localhost";
        static int iPort = 8877;
        static int iSecureEndpointPort = 8889;
        IList<Data> dataList = new List<Data>();
        RegFunc rf = new RegFunc();
        public Form1()
        {
            InitializeComponent();
            string strHostName = Dns.GetHostName(); // 主机名
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            log("代理端口为：" + iPort);
            foreach(var item in ipEntry.AddressList)
            {
                if (item.ToString().IndexOf('.') > 0)
                    log("IP:" + item.ToString());
            }
            log("请先修改wifi代理");
            InitFiddler();
        }
        
        // string url = "https://wxapp.m.jd.com/kwxp/wx/pay.json?code=033P0QL102t46K1hwIL10ejTL10P0QLk&orderId=120318170438&orderType=22&orderTypeCode=0&factPrice=199&appId=wx1edf489cb248852c&fromType=wxapp";
        string GetOrderId(string url)
        {
            Regex rx = new Regex(@"^https://wxapp.m.jd.com/kwxp/wx/pay.json?.*orderId=(\d+).*");
            if (rx.IsMatch(url))
            {
                string orderId = rx.Match(url).Groups[1].Value;
                /*foreach (var item in rx.Match(url).Groups)
                {
                    Console.WriteLine(item);
                }*/
                return orderId;
            }
            return null;
        }

        public bool InstallCertificate()
        {
            if (!CertMaker.rootCertExists())
            {
                if (!CertMaker.createRootCert())
                    return false;

                if (!CertMaker.trustRootCert())
                    return false;
            }

            return true;
        }

        void InitFiddler()
        {
            // 设置别名
            FiddlerApplication.SetAppDisplayName("FiddlerCoreApp");

            InstallCertificate();

            /*CertMaker.createRootCert();
            // CertMaker.dll使用BouncyCastle C＃库（BCMakeCert.dll）从头开始生成新证书。这些证书仅存储在内存中，并与iOS设备兼容。
            X509Certificate2 oRootCert = CertMaker.GetRootCertificate();//Returns the Root certificate that Fiddler uses to generate per-site certificates used for HTTPS interception. 

            System.Security.Cryptography.RSACryptoServiceProvider.UseMachineKeyStore = true;
            System.Security.Cryptography.DSACryptoServiceProvider.UseMachineKeyStore = true;

            X509Store certStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                certStore.Open(OpenFlags.ReadWrite);
            try
            {
                certStore.Remove(oRootCert);
                certStore.Add(oRootCert);
                FiddlerApplication.oDefaultClientCertificate = oRootCert;
            }
            catch(Exception e)
            {
                log(e.Message);
            }
            finally
            {
                certStore.Close();
            }*/

            // 忽略服务器证书错误：
            CONFIG.IgnoreServerCertErrors = true;
            // 发送请求之前执行的操作
            FiddlerApplication.BeforeRequest += BeforeRequest;
            FiddlerApplication.BeforeResponse += BeforeResponse;
            FiddlerApplication.AfterSessionComplete += delegate (Fiddler.Session oS)
            {
                //Console.WriteLine("Finished session:\t" + oS.fullUrl);   
                // this.Invoke(()=> Console.Title = "Session list contains: " + oAllSessions.Count.ToString() + " sessions" ); 
                //log("Session list contains: " + oAllSessions.Count.ToString() + " sessions");
            };

            Fiddler.FiddlerApplication.OnNotification += delegate (object sender, NotificationEventArgs oNEA) { log("** NotifyUser: " + oNEA.NotifyString); };
            Fiddler.FiddlerApplication.Log.OnLogString += delegate (object sender, LogEventArgs oLEA) { log("** LogString: " + oLEA.LogString); };
        }
        public static void WriteCommandResponse(string s)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(s);
            Console.ForegroundColor = oldColor;
        }

        void ClearLog()
        {
            txtlog.Text = "";
            txtlog.AppendText("清空缓存" + Environment.NewLine);
        }

        void log(string msg)
        {
            this.UIThreadInvoke(() =>
            {
                if (txtlog.Lines.Count() > 3000)
                {
                    ClearLog();
                }
                txtlog.AppendText(msg);
                txtlog.AppendText(Environment.NewLine);
            });
        }

        private void btnstart_Click(object sender, EventArgs e)
        {
            WriteCommandResponse("btnstart_Click");

            btnout.Enabled = true;
            btnstart.Enabled = false;

            // 启动方式
            FiddlerCoreStartupFlags oFCSF = FiddlerCoreStartupFlags.Default;

            // Uncomment the next line if you don't want to decrypt SSL traffic.
            // oFCSF = (oFCSF & ~FiddlerCoreStartupFlags.DecryptSSL);

            // 启动代理程序，开始监听http请求
            // 参数： 端口，是否使用windows系统代理(如果为true，系统所有的http访问都会使用该代理) 
            FiddlerApplication.Startup(iPort, true, true, true);

            // We'll also create a HTTPS listener, useful for when FiddlerCore is masquerading as a HTTPS server
            // instead of acting as a normal CERN-style proxy server.
            oSecureEndpoint = FiddlerApplication.CreateProxyEndpoint(iSecureEndpointPort, true, sSecureEndpointHostname);
            if (null != oSecureEndpoint)
            {
                FiddlerApplication.Log.LogFormat("Created secure endpoint listening on port {0}, using a HTTPS certificate for '{1}'", iSecureEndpointPort, sSecureEndpointHostname);
            }
            else
            {
                log("Start SecureEndpointPort failed!");
            }

            log("启动代理成功");
        }

        List<Session> oAllSessions = new List<Session>();
        void BeforeRequest(Session oS)
        {
            Console.WriteLine("Before request for:\t" + oS.fullUrl);
            oS.bBufferResponse = true;   // bBufferResponse这个属性在BeforeRequest里设为true，可以修改响应内容
            Monitor.Enter(oAllSessions);
            oAllSessions.Add(oS);
            Monitor.Exit(oAllSessions);
            oS["X-AutoAuth"] = "(default)";
            Regex rx = new Regex(@"^https://wxapp.m.jd.com/kwxp/wx/pay.json?.*orderId=(\d+).*");
            if (rx.IsMatch(oS.fullUrl))
            {
                oS.oRequest["Accept-Encoding"] = "gzip, deflate";   // br解码失败，所以删除br
            }
        }
        void BeforeResponse(Session oS)
        {
            oS.utilDecodeResponse();
            string content = oS.GetResponseBodyAsString();
            if (oS.isHTTPS)
            {
                /*Console.Write("BeforeResponse:");
                Console.WriteLine(oS.fullUrl);
                log("BeforeResponse:" + oS.fullUrl);
                log(content);//*/
                string orderId = GetOrderId(oS.fullUrl);
                if (!string.IsNullOrWhiteSpace(orderId))
                {
                    log(string.Format("微信支付信息 订单号：{0}", orderId));
                    /*
                     {
	"factPrice": "199",
	"timeStamp": "1589958133",
	"req_from": "1",
	"signType": "MD5",
	"package": "prepay_id=wx201502134422441cc6aeceff1955007000",
	"nonceStr": "3ae4f12b897c4bb51327a8e1c921df7d",
	"desPin": "PBe5KeQQoCUyWZVGYttRaQ==",
	"isInternationalBuyAppId": false,
	"payEnum": "710",
	"jdPayId": "71046320052015021302562",
	"paySign": "AA1377A64D0AB2D60AF60B573399EC04"
}
                     */
                    string headers = oS.ResponseHeaders.ToString();
                    Encoding encode = oS.GetResponseBodyEncoding();
                    log(content);

                }
            }

        }
        private void btnout_Click(object sender, EventArgs e)
        {
            btnstart.Enabled = true;
            btnout.Enabled = false;
            if (null != oSecureEndpoint) oSecureEndpoint.Dispose();
            FiddlerApplication.Shutdown();
            log("断开代理成功");
            Thread.Sleep(500);
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            ClearLog();
        }
    }

    class RegFunc
    {
        public ArrayList GetStrArr(string pContent, string regBegKey, string regEndKey)
        {
            ArrayList arr = new ArrayList(); 
            string regular = "(?<={0})(.|\n)*?(?={1})";
            regular = string.Format(regular, regBegKey, regEndKey);
            Regex regex = new Regex(regular, RegexOptions.IgnoreCase);
            MatchCollection mc = regex.Matches(pContent);
            foreach (Match m in mc)
            {
                arr.Add(m.Value.Trim());
            }
            return arr;
        }

        public string GetStr(string pContent, string regBegKey, string regEndKey)
        {
            string regstr = "";
            string regular = "(?<={0})(.|\n)*?(?={1})";
            regular = string.Format(regular, regBegKey, regEndKey);
            Regex regex = new Regex(regular, RegexOptions.IgnoreCase);
            Match m = regex.Match(pContent);
            if (m.Length > 0)
            {
                regstr = m.Value.Trim();
            }
            return regstr;
        }
    }
}
