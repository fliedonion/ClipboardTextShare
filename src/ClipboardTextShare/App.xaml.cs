using ClipboardTextShare.ClipboardMonitor;
using ClipboardTextShare.Transfer;
using ClipboardTextShare.TrayIcon;
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

        private NotifyIconWrapper notifyIcon;
        private ClipboardViewer cv;
        private UdpTextTransfer udp;

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;

            notifyIcon = new NotifyIconWrapper();

            udp = new UdpTextTransfer(IPAddress.Parse("127.0.0.1"), IPAddress.Parse("127.0.0.1"), 51871);
            udp.TextRecieved += udp_TextRecieved;

            cv = new ClipboardViewer();
            cv.TextCopied += cv_TextCopied;
            cv.Init(GetMainWindowHandle());
        }


        void udp_TextRecieved(object sender, TextRecvEventArgs ev) {

            var text = ev.Text;
            try {
                ClipboardTextWriter.SetText(text);
            }
            catch (Exception ex) {
                // TODO: logging
                Debug.Print(ex.ToString());
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
                // TODO: logging
                Debug.Print(ex.ToString());
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
