using CFB.Common.Utilities;
using CFB.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CFB.Infrastructure.Identity.Stores
{
    public class CosmosRoleStore<TRole> : IRoleStore<TRole>
        where TRole : CosmosIdentityRole, new()
    {
        private readonly Container _rolesContainer;

        public CosmosRoleStore(CosmosClient cosmosClient)
        {
            _rolesContainer = cosmosClient.GetContainer("identity", "roles");
        }

        #region IRoleStore implementation
        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            using (var stream = StreamHelper.ToStream(role))
            {
                using (var responseMessage = await _rolesContainer.CreateItemStreamAsync(stream, PartitionKey.None))
                {
                    return (!responseMessage.IsSuccessStatusCode)
                        ? CosmosIdentityResult.Failed(responseMessage)
                        : CosmosIdentityResult.Success;
                }
            }
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            using (var stream = StreamHelper.ToStream(role))
            {
                using (var responseMessage = await _rolesContainer.UpsertItemStreamAsync(stream, PartitionKey.None))
                {
                    return (!responseMessage.IsSuccessStatusCode)
                        ? CosmosIdentityResult.Failed(responseMessage)
                        : CosmosIdentityResult.Success;
                }
            }
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            using (var responseMessage = await _rolesContainer.DeleteItemStreamAsync(role.Id, PartitionKey.None))
            {
                return (!responseMessage.IsSuccessStatusCode)
                    ? CosmosIdentityResult.Failed(responseMessage)
                    : CosmosIdentityResult.Success;
            }            
        }

        public void Dispose()
        {
        }

        public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentNullException(nameof(roleId));
            }

            using (var responseMessage = await _rolesContainer.ReadItemStreamAsync(roleId, PartitionKey.None))
            {
                if (!responseMessage.IsSuccessStatusCode)
                {
                    return null;
                }

                return StreamHelper.FromStream<TRole>(responseMessage.Content);
            }
        }

        public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }

            var query = new QueryDefinition("select * from roles r where r.normalizedRoleName = @NormalizedRoleName")
                .WithParameter("@NormalizedRoleName", normalizedRoleName);
            var feedIterator = _rolesContainer.GetItemQueryStreamIterator(query,
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = PartitionKey.None,
                    MaxItemCount = 1,
                    MaxConcurrency = 1
                });

            if (feedIterator.HasMoreResults)
            {
                using (var responseMessage = await feedIterator.ReadNextAsync())
                {
                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var cosmosResponse = StreamHelper.FromStream<dynamic>(responseMessage.Content);
                    var roleList = cosmosResponse.Documents.ToObject<IList<TRole>>();

                    return roleList.Count > 0
                        ? roleList[0]
                        : null;
                }
            }

            return null;
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }

            role.NormalizedName = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            role.Name = roleName;

            return Task.CompletedTask;
        }
        #endregion
    }
}
