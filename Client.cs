using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab4
{
    public static class Client
    {
        public static string userName;
        static TcpClient client;
        static NetworkStream stream;
        public static int Number;

        public static void Init()
        {
            Console.Write("Enter name: ");
            userName = Console.ReadLine();
            client = new TcpClient();
            try
            {
                client.Connect(Program.IP, Program.port); //подключение клиента
                stream = client.GetStream(); // получаем поток

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                // запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); //старт потока
                Console.WriteLine("Welcome, {0}", userName);
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

        // отправка сообщений
        static void SendMessage()
        {
            Console.WriteLine("Enter message: ");

            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }
        // получение сообщений
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message); //вывод сообщения
                }
                catch
                {
                    Console.WriteLine("Connection aborted!"); //соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        public static void Parse(string command)
        {
            var parameters = command.Split(' ');
            switch(parameters[0])
            {
                case "NUMBER":
                    Number = Convert.ToInt32(parameters[1]);
                    break;
                case "PROTOCOL":
                    string msg = "";
                    for (int i = 1; i < parameters.Length; i++)
                        msg += parameters[i];
                    Protocol(msg);
                    break;
                case "GENERATE":
                    SendMessage();
                    break;
            }
        }

        public static void Protocol(string message)
        {
            //
        }

        static void Disconnect()
        {
            if (stream != null)
                stream.Close(); //отключение потока
            if (client != null)
                client.Close(); //отключение клиента

            Environment.Exit(0); //завершение процесса
        }
    }
}