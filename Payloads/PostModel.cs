using Backend.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Payloads
{
    public class PostModel
    {
        public string fullname { get; set; }
        public long idUser { get; set; }
        public string profilePic { get; set; }
        public long id { get; set; }
        public string? Text { get; set; }
        public ICollection<ImgURL>? Images { get; set; }
        public DateTime DTPost { get; set; }
        public int NrLikes { get; set; }
        public long nrComm { get; set; }


    }
}
