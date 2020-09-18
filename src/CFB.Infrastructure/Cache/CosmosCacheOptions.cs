using Microsoft.Extensions.Options;

namespace CFB.Infrastructure.Cache
{
    public class CosmosCacheOptions : IOptions<CosmosCacheOptions>
    {
        public string StorageUri { get; set; }
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string TableName { get; set; }
        public CosmosCacheOptions Value => this;
    }
}