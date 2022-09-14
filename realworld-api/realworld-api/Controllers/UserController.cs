using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using realworld_api.Database;
using realworld_api.Validations;
using realworld_api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using realworld_api.Services;

namespace realworld_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public UserController(ApplicationDbContext context, IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _context = context;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> register([FromBody] UserRegisterInput request)
        {
            DateTime currentDateTime = DateTime.Now;
            User curUser = new()
            {
                email = request.user.email,
                username = request.user.username,
                password = hashPasswordWithBcrypt(request.user.password),
                user_created_at = currentDateTime,
                user_updated_at = currentDateTime
            };

            await _context.user.AddAsync(curUser);
            await _context.SaveChangesAsync();

            UserResponse user = new()
            {
                user = new()
                {
                    email = curUser.email,
                    token = null,
                    username = curUser.username,
                    bio = curUser.bio,
                    image = curUser.image
                }
            };
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserResponse>> login([FromBody] UserLoginInput request)
        {
            var curUser = await _context.user.SingleOrDefaultAsync(u => u.email == request.user.email);

            if (curUser == null)
            {
                return BadRequest("User not found.");
            }

            if (!CompareHashWithPassword(request.user.password, curUser.password))
            {
                return BadRequest("Wrong password.");
            }

            string jwtToken = CreateToken(curUser);
            UserResponse user = new()
            {
                user = new()
                {
                    email = curUser.email,
                    token = jwtToken,
                    username = curUser.username,
                    bio = curUser.bio,
                    image = curUser.image
                }
            };
            return Ok(user);
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<UserResponse>> Get()
        {
            var currentUsername = _userService.GetUsername();
            var curUser = await _context.user.SingleOrDefaultAsync(u => u.username == currentUsername);

            if(curUser == null)
            {
                return Unauthorized();
            }

            UserResponse user = new()
            {
                user = new()
                {
                    email = curUser.email,
                    token = _userService.GetToken(),
                    username = curUser.username,
                    bio = curUser.bio,
                    image = curUser.image
                }
            };
            return Ok(user);
        }

        [HttpPut, Authorize]
        public async Task<ActionResult<UserResponse>> Put([FromBody] UserUpdateInput request)
        {
            var currentUsername = _userService.GetUsername();
            var curUser = await _context.user.SingleOrDefaultAsync(u => u.username == currentUsername);

            if (curUser == null)
            {
                return Unauthorized();
            }

            curUser.email = (request.user.email == null || request.user.email.Length == 0) ? curUser.email : request.user.email;
            curUser.username = (request.user.username == null || request.user.username.Length == 0) ? curUser.username : request.user.username;
            curUser.password = (request.user.password == null || request.user.password.Length == 0) ? curUser.password : request.user.password;
            curUser.bio = (request.user.bio == null || request.user.bio.Length == 0) ? curUser.bio : request.user.bio;
            curUser.image = (request.user.image == null || request.user.image.Length == 0) ? curUser.image : request.user.image;
            curUser.user_updated_at = DateTime.Now;

            await _context.SaveChangesAsync();

            UserResponse user = new()
            {
                user = new()
                {
                    email = curUser.email,
                    token = _userService.GetToken(),
                    username = curUser.username,
                    bio = curUser.bio,
                    image = curUser.image
                }
            };
            return Ok(user);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.username)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private string hashPasswordWithBcrypt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool CompareHashWithPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
