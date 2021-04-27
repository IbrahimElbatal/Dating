namespace DatingApp.API.Helpers
{
    public interface ISetting
    {
        string Key { get; set; }
        string Audience { get; set; }
        string Issuer { get; set; }
        Cloudinary Cloudinary { get; set; }
    }

    public class Setting : ISetting
    {
        public string Key { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public Cloudinary Cloudinary { get; set; }
    }

    public class Cloudinary
    {
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}
