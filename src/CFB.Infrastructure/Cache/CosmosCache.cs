using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace CFB.Infrastructure.Cache
{
    public class CosmosCache : IDistributedCache
    {
        private readonly CloudTable _table;

        public CosmosCache(IOptions<CosmosCacheOptions> options)
        {
            var cosmosCacheOptions = options.Value;
            var tableClient = new CloudTableClient(new Uri(cosmosCacheOptions.StorageUri), 
                                                new StorageCredentials(cosmosCacheOptions.AccountName, cosmosCacheOptions.AccountKey));            
            _table = tableClient.GetTableReference(cosmosCacheOptions.TableName);

            _table.CreateIfNotExistsAsync()
                .GetAwaiter()
                .GetResult();
        }
        public byte[] Get(string key)
        {
            return GetAsync(key)
                .GetAwaiter()
                .GetResult();
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            token.ThrowIfCancellationRequested();

            var tableResult = await _table.ExecuteAsync(TableOperation.Retrieve<CosmosCacheEntity>(key, key));
            var cosmosResult = tableResult.Result as CosmosCacheEntity;

            if (cosmosResult != null && cosmosResult.Expired)
            {
                await _table.ExecuteAsync(TableOperation.Delete(cosmosResult));

                return null;
            }

            return cosmosResult?.Content;
        }

        public void Refresh(string key)
        {
            Get(key);
        }

        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            await GetAsync(key, token);
        }

        public void Remove(string key)
        {
            RemoveAsync(key)
                .GetAwaiter()
                .GetResult();
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var tableResult = await _table.ExecuteAsync(TableOperation.Retrieve<CosmosCacheEntity>(key, key));

            if (tableResult.Result is CosmosCacheEntity cosmosResult)
            {
                await _table.ExecuteAsync(TableOperation.Delete(cosmosResult));
            }
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            SetAsync(key, value, options)
                .GetAwaiter()
                .GetResult();
        }

        public async Task SetAsync(string key,
                                   byte[] value,
                                   DistributedCacheEntryOptions options,
                                   CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (options.AbsoluteExpiration.HasValue && options.AbsoluteExpiration.Value < DateTimeOffset.UtcNow)
            {
                return;
            }

            var cosmosCacheEntity = new CosmosCacheEntity
            {
                PartitionKey = key,
                RowKey = key,
                Content = value,
                AbsoluteExpiration = options.AbsoluteExpiration
            };

            await _table.ExecuteAsync(TableOperation.InsertOrMerge(cosmosCacheEntity));
        }
    }
}
