using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Entities.Models
{
    public class Comment
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public string? Message { get; set; }
        public ImgURL? Image { get; set; }
        public List<Comment> SubComment { get; set; }

    }
}
