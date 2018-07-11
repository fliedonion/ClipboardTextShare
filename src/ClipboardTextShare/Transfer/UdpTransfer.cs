using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClipboardTextShare
{
    class UdpTransfer
    {
        private UdpClient recv;
        private UdpClient send;

        public void TestIt() {

            if (recv != null) return;
            recv = new UdpClient();
            recv.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            recv.Client.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 57712));

			send = new UdpClient();
            send.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            send.Client.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 57712));


            recv.BeginReceive(callBack, recv);

            var SendBytes = Encoding.UTF8.GetBytes("テスト送信💢");

            // Debug.Print(IsLocalIp("172.16.222.240").ToString());
            // send.BeginSend(SendBytes, SendBytes.Length, "172.16.222.240", 57712, CallBack, send);
        }

        private bool IsLocalIp(string value) {
            return NetworkInterface.GetAllNetworkInterfaces()
                   .SelectMany(x => x.GetIPProperties().UnicastAddresses)
                   .Select(x => x.Address.ToString())
                   .Contains(value);
        }


        private void CallBack(IAsyncResult ar) {
            send.EndSend(ar);
        }

        private void callBack(IAsyncResult ar) {

			IPEndPoint outIPEP = null;
            var bytes = recv.EndReceive(ar, ref outIPEP);
            Debug.Print(bytes.Length.ToString());
            Application.Current.Dispatcher.
            Invoke(() => {
                Clipboard.SetText(Encoding.UTF8.GetString(bytes));
            });

            recv.BeginReceive(callBack, recv);
        }


    }
}
