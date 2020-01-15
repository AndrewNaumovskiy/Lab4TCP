using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lab4
{
    public class Listener
    {
        public Socket s;
        public bool Listening { get; set; }

        public int Port { get; set; }
        private string IPAddress { get; set; }

        public Listener(string IP, int port)
        {
            IPAddress = IP;
            Port = port;
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            if (Listening) return;

            s.Bind(new IPEndPoint(System.Net.IPAddress.Parse(IPAddress), Port));
            s.Listen(0);

            s.BeginAccept(callback, null);

            Listening = true;
        }

        public void Stop()
        {
            if (!Listening) return;

            s.Close();
            s.Dispose();
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void callback(IAsyncResult res)
        {
            try
            {
                Socket s = this.s.EndAccept(res);

                if (SocketAccepted != null)
                {
                    SocketAccepted(s);
                }

                this.s.BeginAccept(callback, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public delegate void SocketAcceptedHandler(Socket e);

        public event SocketAcceptedHandler SocketAccepted;
    }
}
