using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public static class Server
    {
        private static Listener listener;

        private static List<ClientHelper> connectedClients;

        public static void Init()
        {
            Console.Title = "Server";

            listener = new Listener(Program.IP, Program.port);
            listener.SocketAccepted += new Listener.SocketAcceptedHandler(listener_SocketAccepted);
            listener.Start();
            connectedClients = new List<ClientHelper>();
        }

        private static void listener_SocketAccepted(Socket e)
        {
            ClientHelper client = new ClientHelper(e);
            client.Received += new ClientHelper.ClientReceivedHandler(client_Received);
            client.Disconnected += new ClientHelper.ClientDisconnectHandler(client_Disconnected);

            connectedClients.Add(client);
            Console.WriteLine($"Client {client.endPoint} connected");
        }

        private static void client_Disconnected(ClientHelper sender)
        {
            for (int i = 0; i < connectedClients.Count; i++)
            {
                if (connectedClients[i].ID == sender.ID)
                {
                    connectedClients.RemoveAt(i);
                    break;
                }
            }
        }

        private static void client_Received(ClientHelper sender, byte[] data)
        {
            for (int i = 0; i < connectedClients.Count; i++)
            {
                var kek = Console.ReadLine();
                int s = connectedClients[i].sck.Send(Encoding.ASCII.GetBytes(kek));
            }

            //for (int i = 0; i < connectedClients.Count; i++)
            //{
            //    if (connectedClients[i].ID == sender.ID)
            //    {
            //        var kek = Encoding.ASCII.GetString(data);
            //        if (kek == "exit")
            //        {
            //            connectedClients[i].Close();
            //            return;
            //        }

            //        Console.WriteLine($"{connectedClients[i].endPoint} message: {kek}");
            //        break;
            //    }
            //}
        }

        private static void l_SocketAccepted(Socket e)
        {
            Console.WriteLine($"New connection: {e.RemoteEndPoint}");
        }
    }
}
