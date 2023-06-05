using Microsoft.Extensions.Caching.Memory;
namespace QandA.Data
{
    public class QuestionCache : IQuestionCache
    {
        private MemoryCache _cache { get; set; }
        private string GetCacheKey(int questionId) => $"Question-{questionId}";
        // TODO - create a memory cache
        public QuestionCache()
        {
            // Notice that we have set the cache limit to be 100 items. This is to
            // limit the amount of memory the cache takes up on our web server.
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 100
            });
        }

        // TODO - method to get a cached question
        public QuestionGetSingleResponse Get(int questionId)
        {
            QuestionGetSingleResponse question;
            _cache.TryGetValue(GetCacheKey(questionId), out question);
            return question;
        }

        // TODO - method to add a cached question
        public void Set(QuestionGetSingleResponse question)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1);
            _cache.Set(GetCacheKey(question.QuestionId), question, cacheEntryOptions);

        }

        // TODO - method to remove a cached question
        public void Remove(int questionId)
        {
            _cache.Remove(GetCacheKey(questionId));
        }


    }
}
