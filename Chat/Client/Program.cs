using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client
{
    class Program
    {
        public static void StartClient(string host, int port, string message)
        {
            try
            {
                // Разрешение сетевых имён
                IPAddress ipAddress;

                if(host == "localhost")
                    ipAddress = IPAddress.Loopback;
                else
                    ipAddress = IPAddress.Parse(host);
                

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // CREATE
                Socket sender = new Socket(
                    ipAddress.AddressFamily,
                    SocketType.Stream, 
                    ProtocolType.Tcp);

                try
                {
                    // CONNECT
                    sender.Connect(remoteEP);

                    // Подготовка данных к отправке
                    message += "<EOF>";
                    byte[] msg = Encoding.UTF8.GetBytes(message);

                    // SEND
                    int bytesSent = sender.Send(msg);

                    // RECEIVE
                    byte[] buf = new byte[sizeof(int)];
                    int bytesRec = sender.Receive(buf);
                    MemoryStream ms = new MemoryStream();
                    
                    while(bytesRec > 0)
                    {
                        ms.Write(buf, 0, bytesRec);

                        if(sender.Available > 0)
                            bytesRec = sender.Receive(buf);
                        else    
                            bytesRec = 0;
                    }
                    ms.Flush();
                    ms.Position = 0;

                    string receivedData = "";
                    using (StreamReader reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        receivedData = reader.ReadToEnd();
                    }
                    
                    List<string> history = JsonSerializer.Deserialize<List<string>>(receivedData);   

                    foreach(var str in history)
                    {
                        Console.WriteLine(str);
                    }

                    // RELEASE
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

        static void Main(string[] args)
        {
            if(args.Length != 3)
                Console.WriteLine("Invalid argument count");
            else
                StartClient(args[0], Convert.ToInt32(args[1]), args[2]);
        }
    }
}