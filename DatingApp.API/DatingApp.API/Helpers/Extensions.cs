using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static int CalculateAge(this DateTime dateOfBirth)
        {
            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.AddYears(age) > DateTime.Today)
                age--;
            return age;
        }

        public static void AddPagination<T>(this HttpResponse response, PagedList<T> pagedList) where T : class
        {
            var paging =
                new Paging(pagedList.TotalPages, pagedList.TotalItems, pagedList.PageSize, pagedList.CurrentPage);

            var setting = new JsonSerializerSettings();
            setting.ContractResolver = new CamelCasePropertyNamesContractResolver();

            response.Headers.Add("Pagination",
                JsonConvert.SerializeObject(paging, setting));

            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");

        }
    }
}
