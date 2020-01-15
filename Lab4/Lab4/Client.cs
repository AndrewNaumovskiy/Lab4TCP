using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public static class Client
    {
        private static Socket sck;
        public static void Init()
        {
            Console.Title = "Client";

            sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sck.Connect(Program.IP, Program.port);
            Console.WriteLine("Connected");
            byte[] buffer = new byte[100];

            while (true)
            {
                Task.Run(() =>
                {
                    sck.Accept();
                    sck.Receive(buffer);

                    Console.WriteLine(Encoding.ASCII.GetString(buffer));
                });
                //Task.Run(() =>
                //{
                //    string kek = Console.ReadLine();

                //    int s = sck.Send(Encoding.ASCII.GetBytes(kek));
                //    if (s > 0)
                //    {
                //        Console.WriteLine("Sended");
                //    }
                //});
            }
        }

    }
}
