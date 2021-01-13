using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
#nullable enable
namespace Backend.Payloads
{
    public class CommentPayload
    {
        [Required]
        public long postId { get; set; }

        public long? CommentId { get; set; }
        public string? Message { get; set; }
        public IFormFileCollection? Image { get; set; }

    }
}
