using System;
// using Microsoft.Extensions.Logging;
using System.Text;
using NATS.Client;
using Valuator;

namespace RankCalculator
{
    class Program
    {
        // private IStorage _storage = new RedisStorage(new ILogger<RedisStorage>());

        static void Main(string[] args)
        {
            Console.WriteLine("Consumer started");

            IConnection c = new ConnectionFactory().CreateConnection();

            EventHandler<MsgHandlerEventArgs> handler = (sender, args) =>
            {
                string data = Encoding.UTF8.GetString(args.Message.Data); 
                Console.WriteLine("THIS IS DATA = " + data);               
            };

            IAsyncSubscription s = c.SubscribeAsync("valuator.processing.rank", "load-balancing-queue", handler);

            s.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            s.Unsubscribe();

            c.Drain();
            c.Close();
        }

        public double CalcRank(string text)
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
