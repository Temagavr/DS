using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using NATS.Client;

namespace Common
{
    public class NatsMessageBroker : IMessageBroker
    {
        public void SendMsg(string title, string value)
        {   
            IConnection connection = new ConnectionFactory().CreateConnection();
            
            byte[] data = Encoding.UTF8.GetBytes(value);
            connection.Publish(title, data);

            connection.Drain();

            connection.Close();
        }

        public void SendMsgToLogger(Event info)
        {
            string json = JsonSerializer.Serialize(info);

            SendMsg(info.type, json);
        }
    }
}