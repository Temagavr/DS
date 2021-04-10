using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Common;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private IStorage _storage;

        private IMessageBroker _broker;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage, IMessageBroker broker)
        {
            _logger = logger;
            _storage = storage;
            _broker = broker;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string text, string region)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();
            
            _storage.StoreToMap(id, region);

            string similarity = CheckSimilarity(text).ToString();

            Event similarityCalculatedEvent = new Event("SimilarityCalculated", id, similarity);
            _broker.SendMsgToLogger(similarityCalculatedEvent);

            string textKey = Constants.textPrefix + id;
            _storage.Store(textKey, text, region);

            _broker.SendMsg("valuator.processing.rank", id);

            string similarityKey = Constants.similarityPrefix + id;
            _storage.Store(similarityKey, similarity, region);            

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
    }
}
