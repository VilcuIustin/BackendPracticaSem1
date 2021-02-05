using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Payloads
{
    public class passwordPayload
    {
        [Required]
        public string Password { get; set; }
    }
}
