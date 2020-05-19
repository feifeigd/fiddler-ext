using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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

        void InitFiddler()
        {
            // 设置别名
            FiddlerApplication.SetAppDisplayName("FiddlerCoreApp");

            CertMaker.createRootCert();
            X509Certificate2 oRootCert = CertMaker.GetRootCertificate();//Returns the Root certificate that Fiddler uses to generate per-site certificates used for HTTPS interception. 

            System.Security.Cryptography.RSACryptoServiceProvider.UseMachineKeyStore = true;
            System.Security.Cryptography.DSACryptoServiceProvider.UseMachineKeyStore = true;

            X509Store certStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                certStore.Open(OpenFlags.ReadWrite);
            try
            {
                certStore.Remove(oRootCert);
                certStore.Add(oRootCert);
            }
            catch(Exception e)
            {
                log(e.Message);
            }
            finally
            {
                certStore.Close();
            }

            // 发送请求之前执行的操作
            FiddlerApplication.BeforeRequest += BeforeRequest;
            FiddlerApplication.BeforeResponse += BeforeResponse;
            FiddlerApplication.AfterSessionComplete += delegate (Fiddler.Session oS)
            {
                //Console.WriteLine("Finished session:\t" + oS.fullUrl);   
                // this.Invoke(()=> Console.Title = "Session list contains: " + oAllSessions.Count.ToString() + " sessions" ); 
                //log("Session list contains: " + oAllSessions.Count.ToString() + " sessions");
            };
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
            if (txtlog.Lines.Count() > 3000)
            {ClearLog();
            }
            txtlog.AppendText(msg);
            txtlog.AppendText(Environment.NewLine);
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

            log("启动代理成功");
        }

        List<Session> oAllSessions = new List<Session>();
        void BeforeRequest(Session oS)
        {
            Console.WriteLine("Before request for:\t" + oS.fullUrl);
            oS.bBufferResponse = true;
            Monitor.Enter(oAllSessions);
            oAllSessions.Add(oS);
            Monitor.Exit(oAllSessions);
            oS["X-AutoAuth"] = "(default)";
        }
        void BeforeResponse(Session oS)
        {
            oS.utilDecodeResponse();
            string content = oS.GetResponseBodyAsString();
            if (oS.isHTTPS)
            {
                Console.Write("BeforeResponse:");
                Console.WriteLine(oS.fullUrl);
                log("BeforeResponse:" + oS.fullUrl);
                log(content);
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

    }
}
