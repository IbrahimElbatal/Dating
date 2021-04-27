using System;

namespace DatingApp.API.Dtos
{
    public class CreateMessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        public DateTime DateSent { get; set; }

        public CreateMessageDto()
        {
            DateSent = DateTime.Now;
        }
    }
}
