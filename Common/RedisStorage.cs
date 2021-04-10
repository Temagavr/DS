using System;
using System.Collections.Generic;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;

namespace Common
{
    public class RedisStorage : IStorage
    {
        private readonly ILogger<RedisStorage> _logger;

        private IDatabase _db;
        private IConnectionMultiplexer _conn;
        private IConnectionMultiplexer _conn_Ru;
        private IConnectionMultiplexer _conn_Eu;
        private IConnectionMultiplexer _conn_Other;

        public RedisStorage(ILogger<RedisStorage> logger)
        {
            _logger = logger;
            _conn = ConnectionMultiplexer.Connect("localhost");
            _db = _conn.GetDatabase();

            _conn_Ru = 
                ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_" + Constants.RusShardId, EnvironmentVariableTarget.User));

            _conn_Eu = 
                ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_"  + Constants.EuShardId, EnvironmentVariableTarget.User));
            
            _conn_Other = 
                ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_"  + Constants.OtherShardId, EnvironmentVariableTarget.User));
        }

        public void Store(string key, string value, string region)
        {
            _logger.LogWarning($"LOOKUP: {key}, {region}");
            
            if(key == "" || value == "" || region == "")
                return;
            else
            {
                var db = _conn.GetDatabase();
                if(region == Constants.RusShardId)   
                    db = _conn_Ru.GetDatabase();
                else if(region == Constants.EuShardId)
                    db = _conn_Eu.GetDatabase();
                else if(region == Constants.OtherShardId)
                    db = _conn_Other.GetDatabase();
                
                db.StringSet(key, value);    
            }
        }

        public void StoreToMap(string key, string value)
        {
            _logger.LogWarning($"LOOKUP: {key}, Host");
            
            if(key == "" || value == "")
                return;

            _db.StringSet(key, value);
        }

        public string GetShardId(string key)
        {
            return _db.StringGet(key);
        }

        public string Load(string key, string region)
        {
            if(key == "" || region == "")
                return "";
            else
            {
                var db = _conn.GetDatabase();
                if(region == Constants.RusShardId)   
                    db = _conn_Ru.GetDatabase();
                else if(region == Constants.EuShardId)
                    db = _conn_Eu.GetDatabase();
                else if(region == Constants.OtherShardId)
                    db = _conn_Other.GetDatabase();
                
                return db.StringGet(key);    
            }
        }

        public List<string> GetAllTexts()
        {
            List<string> texts = new List<string>();
            List<IConnectionMultiplexer> connections = new List<IConnectionMultiplexer>()
            {
                _conn_Ru, _conn_Eu, _conn_Other 
            };

            foreach (var conn in connections)
            {
                var db = conn.GetDatabase();
                RedisResult[] textsKeys = (RedisResult[])db.Execute("keys", "TEXT-*");
                foreach (RedisResult key in textsKeys)
                {
                    texts.Add(Load(key.ToString(), GetShardId(key.ToString())));
                }

            }
            
            return texts;
        }

        public bool ExistKey(string key)
        {
            return (bool)_db.Execute("exists", key);
        }
    }
}