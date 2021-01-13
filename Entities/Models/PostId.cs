using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entities.Models
{
    public class PostId
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("post")]
        public long postId { get; set; }
        //[ForeignKey("userId")]
        public long userId { get; set; }

    }
}
