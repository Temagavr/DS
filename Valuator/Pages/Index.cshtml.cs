﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NATS.Client;
using Common;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private IStorage _storage;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();

            string similarity = CheckSimilarity(text).ToString();

            SendNatsMsg("SimilarityCalculated", id);

            string textKey = Constants.textPrefix + id;
            _storage.Store(textKey, text);

            SendNatsMsg("valuator.processing.rank", id);

            string similarityKey = Constants.similarityPrefix + id;
            _storage.Store(similarityKey, similarity);            

            return Redirect($"summary?id={id}");
        }

        public int CheckSimilarity(string text)
        {
            List<string> texts = _storage.GetAllTexts(); 
            int similarity = 0;
            
            foreach (string storedText in texts)
            {
                if(text == storedText)
                {
                    similarity = 1;
                    break;
                }
            }

            return similarity;
        }

        public void SendNatsMsg(string title, string value)
        {
            IConnection connection = new ConnectionFactory().CreateConnection();
            
            byte[] data = Encoding.UTF8.GetBytes(value);
            connection.Publish(title, data);

            connection.Drain();

            connection.Close();
            
            // CancellationTokenSource cts = new CancellationTokenSource();

            // Task.Factory.StartNew(() => ProduceAsync(cts.Token, id), cts.Token);

            // cts.Cancel();
        }

        // Не получается сделать через этот метод, пока не знаю почему
        static async Task ProduceAsync(CancellationToken ct, string id) 
        {
            using (IConnection connection = new ConnectionFactory().CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(id);
                connection.Publish("valuator.processing.rank", data);

                await Task.Delay(1000);
                
                connection.Drain();

                connection.Close();
            }
        }
    }
}
