﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Lab4
{
    public class ClientInstance
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }

        readonly TcpClient client;
        readonly ServerInstance server;
        public int ParentNumber = 0;
        public List<int> ChildNumber;

        public ClientInstance(TcpClient tcpClient, ServerInstance serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);

            if (server.clients.Count > 1)
            {
                ParentNumber = ClassIO.GetNextRndNumber() % server.clients.Count;
                if (ParentNumber > 0)
                    server.clients[ParentNumber - 1].ChildNumber.Add(server.clients.FindIndex(x => x == this) + 1);
                else
                    server.ChildNumber.Add(server.clients.FindIndex(x => x == this) + 1);
            }
            else
            {
                server.ChildNumber.Add(server.clients.FindIndex(x => x == this) + 1);
            }
            ChildNumber = new List<int>();

        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                server.SendToSpecificClient($"NUMBER {server.clients.FindIndex(x => x == this) + 1}", server.clients.FindIndex(x => x == this));
                
                while (true)
                {
                    try
                    {
                        var message = GetMessage();
                        server.Parse(message, this);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
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

        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}