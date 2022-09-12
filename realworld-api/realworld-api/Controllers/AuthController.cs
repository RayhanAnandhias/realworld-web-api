using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using realworld_api.Database;
using realworld_api.DTO;
using realworld_api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace realworld_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> register(UserDTO request)
        {
            DateTime currentDateTime = DateTime.Now;
            User user = new()
            {
                email = request.email,
                username = request.username,
                password = hashPasswordWithBcrypt(request.password),
                user_created_at = currentDateTime,
                user_updated_at = currentDateTime
            };
            await _context.user.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> login(UserDTO request)
        {
            var curUser = await _context.user.SingleOrDefaultAsync(u => u.email == request.email);

            if (curUser == null)
            {
                return BadRequest("User not found.");
            }

            if (!CompareHashWithPassword(request.password, curUser.password))
            {
                return BadRequest("Wrong password.");
            }

            string jwtToken = CreateToken(curUser);
            var objectResponse = new
            {
                user = new
                {
                    email = curUser.email,
                    token = jwtToken,
                    username = curUser.username,
                    bio = curUser.bio,
                    image = curUser.image
                }
            };
            return Ok(objectResponse);
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
