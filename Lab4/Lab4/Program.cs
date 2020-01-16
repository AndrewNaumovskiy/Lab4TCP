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
        public const string RandomFilePath = "random.txt";
        public const string LogFilePath = "log.txt";
        
        static void Main()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            TimeSpan timer;
            while (true)
            {
                timer = watch.Elapsed;
                if (timer.Milliseconds < 5000 && Client.Check())
                {
                    Client.Start();
                }
                else
                {
                    Server.Start();
                }
            }
        }
    }
}