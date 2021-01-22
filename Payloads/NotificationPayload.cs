using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Payloads
{
    public class NotificationPayload
    {
        public string FullName { get; set; }
        public string image { get; set; }
        public string link { get; set; }
        public long id { get; set; }
    }
}
