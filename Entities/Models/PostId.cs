using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Entities.Models
{
    public class PostId
    {
        [Key]
        public long Id { get; set; }
        public long postId { get; set; }
        [ForeignKey("user")]
        public long userId { get; set; }
    
    }
}
