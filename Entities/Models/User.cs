using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Entities.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
        public ImgURL ProfilePic { get; set; }
        public virtual ICollection<UserId> Followers { get; set; }
        public virtual ICollection<UserId> Following { get; set; }
        public virtual ICollection<PostId> PostLiked { get; set; }
        public virtual ICollection<PostId> MyPosts { get; set; }





    }
}
