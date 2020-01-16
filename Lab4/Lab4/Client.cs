using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab4
{
    public static class Client
    {
        public static int Number;
        static TcpClient client;
        static NetworkStream stream;
        public static bool kek = false;
        public static bool CanConnect()
        {
            try
            {
                client = new TcpClient();
                client.Connect(Program.IP, Program.port);
                kek = client.Connected;
            }
            catch (Exception e)
            {
                kek = false;
            }
            finally
            {
                client.Close();
            }
            return kek;
        }

        public static void Init()
        {
            client = new TcpClient();
            try
            {
                client.Connect(Program.IP, Program.port);
                stream = client.GetStream();

                // thream for receiving data
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        // sending data
        static void SendMessage()
        {
            Console.WriteLine("Enter message: ");

            while (true)
            {
                var messageFromConsole = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(messageFromConsole);
                stream.Write(data, 0, data.Length);
            }
        }

        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (stream.DataAvailable);

                    string message = builder.ToString();

                    Console.WriteLine(message);
                    
                    Parse(message);
                }
                catch
                {
                    Console.WriteLine("Connection aborted!");
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        public static void Parse(string command)
        {
            var parameters = command.Split(' ');
            switch (parameters[0])
            {
                case "NUMBER":
                    Number = Convert.ToInt32(parameters[1]);
                    break;
                case "PROTOCOL":
                    MakeLog(command.Replace("PROTOCOL ",""));
                    break;
                case "GENERATE":
                    string kek = FibonachiGenerator.Generate(Convert.ToInt32(parameters[1])).ToString();
                    break;
            }
        }

        public static void MakeLog(string message)
        {
            var FileName = Program.LogFilePath.Replace(".txt", $"_{Number}.txt");

            FileHelper.Log(FileName, message);
        }

        static void Disconnect()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();

            Environment.Exit(0);
        }
    }
}