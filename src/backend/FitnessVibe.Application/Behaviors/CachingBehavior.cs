using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FitnessVibe.Application.Behaviors
{
    /// <summary>
    /// Caching Behavior - the memory muscle that remembers your frequent workouts.
    /// This caches frequently requested data to improve performance,
    /// like how your body remembers movement patterns to perform them more efficiently.
    /// </summary>
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

        public CachingBehavior(
            IMemoryCache cache,
            ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            // Only cache queries, not commands (commands change state, queries read state)
            if (IsCommand(request))
            {
                return await next();
            }

            var cacheKey = GenerateCacheKey(request);
            
            if (_cache.TryGetValue(cacheKey, out TResponse cachedResponse))
            {
                _logger.LogDebug("Cache hit for {RequestName} with key {CacheKey}", 
                    typeof(TRequest).Name, cacheKey);
                return cachedResponse;
            }

            _logger.LogDebug("Cache miss for {RequestName} with key {CacheKey}", 
                typeof(TRequest).Name, cacheKey);

            var response = await next();

            // Cache the response with a reasonable expiration
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = GetCacheExpiration(request),
                Priority = CacheItemPriority.Normal
            };

            _cache.Set(cacheKey, response, cacheOptions);

            _logger.LogDebug("Cached response for {RequestName} with key {CacheKey}", 
                typeof(TRequest).Name, cacheKey);

            return response;
        }

        private static bool IsCommand(TRequest request)
        {
            // Commands typically have "Command" in their name, queries have "Query"
            var typeName = typeof(TRequest).Name;
            return typeName.Contains("Command") || typeName.EndsWith("Command");
        }

        private static string GenerateCacheKey(TRequest request)
        {
            var typeName = typeof(TRequest).Name;
            var requestJson = JsonSerializer.Serialize(request);
            var hash = requestJson.GetHashCode();
            return $"{typeName}_{hash}";
        }

        private static TimeSpan GetCacheExpiration(TRequest request)
        {
            var typeName = typeof(TRequest).Name;
            
            // Different cache durations based on data type - like how different workout data has different freshness needs
            return typeName switch
            {
                // User profile data changes infrequently
                var name when name.Contains("Profile") || name.Contains("User") => TimeSpan.FromMinutes(30),
                
                // Activity and workout data changes more frequently
                var name when name.Contains("Activity") || name.Contains("Workout") => TimeSpan.FromMinutes(5),
                
                // Real-time data like leaderboards should cache briefly
                var name when name.Contains("Leaderboard") || name.Contains("Live") => TimeSpan.FromMinutes(1),
                
                // Static reference data can cache longer
                var name when name.Contains("Template") || name.Contains("Type") => TimeSpan.FromHours(1),
                
                // Default cache duration
                _ => TimeSpan.FromMinutes(15)
            };
        }
    }
}
