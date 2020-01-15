using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Lab4
{
    class Program
    {
        public const string IP = "127.0.0.1";
        public const int port = 8888;
        public const string filePath = "log.txt";
        static void Main()
        {

            int cho = Convert.ToInt32(Console.ReadLine());

            Logger.Log(filePath,cho.ToString());


            // ya lublu minecraft
            //if (cho == 1)
            //{
            //    Server.Init();
            //}
            //else
            //{
            //    Client.Init();
            //}

            Console.ReadKey();
        }
    }
}