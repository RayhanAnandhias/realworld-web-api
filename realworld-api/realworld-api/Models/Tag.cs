using System.ComponentModel.DataAnnotations;

namespace realworld_api.Models
{
    public class Tag
    {
        [Key]
        public int id_tag { get; set; }
        public string name { get; set; }
        public DateTime tag_created_at { get; set; }
        public DateTime tag_updated_at { get; set; }
        public ICollection<ArticleTag> articles { get; set; }
    }
}
