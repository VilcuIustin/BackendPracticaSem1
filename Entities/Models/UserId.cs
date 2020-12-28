using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entities.Models
{
    public class UserId
    {
        [Key]
        public long id { get; set; }
        [ForeignKey("Follows")]
        public long followedBy { get; set; }        // me 
        [ForeignKey("Followed")]
        public long following { get; set; }         // user i want to follow
        public bool status { get; set; } //true- the user accepted the request      false- the user dindn't accepted the request yet
        public DataType dateFollowing { get; set; }


    }
}
