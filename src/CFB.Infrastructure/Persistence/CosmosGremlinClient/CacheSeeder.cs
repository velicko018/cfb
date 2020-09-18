using Gremlin.Net.Driver;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CFB.Infrastructure.Persistence.CosmosGremlinClient
{
    public class CacheSeeder
    {
        private readonly IGremlinClient _gremlinClient;
        private readonly IMemoryCache _memoryCache;

        public CacheSeeder(IGremlinClient gremlinClient, IMemoryCache memoryCache)
        {
            _gremlinClient = gremlinClient;
            _memoryCache = memoryCache;
        }

        public async Task InitData()
        {
            foreach (var field in Labels.GetProperties())
            {
                var count = await _gremlinClient.SubmitWithSingleResultAsync<long>($"g.V().HasLabel('{field.Value}').count()");

                _memoryCache.Set(field.Key, count);
            }
        }
    }

    public static class Labels
    {
        public static string Airports => "airport";
        public static string Flights => "flight";
        public static string Users => "user";
        public static string Bookings => "booking";
        public static Dictionary<string, string> GetProperties()
        {
            return typeof(Labels)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.PropertyType == typeof(string))
                .ToDictionary(f => f.Name, f => (string)f.GetValue(null));
        }

    }

    public static class CacheKeys
    {
        public static string Airports => "Airports";
        public static string Flights => "Flights";
        public static string Users => "Users";
        public static string Logs => "Logs";
        public static string Bookings => "Bookings";
    }
}
