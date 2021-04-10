
using System.Collections.Generic;

namespace Common
{
    public interface IStorage
    {
        void Store(string key, string value, string region);

        void StoreToMap(string key, string value);

        string GetShardId(string key);
        
        string Load(string key, string region);
        
        List<string> GetAllTexts();

        bool ExistKey(string key);
    }
}