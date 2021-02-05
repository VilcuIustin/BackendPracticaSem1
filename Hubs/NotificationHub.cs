using Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
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

       
        public override Task OnConnectedAsync()
        {
            if(((ClaimsIdentity)Context.User.Identity).HasClaim(claims => claims.Type== "Id")){
                long id;
                if(long.TryParse(((ClaimsIdentity)Context.User.Identity).Claims.First(claims => claims.Type == "Id").Value, out id))
                {
                    ConectionMapping.Instance.Add(id, Context.ConnectionId);
                    
                }
            }
            var nr= ConectionMapping.Instance.Count();
            return base.OnConnectedAsync();
        }
       
        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (((ClaimsIdentity)Context.User.Identity).HasClaim(claims => claims.Type == "Id"))
            {
                long id;
                if (long.TryParse(((ClaimsIdentity)Context.User.Identity).Claims.First(claims => claims.Type == "Id").Value, out id))
                {
                    ConectionMapping.Instance.Remove(id, Context.ConnectionId);

                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task GetNotifications()
        {
            long id;
            if (!((ClaimsIdentity)Context.User.Identity).HasClaim(claims => claims.Type == "Id"))
            {
                return;
            }
            id = long.Parse(((ClaimsIdentity)Context.User.Identity).Claims.First(claims => claims.Type == "Id").Value);
          
            var me = _db.Users.Where(user => user.Id == id).Single();

            await Clients.All.SendAsync("NotificationReceived", me.newNotifications);
        }

       

    }
}
