using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Entities.Models
{
    public class Comment
    {
        [Key]
        public long Id { get; set; }
        public int UserId { get; set; }
        public String? Message { get; set; }
        public byte[]? Photo { get; set; }
        public List<Comment> SubComment { get; set; }

    }
}
