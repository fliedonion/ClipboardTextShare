using ClipboardTextShare.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ClipboardTextShare.ClipboardMonitor {
    [System.Security.Permissions.PermissionSet(
            System.Security.Permissions.SecurityAction.Demand,
            Name = "FullTrust")]
    public class ClipboardListener: IObservable<string>, IDisposable  {

        private bool isHandling = false;
        private HwndSource hWndSource;
        private IntPtr handle;

        private Subject<string> provider;
        public bool IsReady {
            get { return isHandling; }
        }

        public ClipboardListener(IntPtr handle) {
            if (handle == IntPtr.Zero) return;
            
            hWndSource = HwndSource.FromHwnd(handle);
            if (hWndSource != null) {
                hWndSource.AddHook(this.WndProc);
                this.handle = handle;
                Win32Api.AddClipboardFormatListener(this.handle);

                provider = new Subject<string>();
                isHandling = true;
            }
        }


        public void Dispose() {
            if (isHandling) {
                hWndSource.RemoveHook(this.WndProc);
                Win32Api.RemoveClipboardFormatListener(this.handle);
                isHandling = false;
                
                provider.OnCompleted();
                provider.Dispose();
                provider = null;
            }
        }

        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            switch (msg) {
                case Win32Api.WM_CLIPBOARDUPDATE:
                    OnClipboardUpdated();
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnClipboardUpdated() {

            IDataObject iData = Clipboard.GetDataObject();

            if (iData.GetDataPresent(DataFormats.Text)) {
                string text = iData.GetData(DataFormats.Text) as string;
                provider.OnNext(text);
            }
            //else if (iData.GetDataPresent(DataFormats.Bitmap)) {
            //    Bitmap image = iData.GetData(DataFormats.Bitmap) as Bitmap;
            //}
        }

        public IDisposable Subscribe(IObserver<string> observer) {
            if (provider != null) {
                return provider.Subscribe(observer);    
            }
            return Observable.Empty<string>().Subscribe(observer);
        }
    }
}
