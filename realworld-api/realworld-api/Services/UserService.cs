using System.Security.Claims;

namespace realworld_api.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUsername()
        {
            var result = string.Empty;

            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }
            return result;
        }

        public string GetToken()
        {
            var result = string.Empty;

            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                result = result.Split(" ")[1].Trim();
            }
            return result;
        }
    }
}
