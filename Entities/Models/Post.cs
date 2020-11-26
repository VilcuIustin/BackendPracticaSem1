using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Entities.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public string? Text { get; set; }
        public Image? Img { get; set; }
        public int NrLikes { get; set; }
       


    }
}
