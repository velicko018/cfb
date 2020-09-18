using System.Threading.Tasks;
using CFB.Application.Services;
using CFB.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CFB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;

        public AccountsController(IAccountService accountService, ITokenService tokenService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tokenDto = await _accountService.LoginAsync(loginDto);

            if (tokenDto is null)
            {
                return BadRequest("The credentials are not valid.");
            }

            return Ok(tokenDto);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Register(UserForCreationDto userForCreationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _accountService.RegisterAsync(userForCreationDto);

            if (user is null)
            {
                return NotFound("User not created.");
            }

            var loginDto = new LoginDto
            {
                Email = userForCreationDto.Email,
                Password = userForCreationDto.Password
            };
            var tokenDto = await _accountService.LoginAsync(loginDto);

            if (tokenDto is null)
            {
                return BadRequest("The credentials are not valid.");
            }

            return Ok(tokenDto);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken(TokenDto tokenDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var token = await _accountService.RefreshTokenAsync(tokenDto);

            if (token is null)
            {
                return BadRequest();
            }

            return Ok(token);
        }

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Logout()
        {
            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var token) && _tokenService.InvalidateOrCheckAccessToken(token))
            {
                return Ok();
            }

            return NotFound("Token does not exist.");
        }

        [Authorize]
        [HttpGet("current-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CurrentUser()
        {
            var currentUser = await _accountService.GetUserByNameAsync(User.Identity.Name);

            if (currentUser is null)
            {
                return NotFound("User does not exist.");
            }

            return Ok(currentUser);
        }
    }
}