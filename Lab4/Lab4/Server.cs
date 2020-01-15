using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab4
{
    public static class Server
    {
        static ServerObject server; // сервер
        static Thread listenThread; // потока для прослушивания

        public static void Init()
        {
            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start(); //старт потока
                
                while (true)
                {
                    var kek = Console.ReadLine();
                    ParseKekw(kek);
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }

        private static void ParseKekw(string kek)
        {
            var mewo = kek.Split(' ');
            switch (mewo[0])
            {
                case "DISCONNECT": // poka zabili boltyaru
                    var number = Convert.ToInt32(mewo[1]);
                    if (server.clients[number].ChildNumber.Count == 0) break;

                    var reconnectClient = server.clients[number];
                    server.clients[Convert.ToInt32(mewo[1])].Close();
                    server.AddConnection(reconnectClient);
                    break;
                case "GENERATE":
                    server.Generate(Convert.ToInt32(mewo[1]));
                    break;
            }
        }
    }
}
