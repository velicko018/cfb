using CFB.Common.Utilities;
using CFB.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace CFB.Infrastructure.Identity.Stores
{
    public class CosmosUserStore<TUser, TRole> : IUserPasswordStore<TUser>,
        IUserEmailStore<TUser>,
        IUserRoleStore<TUser>
        where TUser : CosmosIdentityUser, new()
        where TRole : CosmosIdentityRole, new()
    {
        private readonly Container _usersContainer;
        private readonly IRoleStore<TRole> _roleStore;

        public CosmosUserStore(CosmosClient cosmosClient, IRoleStore<TRole> roleStore)
        {
            _roleStore = roleStore;
            _usersContainer = cosmosClient.GetContainer("identity", "users");
        }

        #region IUserStore
        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            using (var stream = StreamHelper.ToStream(user))
            {
                using (ResponseMessage responseMessage = await _usersContainer.CreateItemStreamAsync(stream, PartitionKey.None))
                {
                    return (!responseMessage.IsSuccessStatusCode)
                        ? CosmosIdentityResult.Failed(responseMessage)
                        : CosmosIdentityResult.Success;
                }
            }
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            using (var responseMessage = await _usersContainer.DeleteItemStreamAsync(user.Id, PartitionKey.None))
            {
                return (!responseMessage.IsSuccessStatusCode)
                    ? CosmosIdentityResult.Failed(responseMessage)
                    : CosmosIdentityResult.Success;
            }            
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            using (var stream = StreamHelper.ToStream(user))
            {
                using (var responseMessage = await _usersContainer.UpsertItemStreamAsync(stream, PartitionKey.None))
                {
                    return (!responseMessage.IsSuccessStatusCode)
                        ? CosmosIdentityResult.Failed(responseMessage)
                        : CosmosIdentityResult.Success;
                }
            }
        }

        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            using (var responseMessage = await _usersContainer.ReadItemStreamAsync(userId, PartitionKey.None))
            {
                if (!responseMessage.IsSuccessStatusCode)
                {
                    return null;
                }

                var user = StreamHelper.FromStream<TUser>(responseMessage.Content);

                return user.Id != null
                    ? user
                    : null;
            }
        }

        public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(normalizedUserName))
            {
                throw new ArgumentNullException(nameof(normalizedUserName));
            }

            var query = new QueryDefinition("select * from users u where u.normalizedUserName = @NormalizedUserName")
                .WithParameter("@NormalizedUserName", normalizedUserName);
            var feedIterator = _usersContainer.GetItemQueryStreamIterator(query,
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
                    var userList = cosmosResponse.Documents.ToObject<List<TUser>>();

                    return userList.Count > 0
                        ? userList[0]
                        : null;
                }
            }

            return null;
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }

            user.NormalizedUserName = normalizedName.ToUpperInvariant();

            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            user.UserName = userName;

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
        #endregion

        #region IUserPasswordStore
        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentNullException(nameof(passwordHash));
            }

            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }
        #endregion

        #region IUserEmailStore
        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("message", nameof(email));
            }

            user.Email = email;

            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.EmailConfirmed = true;

            return Task.CompletedTask;
        }

        public async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(normalizedEmail))
            {
                throw new ArgumentNullException(nameof(normalizedEmail));
            }

            var queryDefinition = new QueryDefinition("select * from users u where u.normalizedEmail = @NormalizedEmail")
                .WithParameter("@NormalizedEmail", normalizedEmail);
            
            var feedIterator = _usersContainer.GetItemQueryStreamIterator(queryDefinition,
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

                    var userList = cosmosResponse.Documents.ToObject<List<TUser>>();

                    return userList.Count > 0 
                        ? userList[0]
                        : null;
                }
            }

            return null;            
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.NormalizedEmail = normalizedEmail;

            return Task.CompletedTask;
        }
        #endregion

        #region IUserRoleStore
        public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var role = await _roleStore.FindByNameAsync(roleName, cancellationToken);

            if (role != null)
            {
                user.Roles.Add(role);
            }
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var role = user.Roles.FirstOrDefault(r => r.Name == roleName);

            if (role != null)
            {
                user.Roles.Remove(role);
            }

            return Task.CompletedTask;
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var roles = user.Roles
                .Select(r => r.Name)
                .ToList();

            return Task.FromResult<IList<string>>(roles);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            return Task.FromResult(user?.Roles.Any(r => r.Name == roleName) ?? false);
        }

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var usersInRole = new List<TUser>();
            var query = new QueryDefinition("select * from users u join r in u.roles r r.normalizedName = @NormalizedRoleName")
                .WithParameter("@NormalizedRoleName", roleName);
            var feedIterator = _usersContainer.GetItemQueryStreamIterator(query,
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = PartitionKey.None,
                    MaxConcurrency = 1
                });


            while (feedIterator.HasMoreResults)
            {
                using (var responseMessage = await feedIterator.ReadNextAsync())
                {
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        usersInRole.Add(StreamHelper.FromStream<TUser>(responseMessage.Content));
                    }
                }
            }

            return await Task.FromResult<IList<TUser>>(usersInRole);
        }
        #endregion
    }
}
