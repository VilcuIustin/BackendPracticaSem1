using System.ComponentModel.DataAnnotations;

namespace Backend.Entities.Models
{
    public class ImgURL
    {
        [Key]
        public long Id { get; set; }
        public string ImgUrl { get; set; }


    }
}
