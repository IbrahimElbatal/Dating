using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.API.Models
{
    public class Like
    {
        public int LikeeId { get; set; }
        public int LikerId { get; set; }
        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}
