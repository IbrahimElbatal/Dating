using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    public class PagedList<T> : List<T> where T : class
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }

        public PagedList(List<T> items, int currentPage, int totalPages, int totalItems, int pageSize)
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;
            TotalItems = totalItems;
            PageSize = pageSize;

            this.AddRange(items);
        }


        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageSize, int pageNumber)
        {
            var count = await source.CountAsync();
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            return new PagedList<T>(source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), pageNumber, totalPages, count, pageSize);
        }
    }
}
