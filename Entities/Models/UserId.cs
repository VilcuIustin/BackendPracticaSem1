using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Entities.Models
{
    public class UserId
    {
        [Key]
        public long id { get; set; }
        [ForeignKey("Follows")]
        public long followedBy { get; set; }
        [ForeignKey("Followed")]
        public long following { get; set; }
        public DataType dateFollowing { get; set; }


    }
}
