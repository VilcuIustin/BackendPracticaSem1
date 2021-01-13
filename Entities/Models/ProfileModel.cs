using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Entities.Models
{
    public class ProfileModel
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string ImgUrl { get; set; }
        public Enums.FollowType friendStatus { get; set; }

    }
}
