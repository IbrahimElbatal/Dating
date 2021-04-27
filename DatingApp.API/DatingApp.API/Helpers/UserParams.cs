namespace DatingApp.API.Helpers
{
    public class UserParams
    {
        private readonly int maxSize = 30;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 6;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value) > maxSize ? maxSize : _pageSize;
        }

        public int UserId { get; set; }
        public string Gender { get; set; }

        public int AgeFrom { get; set; } = 18;
        public int AgeTo { get; set; } = 99;

        public string Sort { get; set; }

        public bool Likers { get; set; } = false;
        public bool Likees { get; set; } = false;

    }
}
