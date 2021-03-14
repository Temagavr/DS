using System;
using Microsoft.Extensions.Logging;
using System.Text;
using NATS.Client;
using Valuator;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = new Logger<RedisStorage>(loggerFactory);

            IStorage storage = new RedisStorage(logger);

            Console.WriteLine("Consumer started");

            IConnection connection = new ConnectionFactory().CreateConnection();

            EventHandler<MsgHandlerEventArgs> handler = (sender, args) =>
            {
                string data = Encoding.UTF8.GetString(args.Message.Data); 
                // Console.WriteLine("THIS IS DATA = " + data);             
                string text = storage.Load(Constants.textPrefix + data);
                storage.Store(Constants.rankPrefix + data, CalcRank(ref text).ToString()); 
            };

            IAsyncSubscription subscription = connection.SubscribeAsync("valuator.processing.rank", "load-balancing-queue", handler);

            subscription.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            subscription.Unsubscribe();

            connection.Drain();
            connection.Close();
        }

        static double CalcRank(ref string text)
        {
            double length = text.Length, notCharsCount = 0; 
            for(int i = 0; i != length; ++i)
            {
                if(!Char.IsLetter(text[i]))
                    ++notCharsCount;
            }
            return Math.Round(notCharsCount / length, 2);
        }
    }
}
