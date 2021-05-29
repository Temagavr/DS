using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;

namespace Chain
{
    class Program
    {
        private static Socket _sender;
        private static Socket _listener;
        
        private static int _x;
        private static bool _initFlag = false;
        
        private static void InitiatorWork()
        {
            var bytes = BitConverter.GetBytes(_x);
            _sender.Send(bytes);

            Socket handler = _listener.Accept();

            //RECEIVE MAX 
            byte[] buf = new byte[4];
            int bytesRec = handler.Receive(buf);
            int y = BitConverter.ToInt32(buf);

            _x = y;
            Console.WriteLine(_x);

            //SEND MAX
            bytes = BitConverter.GetBytes(Math.Max(_x, y));
            int bytesSent = _sender.Send(bytes);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        private static void BasicWork()
        {
            Socket handler = _listener.Accept();

            //RECEIVE FROM PREV
            byte[] buf = new byte[4];
            int bytesRec = handler.Receive(buf);
            int y = BitConverter.ToInt32(buf);

            //SEND TO NEXT
            var bytes = BitConverter.GetBytes(Math.Max(_x, y));
            int bytesSent = _sender.Send(bytes);

            //RECEIVE MAX 
            buf = new byte[4];
            bytesRec = handler.Receive(buf);
            int receivedNumber = BitConverter.ToInt32(buf);
            Console.WriteLine(receivedNumber);

            //SEND MAX  
            _sender.Send(buf);
            
            //RELEASE
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
        
        private static void CreateConnection(int listenPort, string address, int port)
        {
            IPAddress listenIpAddress = IPAddress.Any;
            IPEndPoint localEndPoint = new IPEndPoint(listenIpAddress, listenPort);
            _listener = new Socket(
                 listenIpAddress.AddressFamily,
                 SocketType.Stream,
                 ProtocolType.Tcp);

            _listener.Bind(localEndPoint);
            _listener.Listen(10);
            
            IPAddress ipAddress;
            if(address == "localhost")
                ipAddress = IPAddress.Loopback;
            else
                ipAddress = IPAddress.Parse(address);
            
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            _sender = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            bool connectFlag = false;
            for (int i = 0; i < 5 && !connectFlag; ++i)
            {
                try
                {
                    _sender.Connect(remoteEP);
                    connectFlag = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception : {0}", e.ToString());
                }
            }
            if (!connectFlag)
                Console.WriteLine("Cannot connect to next process");
        }

        static void Main(string[] args)
        {
            var listenPort = Convert.ToInt32(args[0]);
            var address = args[1];
            var port = Convert.ToInt32(args[2]);
            if(args.Length == 4 && args[3] == "true")    
                _initFlag = true;

            CreateConnection(listenPort, address, port);

            _x = Convert.ToInt32(Console.ReadLine());

            if (_initFlag)
            {
                InitiatorWork();
            }
            else
            {
                BasicWork();
            }

            _sender.Shutdown(SocketShutdown.Both);
            _sender.Close();

        }
    }
}
