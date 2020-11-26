using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Entities.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
        //public List<Post> MyPosts { get; set; } 
        public List<User> Fallow { get; set; }


        public virtual ICollection<Post> MyPosts { get; set; }

    }
}
