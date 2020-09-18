using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CFB.Infrastructure.Logging
{
    public class CosmosLoggingOptions : IOptions<CosmosLoggingOptions>
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public LogLevel LogLevel { get; set; } = LogLevel.Error;
        public CosmosLoggingOptions Value => this;
    }
}