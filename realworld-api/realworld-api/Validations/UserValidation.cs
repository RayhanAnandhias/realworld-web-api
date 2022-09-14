using System.ComponentModel.DataAnnotations;

namespace realworld_api.Validations
{
    public class UserRegisterInput
    {
        public class UserRegisterBody
        {
            [Required]
            [EmailAddress]
            public string email { get; set; }
            [Required]
            public string username { get; set; }
            [Required]
            public string password { get; set; }
        }

        [Required]
        public UserRegisterBody user { get; set; }
    }

    public class UserLoginInput
    {
        public class UserLoginBody
        {
            [Required]
            [EmailAddress]
            public string email { get; set; }
            [Required]
            public string password { get; set; }
        }

        [Required]
        public UserLoginBody user { get; set; }

    }

    public class UserUpdateInput
    {
        public class UserUpdateBody
        {
            [EmailAddress]
            public string? email { get; set; }
            public string? username { get; set; }
            public string? password { get; set; }
            public string? bio { get; set; }
            public string? image { get; set; }
        }

        [Required]
        public UserUpdateBody user { get; set; }

    }

    public class UserResponse
    {
        public class UserDataResponse
        {
            public string email { get; set; }
            public string? token { get; set; }
            public string username { get; set; }
            public string? bio { get; set; }
            public string? image { get; set; }
        }

        public UserDataResponse user { get; set; }
    }
}
