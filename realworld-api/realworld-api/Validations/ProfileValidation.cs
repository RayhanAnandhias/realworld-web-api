namespace realworld_api.Validations
{
    public class ProfileResponse
    {
        public class ProfileBodyResponse
        {
            public string username { get; set; }
            public string? bio { get; set; }
            public string? image { get; set; }
            public bool following { get; set; }
        }

        public ProfileBodyResponse profile { get; set; }
    }
}
