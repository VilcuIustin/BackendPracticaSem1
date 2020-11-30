using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Entities.Models
{
    public class Post
    {
        [Key]
        public long Id { get; set; }
        public int IdUser { get; set; }
        public string? Text { get; set; }
        public Image? Img { get; set; }
        public DateTime DTPost { get; set; }
        public int NrLikes { get; set; }
        public virtual ICollection<Comment> PostComment { get; set; }
       


    }
}
