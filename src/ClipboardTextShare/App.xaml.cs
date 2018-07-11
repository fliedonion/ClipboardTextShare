using ClipboardTextShare.TrayIcon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ClipboardTextShare {
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application {

        private NotifyIconWrapper notifyIcon;

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;

            notifyIcon = new NotifyIconWrapper();

        }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);

            notifyIcon.Dispose();

        }
    }
}
