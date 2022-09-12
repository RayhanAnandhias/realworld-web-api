using System.ComponentModel.DataAnnotations;

namespace realworld_api.Models
{
    public class User
    {
        [Key]
        public int id_user { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string? bio { get; set; }
        public string? image { get; set; }
        public DateTime user_created_at { get; set; }
        public DateTime user_updated_at { get; set; }
        public ICollection<ProfileFollowing> followers { get; set; }
        public ICollection<ProfileFollowing> following { get; set; }
        public ICollection<Article> articles { get; set; }
        public ICollection<UserFavorite> user_favorite { get; set; }
        public ICollection<Comment> comments { get; set; }
    }

    public class ProfileFollowing
    {
        public int id_user_observer { get; set; }
        public int id_user_target { get; set; }

        public User user_observer { get; set; }
        public User user_target { get; set; }
    }

    public class UserFavorite
    {
        public int id_user { get; set; }
        public int id_article_favorite { get; set; }
        public User user { get; set; }
        public Article article_favorited { get; set; }
    }
}
