﻿using Backend.Entities;
using Backend.Entities.Models;
using Backend.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Backend.Enums;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : Controller
    {
        private readonly BackendContext _db;
        private readonly IHubContext<NotificationHub> _notification;

        public FollowController(BackendContext db, IHubContext<NotificationHub> notification)
        {
            _db = db;
            _notification = notification;

        }

        [HttpGet("getfollowers/{id}")]              //0 - the same user.     1 - not followed   2 - your request is in pending 
                                                    //  3 - you have a request in pending 4 - fallowed -1 - not followed
        public async Task<ActionResult<Enums.FollowType>> GetFallowers([FromRoute] long id)
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(claims => claims.Type == "Id"))
            {
                long myId;
                bool re = long.TryParse((currentUser.Claims.FirstOrDefault(c => c.Type == "Id").Value), out myId);
                if (!re)
                    return BadRequest("Can't parse id");

                if (myId == id)
                {
                    return FollowType.Same;
                }
                try
                {
                    User user1 = _db.Users.Include(user => user.Friends)
                        .ThenInclude(user=> user.User1)
                        .Include(user => user.Friends)
                        .ThenInclude(user => user.User2)
                      .Where(user => user.Id == myId).Single();
                    User user2 = _db.Users.Include(user => user.Friends)
                        .Where(user => user.Id == id).Single();
                    Friend? a1 = null;
                    Friend? a2 = null;

                    //a1 = user1.Friends.Where(user => user.User1.Id == myId && user.User2.Id == id).FirstOrDefault();
                    //a2 = user1.Friends.Where(user => user.User1.Id == id && user.User2.Id == myId).FirstOrDefault();
                    a1 = user1.Friends.Where(user => user.User1.Id == id && user.User2.Id == myId).FirstOrDefault();
                    if (a1 != null && a1?.status == false)
                    {
                        if (a1.sent == true)
                        {
                            return FollowType.Pending;
                        }
                        else
                            return FollowType.Wait;
                    }
                    if (a1 != null && a1?.status == true)
                    {
                        return FollowType.Friends;
                    }

                    return FollowType.NotFriends;


                }
                catch (Exception)
                {

                    return FollowType.UserNotFound;
                }
            }
            return BadRequest("Your token is invalid");


        }


        [HttpPost("followreq")]
        public async Task<ActionResult> AddFriend([FromBody] long id)     
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(claims => claims.Type == "Id"))
            {
                long idUser;
                bool re = long.TryParse((currentUser.Claims.FirstOrDefault(c => c.Type == "Id").Value), out idUser);
                if (!re)
                    return BadRequest("Can't parse id");
               
                try
                {
                    var me = _db.Users.Include(user => user.Friends).ThenInclude(user=> user.User1).Include(user => user.ProfilePic).Where(user => user.Id == idUser).Single();
                    //var follow1 = me.Following.FirstOrDefault(follow => follow.fo == id);
                    var follow2 = me.Friends.FirstOrDefault(follow => follow.User1.Id == id);
                    if (follow2 != null)
                    {
                        return (ActionResult)await acceptFollow(id);
                    }
                    var userForFollow = _db.Users.Include(user => user.Friends).Include(user => user.notifications).Include(user=> user.ProfilePic).Where(user => user.Id == id).Single();

                    me.Friends.Add(new Friend
                    {
                        User1 = userForFollow,
                        User2 = me,
                        status = false,
                        sent = true,

                    }) ;
                    userForFollow.Friends.Add(new Friend
                    {
                        User1 = me,
                        User2 = userForFollow,
                        status = false,
                        sent = false,

                    }) ;
                    userForFollow.newNotifications++;

                    userForFollow.notifications.Add(new Notification
                    {
                        message = "sent you a friend request",
                        idReceiver = userForFollow.Id,
                        idSender = me.Id,
                        NotificationPath = "profile",
                    });


                     _db.SaveChanges();
                    await sendNotifications(id, userForFollow.newNotifications);
                    return new JsonResult(new { status = "true", message = "Follow request sended" });
                }
                catch (InvalidOperationException)
                {
                    return new JsonResult(new { status = "false", message = "user not found" });
                }


            }

            return BadRequest("Id not found");
        }


        [HttpDelete("Unfallow/{id}")]
        public async Task<IActionResult> unfallow(long id)
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(claims => claims.Type == "Id"))
            {
                long myId;
                bool re = long.TryParse((currentUser.Claims.FirstOrDefault(c => c.Type == "Id").Value), out myId);
                if (!re)
                    return BadRequest("Can't parse id");

                if (myId == id)
                {
                    return new JsonResult(new { status = "false", message = "you can't unfollow yourself" });

                }
                User me;
                User otherusr;
                try
                {
                    me = _db.Users.Include(user => user.Friends)
                        .ThenInclude(user=> user.User1)
                        .Where(user => user.Id == myId).Single();
                    otherusr = _db.Users.Include(user => user.Friends)
                        .ThenInclude(user => user.User1)
                        .Where(user => user.Id == id).Single();
                }
                catch (ArgumentNullException)
                {
                    return BadRequest("User not found");

                }

                Friend following = null;
                Friend follower = null;
                
                try
                {
                    following = me.Friends.Where(user => user.User1.Id == id && user.User2.Id == myId).FirstOrDefault();
                    follower = otherusr.Friends.Where(user => user.User1.Id == myId && user.User2.Id == id).FirstOrDefault();
                    me.Friends.Remove(following);
                    otherusr.Friends.Remove(follower);
                    _db.Remove(follower);
                    _db.Remove(following);
                    _db.SaveChanges();
                    
                    return new JsonResult(new { status = "true", message = " success" });
                }
                catch (Exception)
                { }

                return new JsonResult(new { status = "false", message = "You are not following this user" });

            }
            return new JsonResult(new { status = "false", message = "token invalid" });
        }

        [HttpPost("acceptfollow")]
        public async Task<IActionResult> acceptFollow([FromBody] long id)
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(claims => claims.Type == "Id"))
            {
                long myId;
                bool re = long.TryParse((currentUser.Claims.FirstOrDefault(c => c.Type == "Id").Value), out myId);
                if (!re)
                    return BadRequest("Can't parse id");

                if (myId == id)
                {
                    return new JsonResult(new { status = "false", message = "you can't fallow yourself" });

                }
                User me;
                User otherusr;
                try
                {
                    me = _db.Users.Include(user => user.Friends)
                        .ThenInclude(user=> user.User1)
                        .Where(user => user.Id == myId).Single();
                    otherusr = _db.Users.Include(user => user.Friends)
                        .ThenInclude(user => user.User1)
                        .Include(user => user.notifications)
                        .Where(user => user.Id == id).Single();
                }
                catch (ArgumentNullException)
                {
                    return BadRequest("User not found");

                }

                Friend following = null;
                Friend follower = null;
              
                try
                {
                    // following = me.Friends.Where(user => user.User1.Id == id).Single();
                    //follower = otherusr.Friends.Where(user => user.User2.Id == myId).Single();
                    following = me.Friends.Where(user => user.User1.Id == id && user.User2.Id == myId).Single();
                    follower = otherusr.Friends.Where(user => user.User1.Id == myId && user.User2.Id == id).Single();

                    following.status = true;
                    follower.status = true;

                   
                    otherusr.notifications.Add( new Notification
                    {
                        message = "accepted your friend request",
                        idReceiver = otherusr.Id,
                        NotificationPath = "profile",
                        idSender = me.Id,

                    });
                    otherusr.newNotifications++;
                    _db.SaveChanges();
                    return new JsonResult(new { status = "true", message = " success" });
                }
                catch (Exception)
                { }
                return new JsonResult(new { status = "false", message = "You have no follow request from this user" });

            }
            return new JsonResult(new { status = "false", message = "token invalid" });
        }

        private async Task sendNotifications(long id, long noNotification)
        {

            HashSet<string> conections = ConectionMapping.Instance.Find(id);
            if (conections == null)
            {
                return;
            }
            foreach (string conn in conections)
            {
                await _notification.Clients.Client(conn).SendAsync("NewNotificationReceived",noNotification);
            }
        }

        [HttpGet("getFriend")]
        public async Task<ActionResult> getFriends(long id, int pagesize, int pagenumber)
        {
            var usr=_db.Users.Include(user => user.Friends).ThenInclude(usr => usr.User1).ThenInclude(user=> user.ProfilePic).Where(user => user.Id == id).FirstOrDefault();
            if (usr == null)
            {
                return new JsonResult(new { status=false, message="user not found"});
            }
            var firends = usr.Friends.Where(user => user.User1.Id != id)
                .OrderBy(user => user.User1.FullName)
                .Skip(pagesize * (pagenumber-1))
                .Take(pagesize);
            List<ProfileModel> profiles = new List<ProfileModel>();
            profiles = firends.Select(c => new ProfileModel
            {
                Id = c.User1.Id,
                FullName = c.User1.FullName,
                ImgUrl = "https://localhost:44355/images/" + c.User1.ProfilePic.ImgUrl,
                friendStatus= FollowType.Friends,

            }).ToList();

            return new JsonResult(new {status= true, profiles=profiles });

        }


    }

}
