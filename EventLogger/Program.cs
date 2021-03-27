using System;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
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
                Event info = JsonSerializer.Deserialize<Event>(args.Message.Data); 
                Console.WriteLine($"Event type: {info.type}; id: {info.id}; value: {info.value}");             
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
