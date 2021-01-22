using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entities.Models
{
    public class User
    {
        [Key]

        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
        public ImgURL ProfilePic { get; set; }
        public int newNotifications { get; set; }
        public virtual ICollection<Friend> Friends { get; set; }
        public virtual ICollection<Notification> notifications { get; set; }
        //[ForeignKey("idPost")]
        public virtual ICollection<PostId> PostLiked { get; set; }
        //[ForeignKey("MyPost")]
        [ForeignKey("idPost")]
        public virtual ICollection<PostId> MyPosts { get; set; }
       
    }
}
