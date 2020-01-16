using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Lab4
{
    public class ServerObject
    {
        static TcpListener tcpListener;
        public List<ClientObject> clients = new List<ClientObject>();
        public List<int> ChildNumber;

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                clients.Remove(client);
        }
        protected internal void Listen()
        {
            try
            {
                ChildNumber = new List<int>();

                tcpListener = new TcpListener(IPAddress.Parse(Program.IP), Program.port);
                tcpListener.Start();
                Console.WriteLine("Server started. Waiting for connections...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id)
                {
                    clients[i].Stream.Write(data, 0, data.Length);
                }
            }
        }

        private int nextId = -1;
        public int GetNextId()
        {
            nextId++;
            if (nextId >= ChildNumber.Count)
                nextId = 0;
            return nextId;
        }

        private List<int> logs;
        public void Generate(int number)
        {
            logs = new List<int>();

            if (number < 2 || ChildNumber.Count == 0)
            {
                logs.Add(0);
                Protocol(number);
            }
            else
            {
                if (ChildNumber.Count == 1)
                {
                    Protocol(number);
                    generate(number - 2, ChildNumber[GetNextId()] - 1);
                }
                else
                {
                    generate(number - 1, ChildNumber[GetNextId()] - 1);
                    generate(number - 2, ChildNumber[GetNextId()] - 1);
                }
            }

            string kek = $"F{number}: ";
            logs.Distinct().ToList().ForEach(x => kek += x.ToString()+" ");
            FileHelper.Log("server_logs.txt", kek);
        }

        private void generate(int number, int id)
        {
            logs.Add(id);

            if (number < 2 || clients[id].ChildNumber.Count == 0)
            {
                SendToSpecificClient(GenProtocol(number), id);
                return;
            }
            if (clients[id].ChildNumber.Count == 1)
            {
                generate(number - 1, id);
                generate(number - 2, clients[id].ChildNumber[clients[id].GetNextId()] - 1);
                return;
            }
            generate(number - 1, clients[id].ChildNumber[clients[id].GetNextId()] - 1);
            generate(number - 2, clients[id].ChildNumber[clients[id].GetNextId()] - 1);
        }

        public void Parse(string command, ClientObject sender)
        {
            var parameters = command.Split(' ');
            switch (parameters[0])
            {
                case "OUT":
                    int number = Convert.ToInt32(parameters[1]);
                    var kek = parameters[2].Replace("F", "");
                    SendToSpecificClient($"GENERATE {kek}", number - 1);
                    break;
                case "GENERATE":
                    string fibonacciGenerated = FibonachiGenerator.Generate(Convert.ToInt32(parameters[1])).ToString();
                    SendToSpecificClient(fibonacciGenerated, clients.FindIndex(x => x == sender) + 1);
                    break;
            }
        }

        public string GenProtocol(int fib, int to = -1)
        {
            if (to == -1)
            {
                return $"PROTOCOL IN F{fib}";
            }

            return $"PROTOCOL OUT {to} F{fib}";
        }

        public void Protocol(int fib, int to = -1)
        {
            string msg;
            if (to == -1)
            {
                msg = $"IN F{fib}";
            }
            else
            {
                msg = $"OUT {to} F{fib}";
            }
            FileHelper.Log(Program.LogFilePath, msg);
        }

        public  void SendToSpecificClient(string message, int number)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            clients[number].Stream.Write(data,0,data.Length);
        }

        protected internal void Disconnect()
        {
            tcpListener.Stop();

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            Environment.Exit(0);
        }
    }
}
