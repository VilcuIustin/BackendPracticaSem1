using System.ComponentModel.DataAnnotations;

namespace Backend.Payloads
{
    public class LoginPayload
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
