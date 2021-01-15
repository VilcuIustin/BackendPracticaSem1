using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entities.Models
{
    public class Friend
    {
        [Key]
        public long id { get; set; }
        public long User1id { get; set; }
        public User User1 { get; set; }        
       
        public User User2 { get; set; }  
           
        public bool status { get; set; } //true- the user accepted the request      false- the user dindn't accepted the request yet
      

    }
}
