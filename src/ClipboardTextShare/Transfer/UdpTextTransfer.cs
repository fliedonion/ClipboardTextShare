using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClipboardTextShare.Transfer
{
    class UdpTextTransfer : IDisposable
    {

        private UdpClient udpClient;
        private IPEndPoint remoteEndPoint;
        private IPEndPoint localEndPoint;

        private string lastRecieved;

        public delegate void TextRecvEventHandler(object sender, TextRecvEventArgs ev);
        public event TextRecvEventHandler TextRecieved;


        public UdpTextTransfer(IPAddress localAddress, IPAddress remoteAddress,int port) {
            this.remoteEndPoint = new IPEndPoint(remoteAddress, port);
            this.localEndPoint = new IPEndPoint(localAddress, port);

            udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Client.Bind(localEndPoint);

            StartRecieving();
        }

        public void Send(string text) {

            var dataBytes = Encoding.UTF8.GetBytes(text);
            udpClient.BeginSend(dataBytes, dataBytes.Length, remoteEndPoint, SendCallback, udpClient);
        }

        private void SendCallback(IAsyncResult ar) {
            // ((UdpClient)ar.AsyncState).EndSend(ar);
            udpClient.EndSend(ar);
        }

        private void StartRecieving() {
            udpClient.BeginReceive(RequestCallback, udpClient);
        }

        private void RequestCallback(IAsyncResult ar) {
            // var udpClient = ((UdpClient)ar.AsyncState));

            byte[] bytes;
            try {
                IPEndPoint remote = null;
                bytes = udpClient.EndReceive(ar, ref remote);
            }
            catch (SocketException se) {
                // TODO: logging
                Debug.Print(se.ToString());
                return;
            }
            catch (ObjectDisposedException ex) {
                // object has been already closed.
                return;
            }

            if (bytes != null && bytes.Length > 0) {
                Application.Current.Dispatcher.Invoke(() => {
                    try {
                        var gotString = Encoding.UTF8.GetString(bytes);
                        if (TextRecieved != null && gotString != lastRecieved) {
                            lastRecieved = gotString;
                            TextRecieved(this, new TextRecvEventArgs(gotString));
                        }
                    }
                    catch (Exception) {
                        // todo: logging
                    }
                });
            }
        }

        private bool IsLocalIp(string value) {
            return NetworkInterface.GetAllNetworkInterfaces()
                   .SelectMany(x => x.GetIPProperties().UnicastAddresses)
                   .Select(x => x.Address.ToString())
                   .Contains(value);
        }

        public void Dispose() {
            if (udpClient != null) {
                try {
                    udpClient.Close();
                    udpClient.Client.Dispose();
                }
                catch (Exception ex) {
                    // TODO: logging
                    Debug.Print(ex.ToString());
                }
            }
        }
    }
}
