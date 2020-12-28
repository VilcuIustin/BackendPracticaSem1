using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entities.Models
{
    public class Post
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("IdUser")]
        public int IdUser { get; set; }
        public string? Text { get; set; }
        public ICollection<ImgURL>? Images { get; set; }
        public DateTime DTPost { get; set; }
        public int NrLikes { get; set; }
        public virtual ICollection<Comment> PostComment { get; set; }



    }
}
