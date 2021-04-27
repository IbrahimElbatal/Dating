using Microsoft.AspNetCore.Http;
using System;

namespace DatingApp.API.Dtos
{
    public class PhotoForUploadDto
    {
        public IFormFile File { get; set; }
        public bool IsMain { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }

        public PhotoForUploadDto()
        {
            DateAdded = DateTime.UtcNow;
        }
    }
}
