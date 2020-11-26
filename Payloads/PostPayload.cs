using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Entities.Models;

namespace Backend.Payloads
{
    public class PostPayload
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string? Text { get; set; }
        public Image? photo { get; set; }
    }
}
