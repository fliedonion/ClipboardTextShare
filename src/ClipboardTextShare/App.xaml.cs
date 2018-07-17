using ClipboardTextShare.ClipboardMonitor;
using ClipboardTextShare.Transfer;
using ClipboardTextShare.TrayIcon;
using Mono.Options;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ClipboardTextShare {
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application {

        private Logger logger;

        private NotifyIconWrapper notifyIcon;
        private ClipboardViewer cv;
        private UdpTextTransfer udp;

        const int DefaultPort = 51871;
        class OptionEntity {
            public IPAddress RemoteIp { get; set; }
            public IPAddress LocalIp { get; set; }
            public int Port { get; set; }

            internal OptionEntity() {
                Port = DefaultPort;
                LocalIp = IPAddress.Any;
            }

            public override string ToString() {
                return string.Format("local: {0}, remote: {1}, port {2}", LocalIp, RemoteIp, Port);
            }
        }


        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            logger = LogManager.GetCurrentClassLogger();

            Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            logger.Info("Startup");

            var options = new OptionEntity();
            try {
                options = ParseArgs(e.Args, options);
            }
            catch (OptionException ex) {
                logger.Error(ex);
                MessageBox.Show(ex.Message, "Invalid Option", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }
            catch (Exception ex) {
                logger.Error(ex);
                MessageBox.Show(ex.Message, "Option Parse Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }
            logger.Info("option : " + options.ToString());


            notifyIcon = new NotifyIconWrapper();

            udp = new UdpTextTransfer(options.LocalIp, options.RemoteIp, options.Port);
            udp.TextRecieved += udp_TextRecieved;

            cv = new ClipboardViewer();
            cv.TextCopied += cv_TextCopied;
            cv.Init(GetMainWindowHandle());
        }

        private OptionEntity ParseArgs(string[] args, OptionEntity defaultValues){
            var opt = new OptionEntity { RemoteIp = defaultValues.RemoteIp, LocalIp = defaultValues.LocalIp, Port = defaultValues.Port };


            string destIpOption = null;
            string localIpOption = null;
            int? portOption = null;
            string portOptionDesc = "Port to use ";
            if (defaultValues.Port > 0) {
                portOptionDesc += "(default: " + defaultValues.Port + " )";
            }

            var p = new OptionSet() {
                {"d|dest=", "Distination Address to Send Text (require)", v=>destIpOption = v},
                {"a|addr=", "Address to listen (default: IPAddress.Any)", v=>localIpOption = v},
                {"p|port=", portOptionDesc, (int i)=> portOption = i},
            };

            p.Parse(args);

            if (destIpOption == null) {
                throw new OptionException("Destination IP Address must be specified by -d option", "d");
            }
            else {
                opt.RemoteIp = IPAddress.Parse(destIpOption);
            }

            if (localIpOption != null) {
                opt.LocalIp = IPAddress.Parse(localIpOption);
            }

            if (portOption.HasValue) {
                opt.Port = portOption.Value;
            }
            return opt;
        }




        void udp_TextRecieved(object sender, TextRecvEventArgs ev) {

            var text = ev.Text;
            try {
                ClipboardTextWriter.SetText(text);
            }
            catch (Exception ex) {
                logger.Error(ex);

                if (Debugger.IsAttached) {
                    throw;
                }
            }
        }

        void cv_TextCopied(object sender, TextCopiedEventArgs ev) {
            try {
                udp.Send(ev.Text);
            }
            catch (Exception ex) {
                logger.Error(ex);

                if (Debugger.IsAttached) {
                    throw;
                }
            }
        }


        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
            
            notifyIcon.Dispose();
            cv.Dispose();
        }

        private IntPtr GetMainWindowHandle() {
            var wnd = new MainWindow();
            var helper = new WindowInteropHelper(wnd);
            return helper.EnsureHandle();
        }

    }
}
