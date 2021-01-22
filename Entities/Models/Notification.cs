using System.ComponentModel.DataAnnotations;

namespace Backend.Entities.Models
{
    public class Notification
    {

        [Key]
        public long id { get; set; }
        public string message { get; set; }
        public long idReceiver { get; set; }
        public long idSender { get; set; }
        public string NotificationPath { get; set; }
        public bool status { get; set; }            // true if the user saw the notification
    }
}
