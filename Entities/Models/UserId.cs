using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Entities.Models
{
    public class UserId
    {
        [Key]
        public long Id { get; set; }    
        public long postId { get; set; }
        public long userId { get; set; }
    }
}
