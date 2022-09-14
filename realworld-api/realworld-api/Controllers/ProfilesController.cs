using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using realworld_api.Database;
using realworld_api.Services;
using realworld_api.Validations;
using Microsoft.EntityFrameworkCore;
using realworld_api.Models;

namespace realworld_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public ProfilesController(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet, Authorize, AllowAnonymous]
        [Route("{username}")]
        public async Task<ActionResult<ProfileResponse>> Get(string username)
        {
            var currentUsername = _userService.GetUsername();

            bool isFollowing = await CheckFollowing(currentUsername, username);

            var curViewedProfile = await _context.user.SingleOrDefaultAsync(u => u.username == username);

            if (curViewedProfile == null)
            {
                return NotFound();
            }

            ProfileResponse profile = new()
            {
                profile = new()
                {
                    username = curViewedProfile.username,
                    bio = curViewedProfile.bio,
                    image = curViewedProfile.image,
                    following = isFollowing
                }
            };

            return Ok(profile);
        }

        [HttpPost, Authorize]
        [Route("{username}/follow")]
        public async Task<ActionResult<ProfileResponse>> follow(string username)
        {
            var currentUsername = _userService.GetUsername();
            var curUser = await _context.user.SingleOrDefaultAsync(u => u.username == currentUsername);
            if (curUser == null) return NotFound();
            var followedUser = await _context.user.SingleOrDefaultAsync(u => u.username == username);
            if (followedUser == null) return NotFound();
            await _context.profile_following.AddAsync(new()
            {
                user_observer = curUser,
                user_target = followedUser
            });
            await _context.SaveChangesAsync();
            bool isFollowing = await CheckFollowing(currentUsername, username);
            ProfileResponse profile = new()
            {
                profile = new()
                {
                    username = followedUser.username,
                    bio = followedUser.bio,
                    image = followedUser.image,
                    following = isFollowing
                }
            };

            return Ok(profile);
        }

        [HttpDelete, Authorize]
        [Route("{username}/follow")]
        public async Task<ActionResult<ProfileResponse>> unfollow(string username)
        {
            var currentUsername = _userService.GetUsername();
            
            var curProfileFollowing = await _context.profile_following
                .Include(pf => pf.user_observer)
                .Include(pf => pf.user_target)
                .SingleOrDefaultAsync(pf => pf.user_observer.username == currentUsername && pf.user_target.username == username);

            if (curProfileFollowing == null) return NotFound();

            _context.profile_following.Remove(curProfileFollowing);
            await _context.SaveChangesAsync();
            bool isFollowing = await CheckFollowing(currentUsername, username);
            ProfileResponse profile = new()
            {
                profile = new()
                {
                    username = curProfileFollowing.user_target.username,
                    bio = curProfileFollowing.user_target.bio,
                    image = curProfileFollowing.user_target.image,
                    following = isFollowing
                }
            };

            return Ok(profile);
        }

        private async Task<bool> CheckFollowing(string currentUsername, string viewedUsername)
        {
            if (!string.IsNullOrEmpty(currentUsername))
            {
                var curUser = await _context.user
                    .Include(f => f.following)
                    .ThenInclude(t => t.user_target)
                    .SingleOrDefaultAsync(u => u.username == currentUsername);

                if (curUser != null)
                {
                    foreach (var pf in curUser.following)
                    {
                        if (pf.user_target.username == viewedUsername)
                        {
                            return true;
                        }
                    }
                }

            }

            return false;
        }
    }
}
