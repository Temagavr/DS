using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Common;

namespace Valuator.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly ILogger<SummaryModel> _logger;
        
        private IStorage _storage;

        public SummaryModel(ILogger<SummaryModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;     
        }

        public double Rank { get; set; }
        public double Similarity { get; set; }

        public void OnGet(string id)
        {
            _logger.LogDebug(id);
            
            for(int check = 0; check < 20; ++check)
            {
                if(_storage.ExistKey(Constants.rankPrefix + id))
                {
                    Rank = Convert.ToDouble(_storage.Load(Constants.rankPrefix + id));
                    break;
                }
            }

            Similarity = Convert.ToDouble(_storage.Load(Constants.similarityPrefix + id));
        }
    }
}
