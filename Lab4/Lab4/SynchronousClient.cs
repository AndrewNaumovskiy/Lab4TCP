using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lab4
{
    public class SynchronousSocketClient
    {

        public static void StartClient()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                IPAddress ipAddress = IPAddress.Parse(Program.IP);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Program.port);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());
                    while (true)
                    {
                        var kek = Console.ReadLine();

                        if (kek == "exit")
                        {
                            sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                        }

                        // Encode the data string into a byte array.  
                        byte[] msg = Encoding.ASCII.GetBytes(kek + "<EOF>");

                        // Send the data through the socket.  
                        int bytesSent = sender.Send(msg);

                        // Receive the response from the remote device.  
                        int bytesRec = sender.Receive(bytes);
                        Console.WriteLine("Received = {0}",
                            Encoding.ASCII.GetString(bytes, 0, bytesRec));
                    }
                    

                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static int Init()
        {
            StartClient();
            return 0;
        }
    }
}
