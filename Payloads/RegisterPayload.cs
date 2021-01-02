using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Controllers
{
    public class RegisterPayload
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public DateTime Birthday { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string gender { get; set; }




    }
}