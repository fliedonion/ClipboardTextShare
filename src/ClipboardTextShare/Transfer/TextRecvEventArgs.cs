using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardTextShare.Transfer {
    public class TextRecvEventArgs : EventArgs{
        public string Text { get; private set; }

        public TextRecvEventArgs() {

        }
        public TextRecvEventArgs(string text) {
            Text = text;
        }
    }
}
