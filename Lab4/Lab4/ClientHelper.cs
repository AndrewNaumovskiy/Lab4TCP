using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lab4
{
    public class ClientHelper
    {
        public string ID { get; set; }

        public IPEndPoint endPoint { get; set; }

        public Socket sck;

        public ClientHelper(Socket acceptedSocket)
        {
            sck = acceptedSocket;
            ID = Guid.NewGuid().ToString();
            endPoint = (IPEndPoint)sck.RemoteEndPoint;
            sck.BeginReceive(new byte[] { 0 }, 0, 0, 0, callback, null);
        }

        public void callback(IAsyncResult ar)
        {
            try
            {
                sck.EndReceive(ar);

                byte[] buffer = new byte[100];
                int rec = sck.Receive(buffer, buffer.Length, 0);

                if (rec < buffer.Length)
                {
                    Array.Resize<byte>(ref buffer, rec);
                }

                if (Received != null)
                {
                    Received(this, buffer);
                }
                sck.BeginReceive(new byte[] { 0 }, 0, 0, 0, callback, null);

                if (Send != null)
                {
                    Send(this, buffer);
                }
                sck.BeginSend(new byte[] { 0 }, 0, 0, 0, callback, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Close();
                if (Disconnected != null)
                {
                    Disconnected(this);
                }
            }
        }

        public void Close()
        {
            sck.Close();
            sck.Dispose();
        }

        public delegate void ClientReceivedHandler(ClientHelper sender, byte[] data);
        public delegate void ClientSendHandler(ClientHelper sender, byte[] data);
        public delegate void ClientDisconnectHandler(ClientHelper sender);

        public event ClientReceivedHandler Received;
        public event ClientSendHandler Send;
        public event ClientDisconnectHandler Disconnected;

    }
}
