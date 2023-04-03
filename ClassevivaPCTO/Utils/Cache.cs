using Microsoft.Extensions.Caching.Memory;

namespace ClassevivaPCTO.Utils
{
    internal class Cache
    {
        private const string KEY = "login_cache";
        private readonly IMemoryCache memoryCache;

        public Cache(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;

        }


        public void AddToCache(LoginResult loginResult)
        {
            var option = new MemoryCacheEntryOptions
            {
                //SlidingExpiration = TimeSpan.FromSeconds(1),
                AbsoluteExpiration = loginResult.Expire
            };

            memoryCache.Set<LoginResult>(KEY, loginResult, option);
        }

        public LoginResult GetCachedLoginResult()
        {
            LoginResult loginResult = null;
            memoryCache.TryGetValue(KEY, out loginResult);

            return loginResult;
        }
    }
}
