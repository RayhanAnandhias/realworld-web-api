using System.ComponentModel.DataAnnotations;
using static realworld_api.Validations.ProfileResponse;

namespace realworld_api.Validations
{
    public class CreateArticleInput
    {
        public class CreateArticleBody
        {
            [Required]
            public string title { get; set; }
            [Required]
            public string description { get; set; }
            [Required]
            public string body { get; set; }
            public string[]? tagList { get; set; }
        }

        [Required]
        public CreateArticleBody article { get; set; }
    }

    public class UpdateArticleInput
    {
        public class UpdateArticleBody
        {
            public string? title { get; set; }
            public string? description { get; set; }
            public string? body { get; set; }
        }

        [Required]
        public UpdateArticleBody article { get; set; }
    }

    public class ArticleResponse
    {
        public class ArticleBodyResponse
        {
            public string slug { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string body { get; set; }
            public string[]? tagList { get; set; }
            public DateTime article_created_at { get; set; }
            public DateTime article_updated_at { get; set; }
            public bool favorited { get; set; }
            public int favorites_count { get; set; }
            public ProfileBodyResponse author { get; set; }
        }

        public ArticleBodyResponse article { get; set; }
    }
}
