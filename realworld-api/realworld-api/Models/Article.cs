using Microsoft.Extensions.DependencyModel;
using System.ComponentModel.DataAnnotations;

namespace realworld_api.Models
{
    public class Article
    {
        [Key]
        public int id_article { get; set; }
        public int? id_author { get; set; }
        public User author { get; set; }
        public string slug { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string body { get; set; }
        public int favorites_count { get; set; }
        public DateTime article_created_at { get; set; }
        public DateTime article_updated_at { get; set; }
        public ICollection<UserFavorite> user_favorite { get; set; }
        public ICollection<Comment> comments { get; set; }
        public ICollection<ArticleTag> tags { get; set; }
    }

    public class ArticleTag
    {
        public int id_article { get; set; }
        public int id_tag { get; set; }
        public Article article { get; set; }
        public Tag tag { get; set; }
    }
}
