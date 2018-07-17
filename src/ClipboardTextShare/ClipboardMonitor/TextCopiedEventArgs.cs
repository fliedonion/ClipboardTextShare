using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardTextShare.ClipboardMonitor {
    public class TextCopiedEventArgs : EventArgs{
        public string Text { get; private set; }

        public TextCopiedEventArgs() {

        }
        public TextCopiedEventArgs(string text) {
            Text = text;
        }
    }
}
