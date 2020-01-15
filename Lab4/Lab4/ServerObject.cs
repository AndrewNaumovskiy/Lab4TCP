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
        static TcpListener tcpListener; // сервер для прослушивания
        public List<ClientObject> clients = new List<ClientObject>(); // все подключения

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null)
                clients.Remove(client);
        }
        // прослушивание входящих подключений
        protected internal void Listen()
        {
            try
            {
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

        // трансляция сообщения подключенным клиентам
        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id) // если id клиента не равно id отправляющего
                {
                    clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
            }
        }

        bool beforeGenerate = true;
        public void Parse(string command, ClientObject sender)
        {

            var parameters = command.Split(' ');
            switch (parameters[0])
            {
                case "OUT":
                    int number = Convert.ToInt32(parameters[1]);
                    parameters[2].Replace("F", "");
                    SendToSpecificClient($"GENERATE {parameters[2]}", number); 
                    break;
                case "GENERATE":
                    beforeGenerate = false;
                    string fibonacciGenerated = FibonachiGenerator.Generate(Convert.ToInt32(parameters[1])).ToString();
                    SendToSpecificClient(fibonacciGenerated, clients.FindIndex(x => x == sender) + 1);
                    break;
                case "DISCONNECT":
                    if (beforeGenerate)
                    {
                        var reconnectClient =  clients[Convert.ToInt32(parameters[1])];
                        clients[Convert.ToInt32(parameters[1])].Close();
                        AddConnection(reconnectClient);
                    }
                    break;
                
            }
        }

        public  void SendToSpecificClient(string message, int number)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            clients[number].Stream.Write(data,0,data.Length);
        }

        // отключение всех клиентов
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }
    }
}
