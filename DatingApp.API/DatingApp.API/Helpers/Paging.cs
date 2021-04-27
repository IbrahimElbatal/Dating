namespace DatingApp.API.Helpers
{
    public class Paging
    {
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }

        public Paging(int totalPages, int totalItems, int itemsPerPage, int currentPage)
        {
            TotalPages = totalPages;
            TotalItems = totalItems;
            ItemsPerPage = itemsPerPage;
            CurrentPage = currentPage;
        }
    }
}
