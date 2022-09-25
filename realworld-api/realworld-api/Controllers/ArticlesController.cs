using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using realworld_api.Database;
using realworld_api.Models;
using realworld_api.Services;
using realworld_api.Validations;
using Slugify;

namespace realworld_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public ArticlesController(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        [Route("{slug}")]
        public async Task<ActionResult<ArticleResponse>> Get(string slug)
        {
            var currentArticle = await _context.article
                .Include(a => a.author)
                .Include(a => a.tags)
                .ThenInclude(t => t.tag)
                .SingleOrDefaultAsync(a => a.slug.ToLower().Trim().Equals(slug.ToLower().Trim()));

            if (currentArticle == null) return NotFound();

            string[] tags = currentArticle.tags.Select(t => t.tag.name).ToArray();
            ArticleResponse articleResponse = new()
            {
                article = new()
                {
                    slug = currentArticle.slug,
                    title = currentArticle.title,
                    description = currentArticle.description,
                    body = currentArticle.body,
                    tagList = tags,
                    article_created_at = currentArticle.article_created_at,
                    article_updated_at = currentArticle.article_updated_at,
                    favorited = await IsFavorited(_userService.GetUsername(), currentArticle.slug),
                    favorites_count = currentArticle.favorites_count,
                    author = new()
                    {
                        username = currentArticle.author.username,
                        bio = currentArticle.author.bio,
                        following = await IsFollowed(_userService.GetUsername(), currentArticle.author.username),
                        image = currentArticle.author.image
                    }
                }
            };
            return Ok(articleResponse);
        }

        [HttpGet, Authorize, AllowAnonymous]
        public ActionResult List([FromQuery] string? tag, string? author, string? favorited, int limit = 20, int offset = 0)
        {
            string tagQ = (string.IsNullOrEmpty(tag)) ? string.Empty : tag.ToLower().Trim();
            string authorQ = (string.IsNullOrEmpty(author)) ? string.Empty : author.ToLower().Trim();
            string favoritedQ = (string.IsNullOrEmpty(favorited)) ? string.Empty : favorited.ToLower().Trim();

            var listArticles =
                from a in _context.article
                join u in _context.user on a.id_author equals u.id_user
                join at2 in _context.article_tag on a.id_article equals at2.id_article
                join t in _context.tag on at2.id_tag equals t.id_tag
                select new
                {
                    a,
                    u,
                    t
                };

            var liss = listArticles.Where(el => el.t.name.Contains(tagQ) && el.u.username.Contains(authorQ)).ToList();

            return Ok(liss);
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<ArticleResponse>> Create(CreateArticleInput request)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var author = await _context.user.SingleOrDefaultAsync(u => u.username == _userService.GetUsername());
                if (author == null) return NotFound();

                var tagList = request.article.tagList;
                List<Tag> newTags = new();
                List<Tag> compilationTags = new();

                if (tagList != null)
                {
                    foreach (var tagEl in tagList)
                    {
                        var tagFromDb = await _context.tag.SingleOrDefaultAsync(t => t.name.ToLower().Trim().Equals(tagEl.ToLower().Trim()));
                        if (tagFromDb == null)
                        {
                            Tag newTag = new()
                            {
                                name = tagEl,
                                tag_created_at = DateTime.Now,
                                tag_updated_at = DateTime.Now
                            };
                            newTags.Add(newTag);
                            compilationTags.Add(newTag);
                        }
                        else
                        {
                            compilationTags.Add(tagFromDb);
                        }
                    }
                }

                Article newArticle = new()
                {
                    author = author,
                    slug = await slugGenerator(request.article.title),
                    title = request.article.title.Trim(),
                    description = request.article.description.Trim(),
                    body = request.article.body.Trim(),
                    favorites_count = 0,
                    article_created_at = DateTime.Now,
                    article_updated_at = DateTime.Now
                };

                List<ArticleTag> articleTags = new();
                foreach (var t in compilationTags)
                {
                    ArticleTag articleTag = new()
                    {
                        tag = t,
                        article = newArticle
                    };
                    articleTags.Add(articleTag);
                }

                newArticle.tags = articleTags;

                await _context.article.AddAsync(newArticle);
                await _context.tag.AddRangeAsync(newTags);
                await _context.SaveChangesAsync();

                transaction.Commit();

                ArticleResponse articleResponse = new()
                {
                    article = new()
                    {
                        slug = newArticle.slug,
                        title = newArticle.title,
                        description = newArticle.description,
                        body = newArticle.body,
                        tagList = tagList,
                        article_created_at = newArticle.article_created_at,
                        article_updated_at = newArticle.article_updated_at,
                        favorited = false,
                        favorites_count = newArticle.favorites_count,
                        author = new()
                        {
                            username = newArticle.author.username,
                            bio = newArticle.author.bio,
                            following = false,
                            image = newArticle.author.image
                        }
                    }
                };

                return Ok(articleResponse);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        [HttpPut, Authorize]
        [Route("{slug}")]
        public async Task<ActionResult<ArticleResponse>> Update(string slug, [FromBody] UpdateArticleInput request)
        {
            var slugLowered = slug.ToLower().Trim();
            var currentArticle = await _context.article
                .Include(ar => ar.author)
                .Include(ar => ar.tags)
                .ThenInclude(art => art.tag)
                .SingleOrDefaultAsync(ar => ar.slug.ToLower().Trim().Equals(slugLowered));
            if (currentArticle == null) return NotFound();

            currentArticle.slug = (string.IsNullOrEmpty(request.article.title)) ? currentArticle.slug : await slugGenerator(request.article.title);
            currentArticle.title = (string.IsNullOrEmpty(request.article.title)) ? currentArticle.title : request.article.title.Trim();
            currentArticle.description = (string.IsNullOrEmpty(request.article.description)) ? currentArticle.description : request.article.description.Trim();
            currentArticle.body = (string.IsNullOrEmpty(request.article.body)) ? currentArticle.body : request.article.body.Trim();
            currentArticle.article_updated_at = DateTime.Now;

            await _context.SaveChangesAsync();

            string[] tags = currentArticle.tags.Select(t => t.tag.name).ToArray();
            ArticleResponse articleResponse = new()
            {
                article = new()
                {
                    slug = currentArticle.slug,
                    title = currentArticle.title,
                    description = currentArticle.description,
                    body = currentArticle.body,
                    tagList = tags,
                    article_created_at = currentArticle.article_created_at,
                    article_updated_at = currentArticle.article_updated_at,
                    favorited = await IsFavorited(_userService.GetUsername(), currentArticle.slug),
                    favorites_count = currentArticle.favorites_count,
                    author = new()
                    {
                        username = currentArticle.author.username,
                        bio = currentArticle.author.bio,
                        following = await IsFollowed(_userService.GetUsername(), currentArticle.author.username),
                        image = currentArticle.author.image
                    }
                }
            };
            return Ok(articleResponse);
        }

        [HttpDelete, Authorize]
        [Route("{slug}")]
        public async Task<ActionResult<string>> Delete(string slug)
        {
            var slugLowered = slug.ToLower().Trim();
            var currentArticle = await _context.article
                .SingleOrDefaultAsync(ar => ar.slug.ToLower().Trim().Equals(slugLowered));
            if (currentArticle == null) return NotFound();

            _context.article.Remove(currentArticle);
            await _context.SaveChangesAsync();
            return Ok($"Success Delete Article with slug name: {currentArticle.slug}");
        }

        private async Task<bool> IsFavorited(string currentUsername, string slug)
        {
            if (!string.IsNullOrEmpty(currentUsername))
            {
                var curUser = await _context.user
                    .Include(u => u.user_favorite)
                    .ThenInclude(t => t.article_favorited)
                    .SingleOrDefaultAsync(u => u.username == currentUsername);
                if (curUser != null)
                {
                    foreach (var pf in curUser.user_favorite)
                    {
                        if (pf.article_favorited.slug == slug)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private async Task<bool> IsFollowed(string currentUsername, string viewedUsername)
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

        private async Task<string> slugGenerator(string title)
        {
            string titleLower = title.ToLower().Trim();
            var sameArticles = await _context.article.Where(a => a.title.ToLower().Trim().Equals(titleLower)).ToListAsync();

            SlugHelper slugHelper = new();
            string generatedSlug = slugHelper.GenerateSlug(titleLower);

            if (sameArticles.Count == 0) return generatedSlug;

            return $"{generatedSlug}-{DateTime.Now:yyyyMMddHHmmssffff}";
        }
    }
}
