using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Backend.Payloads
{
    public class PostPayload
    {
        [Required]
        public int UserId { get; set; }

        public int Id { get; set; }
        public string? Text { get; set; }
        public IFormFileCollection? Photos { get; set; }
    }
}
