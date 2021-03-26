using System;
using Microsoft.Extensions.Logging;
using System.Text;
using NATS.Client;
using Common;

namespace EventLogger
{
    class Program
    {
        static void Main(string[] args)
        {
        
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = new Logger<RedisStorage>(loggerFactory);

            IStorage storage = new RedisStorage(logger);

            Console.WriteLine("EventLogger started");

            IConnection connection = new ConnectionFactory().CreateConnection();

            EventHandler<MsgHandlerEventArgs> handler = (sender, args) =>
            {
                string data = Encoding.UTF8.GetString(args.Message.Data); 
                Console.WriteLine("THIS IS DATA = " + data);             
            };

            IAsyncSubscription subscription;
            subscription = connection.SubscribeAsync("RankCalculated", handler);
            subscription = connection.SubscribeAsync("SimilarityCalculated", handler);

            subscription.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            subscription.Unsubscribe();

            connection.Drain();
            connection.Close();
        }
    }
}
