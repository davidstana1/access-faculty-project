using System.ComponentModel.DataAnnotations;
using backend.entity;
using backend.entity.auth;
using backend.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public AuthController(
            UserManager<User> userManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized(new { message = "Email sau parolă incorectă" });

            var tokenResponse = await _tokenService.GenerateTokens(user);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                token = tokenResponse.AccessToken,
                refreshToken = tokenResponse.RefreshToken,
                expiration = tokenResponse.ExpiresAt,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    roles = roles
                }
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            try
            {
                var tokenResponse = await _tokenService.RefreshToken(model.RefreshToken);

                return Ok(new
                {
                    token = tokenResponse.AccessToken,
                    refreshToken = tokenResponse.RefreshToken,
                    expiration = tokenResponse.ExpiresAt
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutModel model)
        {
            await _tokenService.RevokeToken(model.RefreshToken);
            return Ok(new { message = "Deconectare reușită" });
        }

        [Authorize(Policy = "HROnly")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // verify if user exists
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return BadRequest(new { message = "Un utilizator cu acest email există deja" });

            // create new user
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DivisionId = model.DivisionId
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Eroare la crearea utilizatorului", errors = result.Errors });

            // add user to specified role
            await _userManager.AddToRoleAsync(user, model.Role);

            return Ok(new { message = "Utilizator creat cu succes" });
        }
    }
}