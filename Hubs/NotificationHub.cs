using Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly BackendContext _db;
        public NotificationHub(BackendContext db)
        {
            _db = db;
        }
        public async Task SendNotification()
        {

            long id;

            if (!((ClaimsIdentity)Context.User.Identity).HasClaim(claims => claims.Type == "Id"))
            {
                return;
            }
            id = long.Parse(((ClaimsIdentity)Context.User.Identity).Claims.First(claims => claims.Type == "Id").Value);
            // while (true)
            // {
            var me = _db.Users.Where(user => user.Id == id).Single();

            await Clients.All.SendAsync("NotificationReceived", me.newNotifications);

            //    Thread.Sleep(2000);
            //    System.GC.Collect();
            //   }


        }

    }
}
