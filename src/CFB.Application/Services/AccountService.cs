using CFB.Application.Dispatchers;
using CFB.Application.Models;
using CFB.Common.DTOs;
using CFB.Domain.Commands;
using CFB.Infrastructure.Identity.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CFB.Application.Services
{
    public interface IAccountService
    {
        Task<UserDto> GetUserByNameAsync(string userName);
        Task<UserDto> RegisterAsync(UserForCreationDto userForCreationDto);
        Task<TokenDto> LoginAsync(LoginDto loginDto);
        Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto);
    }

    public class AccountService : IAccountService
    {
        private readonly UserManager<CosmosIdentityUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger _logger;
        private readonly CommandDispatcher _commandDispatcher;

        public AccountService(
            UserManager<CosmosIdentityUser> userManager,
            CommandDispatcher commandDispatcher,
            ITokenService tokenService,
            IOptions<JwtOptions> jwtOptions,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _jwtOptions = jwtOptions.Value;
            _logger = loggerFactory.CreateLogger(nameof(AccountService));
            _commandDispatcher = commandDispatcher;
        }

        public async Task<UserDto> GetUserByNameAsync(string userName)
        {
            var identityUser = await _userManager.FindByNameAsync(userName);

            if (identityUser is null)
            {
                _logger.LogError("User does not exist.");

                return null;
            }

            return new UserDto
            {
                Email = identityUser.Email,
                UserName = identityUser.UserName,
                Id = identityUser.Id
            };
        }

        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            var identityUser = await _userManager.FindByEmailAsync(loginDto.Email);

            if (identityUser is null)
            {
                _logger.LogError("User does not exist.");

                return null;
            }

            if (await _userManager.CheckPasswordAsync(identityUser, loginDto.Password))
            {
                var refreshToken = _tokenService.GenerateRefreshToken();
                var accessToken = _tokenService.GenerateAccessToken(identityUser.UserName, identityUser.Id, identityUser.Roles.First().Name);

                identityUser.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(identityUser);

                return new TokenDto(accessToken, refreshToken, _jwtOptions.Expiration);
            }

            _logger.LogError("Password is not valid for the specified user.");

            return null;
        }

        public async Task<UserDto> RegisterAsync(UserForCreationDto userForCreationDto)
        {
            var identityUser = await _userManager.FindByEmailAsync(userForCreationDto.Email);

            if (identityUser != null)
            {
                _logger.LogError("User already exists.");

                return null;
            }
            var cosmosIdentityUser = new CosmosIdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = userForCreationDto.Email,
                UserName = userForCreationDto.UserName
            };

            var identityResult = await _userManager.CreateAsync(cosmosIdentityUser, userForCreationDto.Password);

            if (!identityResult.Succeeded)
            {
                _logger.LogError(identityResult.Errors.First().Description);

                return null;
            }

            await _userManager.AddToRoleAsync(cosmosIdentityUser, "User");

            var createUserCommand = new CreateUserCommand
            {
                Id = cosmosIdentityUser.Id,
                Email = userForCreationDto.Email,
                UserName = userForCreationDto.UserName,
                FirstName = userForCreationDto.FirstName,
                LastName = userForCreationDto.LastName,
                Address = userForCreationDto.Address,
                City = userForCreationDto.City,
                Zip = userForCreationDto.Zip
            };

            var result = await _commandDispatcher.Dispatch(createUserCommand);

            if (result.IsSuccess)
            {
                return new UserDto
                {
                    Email = userForCreationDto.Email,
                    UserName = userForCreationDto.UserName
                };
            }

            await _userManager.DeleteAsync(cosmosIdentityUser);
            _logger.LogError("Unable to create a user.");

            return null;


        }

        public async Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto)
        {
            var usernameClaim = _tokenService.GetUsernameClaimFromToken(tokenDto.AccessToken);

            if (usernameClaim is null)
            {
                _logger.LogError("Username claim does not exist.");

                return null;
            }

            var identityUser = await _userManager.FindByNameAsync(usernameClaim.Value);

            if (identityUser is null)
            {
                _logger.LogError("User does not exist.");

                return null;
            }

            if (identityUser.RefreshTokens.Contains(tokenDto.RefreshToken))
            {
                var refreshToken = _tokenService.GenerateRefreshToken();
                var accessToken = _tokenService.GenerateAccessToken(identityUser.UserName, identityUser.Id, identityUser.Roles.FirstOrDefault()?.Name);

                identityUser.RefreshTokens.Remove(tokenDto.RefreshToken);
                identityUser.RefreshTokens.Add(refreshToken);

                var identityResult = await _userManager.UpdateAsync(identityUser);

                if (identityResult.Succeeded)
                {
                    return new TokenDto(accessToken, refreshToken, _jwtOptions.Expiration);
                }

                _logger.LogError(identityResult.Errors.First().Description);

                return null;
            }

            _logger.LogError("Refresh token is not valid.");

            return null;
        }
    }
}
