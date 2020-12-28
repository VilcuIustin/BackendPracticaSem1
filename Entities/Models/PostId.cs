using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entities.Models
{
    public class PostId
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("postId")]
        public long postId { get; set; }
        [ForeignKey("Iduser")]
        public long userId { get; set; }

    }
}
