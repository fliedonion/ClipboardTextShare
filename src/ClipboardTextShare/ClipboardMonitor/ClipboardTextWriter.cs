using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClipboardTextShare.ClipboardMonitor {
    class ClipboardTextWriter {

        const uint CLIPBRD_E_CANT_OPEN = 0x800401D0;


        public static void SetText(string text){
            if (!Clipboard.IsCurrent(new DataObject(DataFormats.UnicodeText, (Object)text, true))) {

                // note: System.Windows.Forms version Clipboard.SetText retry automatically, however System.Windows version doesn't.
                int MaxRetry = 10;

                for (var i = 0; i < MaxRetry; i++) {
                    try {
                        Clipboard.SetText(text);
                    }
                    catch (COMException ex) {
                        if ((uint)ex.ErrorCode != CLIPBRD_E_CANT_OPEN) {
                            throw;
                        }
                        if (i == MaxRetry - 1) {
                            throw;
                        }
                        System.Threading.Thread.Sleep(30);
                    }
                }
            }

        }
    }
}
