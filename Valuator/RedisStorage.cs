using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace Valuator
{
    public class RedisStorage : IStorage
    {
        private readonly ILogger<RedisStorage> _logger;

        private IDatabase _db;

        public RedisStorage(ILogger<RedisStorage> logger)
        {
            _logger = logger;
            IConnectionMultiplexer connectionMultiplexer = 
                ConnectionMultiplexer.Connect("localhost");
            _db = connectionMultiplexer.GetDatabase();
        }

        public void Store(string key, string value)
        {
            _logger.LogWarning("{0}, {1}", key, value);
            _db.StringSet(key, value);
        }

        public string Load(string key)
        {
            _logger.LogWarning(key);
            return _db.StringGet(key);
        }

        public List<string> GetAllTexts()
        {
            RedisResult[] textsKeys = (RedisResult[])_db.Execute("keys", "TEXT-*");
            List<string> texts = new List<string>();
            foreach (RedisResult key in textsKeys)
            {
                texts.Add(Load(key.ToString()));
            }
            return texts;
        }
    }
}