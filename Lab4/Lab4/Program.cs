using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Lab4
{
    class Program
    {
        public const string IP = "127.0.0.1";
        public const int port = 8888;
        public const string LogFilePath = "log.txt";
        public const string RandomFilePath = "random.txt";

        static void Main()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            TimeSpan timer;
            while (true)
            {
                timer = stopwatch.Elapsed;
                if (timer.Seconds < 5 && Client.CanConnect())
                {
                    Client.Init();
                }
                else
                {
                    Server.Init();
                }
            }
        }
    }
}