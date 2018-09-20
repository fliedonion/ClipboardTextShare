using ClipboardTextShare.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ClipboardTextShare.ClipboardMonitor {

    [System.Security.Permissions.PermissionSet(
            System.Security.Permissions.SecurityAction.Demand,
            Name = "FullTrust")]
    [Obsolete("User ClipboardListener instead.", true)]
    public class ClipboardViewer : IDisposable {

        private bool isHandling = false;
        private IntPtr hWndNext;
        private HwndSource hWndSource;

        public void Init(IntPtr handle) {
            hWndSource = HwndSource.FromHwnd(handle);
            hWndSource.AddHook(this.WndProc);
            hWndNext = Win32Api.SetClipboardViewer(handle);
            isHandling = true;
        }


        public void Dispose() {
            Win32Api.ChangeClipboardChain(hWndSource.Handle, hWndNext);

            hWndNext = IntPtr.Zero;
            hWndSource.RemoveHook(this.WndProc);

            isHandling = false;
        }

        public delegate void TextCopiedEventHandler(object sender, TextCopiedEventArgs ev);
        public event TextCopiedEventHandler TextCopied;

        private void ProcessClipboardData() {
            if (Clipboard.ContainsText()) {
                var text = Clipboard.GetText();
                if (TextCopied != null) {
                    TextCopied(this, new TextCopiedEventArgs(text));
                }
            }
        }

        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {

            switch (msg) {
                case Win32Api.WM_CHANGECBCHAIN:
                    if (wParam == hWndNext) {
                        hWndNext = lParam;
                    }
                    else if (hWndNext != IntPtr.Zero) {
                        Win32Api.SendMessage(hWndNext, msg, wParam, lParam);
                    }
                    break;
                case Win32Api.WM_DRAWCLIPBOARD:
                    this.ProcessClipboardData();
                    Win32Api.SendMessage(hWndNext, msg, wParam, lParam);
                    break;
            }
            return IntPtr.Zero;
        }
    }
}

