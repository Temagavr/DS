using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

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

            string textKey = "TEXT-" + id;
            //TODO: сохранить в БД text по ключу textKey
            _storage.Store(textKey, text);

            string rankKey = "RANK-" + id;
            //TODO: посчитать rank и сохранить в БД по ключу rankKey
            string rank = CalcRank(text).ToString();
            _storage.Store(rankKey, rank);

            string similarityKey = "SIMILARITY-" + id;
            //TODO: посчитать similarity и сохранить в БД по ключу similarityKey
            _storage.Store(similarityKey, similarity);            

            return Redirect($"summary?id={id}");
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
