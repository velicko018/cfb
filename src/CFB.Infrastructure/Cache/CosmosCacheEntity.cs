using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;

namespace CFB.Infrastructure.Cache
{
    public class CosmosCacheEntity : TableEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("content")]
        public byte[] Content { get; set; }

        [JsonProperty("slidingExpiration")]
        public TimeSpan? SlidingExpiration { get; set; }

        [JsonProperty("absoluteExpiration")]
        public DateTimeOffset? AbsoluteExpiration { get; set; }

        [JsonIgnore]
        public bool Expired => AbsoluteExpiration.HasValue && AbsoluteExpiration.Value < DateTimeOffset.UtcNow;
    }
}
