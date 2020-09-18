using CFB.Application.Dispatchers;
using CFB.Application.Models;
using CFB.Common.DTOs;
using CFB.Domain.Commands;
using CFB.Domain.Queries;
using CFB.Infrastructure.Identity.Models;
using CFB.Infrastructure.Persistence.CosmosGremlinClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CFB.Application.Services
{
    public interface IUserService
    {
        Task<UsersDto> GetUsersAsync(PaginationParameters paginationParameters);
        Task<UserDto> GetUserAsync(Guid userId);
        Task<Result> DeleteUserAsync(Guid userId);
        Task<Result> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto);
    }

    public class UserService : IUserService
    {
        private readonly CommandDispatcher _commandDispatcher;
        private readonly QueryDispatcher _queryDispatcher;
        private readonly IMemoryCache _memoryCache;
        private readonly UserManager<CosmosIdentityUser> _userManager;

        public UserService(QueryDispatcher queryDispatcher, 
            CommandDispatcher commandDispatcher, UserManager<CosmosIdentityUser> userManager, IMemoryCache memoryCache)
        {
            _commandDispatcher = commandDispatcher;
            _userManager = userManager;
            _queryDispatcher = queryDispatcher;
            _memoryCache = memoryCache;
        }

        public async Task<Result> DeleteUserAsync(Guid userId)
        {
            var cosmosIdentityUser = await _userManager.FindByIdAsync(userId.ToString());

            if (cosmosIdentityUser != null)
            {
                var identityResult = await _userManager.DeleteAsync(cosmosIdentityUser);

                if (identityResult.Succeeded)
                {
                    var deleteUserCommand = new DeleteUserCommand
                    {
                        UserId = userId
                    };

                    var result = await _commandDispatcher.Dispatch(deleteUserCommand);

                    if (result.IsSuccess)
                    {
                        _memoryCache.TryGetValue(CacheKeys.Users, out long numberOfUsers);
                        _memoryCache.Set(CacheKeys.Users, --numberOfUsers);
                    }

                    return result;
                }
            }

            return Result.Failure;
        }

        public async Task<UsersDto> GetUsersAsync(PaginationParameters paginationParameters)
        {
            var getUsersQuery = new GetUsersQuery
            {
                RangeFrom = paginationParameters.PageIndex * paginationParameters.PageSize,
                RangeTo = paginationParameters.PageIndex * paginationParameters.PageSize + paginationParameters.PageSize
            };

            var users = await _queryDispatcher.Dispatch(getUsersQuery);

            if (!users.Any())
            {
                return null;
            }

            _memoryCache.TryGetValue(CacheKeys.Users, out long total);

            return new UsersDto
            {
                Users = users,
                Total = total
            };
        }

        public async Task<UserDto> GetUserAsync(Guid userId)
        {
            var getUserQuery = new GetUserQuery
            {
                UserId = userId
            };

            var user = await _queryDispatcher.Dispatch(getUserQuery);

            return user;
        }

        public async Task<Result> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            var updateUserCommand = new UpdateUserCommand
            {
                Id = userId.ToString(),
                Email = updateUserDto.Email,
                FirstName = updateUserDto.FirstName,
                LastName = updateUserDto.LastName,
                Address = updateUserDto.Address,
                City = updateUserDto.City,
                Zip = updateUserDto.Zip
            };

            var result = await _commandDispatcher.Dispatch(updateUserCommand);

            return result;
        }
    }
}
