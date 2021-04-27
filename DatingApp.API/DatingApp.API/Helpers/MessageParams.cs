namespace DatingApp.API.Helpers
{
    public class MessageParams
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
        public string MessageContainer { get; set; } = "UnRead";

    }
}
