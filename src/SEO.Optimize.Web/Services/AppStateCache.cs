using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SEO.Optimize.Web.Services
{
    public class AppStateCache
    {
        private readonly IDistributedCache distributedCache;

        public AppStateCache(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public async Task SetAsync(string key, Dictionary<string, string> stateValues)
        {
            var values = JsonConvert.SerializeObject(stateValues);
            await distributedCache.SetStringAsync(key, values);
        }

        public async Task UpdateAsync(string key, Dictionary<string, string> stateValues)
        {
            var values = await distributedCache.GetStringAsync(key);
            if (string.IsNullOrWhiteSpace(values))
            {
                throw new KeyNotFoundException();
            }

            var valueDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(values);
            foreach(var pair in stateValues)
            {
                valueDict[pair.Key] = pair.Value;
            }

            await SetAsync(key, valueDict);
        }

        public async Task<Dictionary<string,string>> GetAsync(string key)
        {
            var values = await distributedCache.GetStringAsync(key);
            if (string.IsNullOrWhiteSpace(values))
            {
                throw new KeyNotFoundException();
            }

            var valueDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(values);
            return valueDict;
        }

        public async Task ClearCache(string key)
        {
            await distributedCache.RemoveAsync(key);
        }
    }
}
