using DogGrooming_Server.Data.Models;
using DogGrooming_Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogGrooming_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserService _userService;
        private readonly JwtTokenService _tokenService;

        public UserController(UserService userService, JwtTokenService jwtTokenService)
        {
            _userService = userService;
            _tokenService = jwtTokenService;
        }

        // SignUp User
        [HttpPost("signup")]
        public async Task<ActionResult<User>> SignUpUser(User user)
        {
            try
            {
                var newUser = await _userService.SignUpUser(user);

                return StatusCode(201,newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "תקלה בניסיון הרשמה " + ex);
            }
        }
        // Login User
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(Login login)
        {
            try
            {
                var user = await _userService.Login(login);

                if (user == null)
                {
                    return Unauthorized("Password or username not true");
                }

                var token = _tokenService.GenerateToken(user.UserId.ToString(), user.Role);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "תקלה בניסיון התחברות " + ex);
            }
        }

        // GET: api/User/
        // Get User 
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetUser()
        {
            var user = await GetUserFromToken();

            var res = await _userService.GetUserById(user.UserId);
            return Ok(res);
        }

        private async Task<User> GetUserFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("Invalid JWT token.");
            }
            var userId = Int32.Parse(userIdClaim.Value);
            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            return user;
        }
    }
}
