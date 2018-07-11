using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClipboardTextShare.TrayIcon {
    public partial class NotifyIconWrapper : Component {
        public NotifyIconWrapper() {
            InitializeComponent();

            toolStripMenuItem1.Click += toolStripMenuItem1_Click;

        }

        void toolStripMenuItem1_Click(object sender, EventArgs e) {
            Application.Current.Shutdown();
        }

        public NotifyIconWrapper(IContainer container) {
            container.Add(this);

            InitializeComponent();
        }
    }
}
