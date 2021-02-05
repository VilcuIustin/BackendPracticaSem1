using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Payloads
{
    public class EmailPayload
    {
        [Required]
        public string Email { get; set; }
    }
}
