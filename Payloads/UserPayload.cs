using System.ComponentModel.DataAnnotations;

namespace Backend.Payloads
{
    public class UserPayload
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Gender { get; set; }
        public int Id { get; set; }





    }
}
