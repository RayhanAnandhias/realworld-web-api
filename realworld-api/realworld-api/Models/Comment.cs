using System.ComponentModel.DataAnnotations;

namespace realworld_api.Models
{
    public class Comment
    {
        [Key]
        public int id_comment { get; set; }
        public int id_article { get; set; }
        public Article article { get; set; }
        public int id_user { get; set; }
        public User user { get; set; }
        public string comment_body { get; set; }
        public DateTime comment_created_at { get; set; }
        public DateTime comment_updated_at { get; set; }
    }
}
