﻿using Backend.Entities;
using Backend.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : Controller
    {
        private readonly BackendContext _db;

        public FollowController(BackendContext db)
        {
            _db = db;
        }

        [HttpGet("getfollowers/{id}")]              //0 - the same user.     1 - not followed   2 - your request is in pending 
                                                    //  3 - you have a request in pending 4 - fallowed -1 - not followed
        public async Task<ActionResult<int>> GetFallowers([FromRoute] long id)
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
                    return 0;
                }
                User me;
                User otherusr;
                try
                {
                    me = _db.Users.Include(user => user.Following)
                        .Include(user => user.Followers)
                        .Where(user => user.Id == myId).Single();
                    otherusr = _db.Users.Include(user => user.Followers)
                        .Include(user => user.Following)
                        .Where(user => user.Id == id).Single();
                }
                catch (ArgumentNullException)
                {
                    return BadRequest("User not found");

                }

                UserId following = null;
                UserId follower = null;
                UserId following2 = null;
                UserId follower2 = null;
                try
                {
                    following = me.Following.Where(user => user.following == id).Single();
                    follower = otherusr.Followers.Where(user => user.followedBy == myId).Single();
                    if (following != null && follower != null)
                    {
                        if (following.followedBy == myId)
                            return 2;
                        else if (following.followedBy == id)
                            return 3;
                        else if (following.status == true)
                            return 4;
                        return 1;
                    }
                }
                catch (Exception)
                { }
                try
                {
                    follower2 = me.Followers.Where(user => user.followedBy == id).Single();
                    following2 = otherusr.Following.Where(user => user.following == myId).Single();
                    if (following2 != null && follower2 != null)
                    {
                        if (following2.followedBy == myId)
                            return 2;
                        else if (following2.followedBy == id)
                            return 3;
                        else if (following2.status == true)
                            return 4;
                    }
                    return 1;
                }
                catch (Exception)
                { }

                return 1;
              
            }
            return BadRequest("Your token is invalid");
          

        }


        [HttpPost("followreq")]
        public async Task<ActionResult> Follow([FromBody] long id)     //trebuie rescris pentru SignalR
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(claims => claims.Type == "Id"))
            {
                long idUser;
                bool re = long.TryParse((currentUser.Claims.FirstOrDefault(c => c.Type == "Id").Value), out idUser);
                if (!re)
                    return BadRequest("Can't parse id");
                var me = _db.Users.Include(user => user.Following).Where(user => user.Id == idUser).Single();
                try
                {
                    var userForFollow = _db.Users.Include(user => user.Followers).Where(user => user.Id == id).Single();

                    me.Following.Add(new UserId
                    {
                        followedBy = idUser,
                        following = id,
                        status = false
                    });
                    userForFollow.Followers.Add(new UserId
                    {
                        followedBy = idUser,
                        following = id,
                        status = false
                    });



                    await _db.SaveChangesAsync();
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
        public async Task<IActionResult> unfallow( long id)
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
                    me = _db.Users.Include(user => user.Following)
                        .Include(user => user.Followers)
                        .Where(user => user.Id == myId).Single();
                    otherusr = _db.Users.Include(user => user.Followers)
                        .Include(user => user.Following)
                        .Where(user => user.Id == id).Single();
                }
                catch (ArgumentNullException)
                {
                    return BadRequest("User not found");

                }

                UserId following = null;
                UserId follower = null;
                UserId following2 = null;
                UserId follower2 = null;
                try
                {
                    following = me.Following.Where(user => user.following == id).Single();
                    follower = otherusr.Followers.Where(user => user.followedBy == myId).Single();
                    me.Following.Remove(following);
                    otherusr.Followers.Remove(follower);
                    _db.SaveChanges();
                    return new JsonResult( new { status ="true"});
                }
                catch (Exception)
                { }
                try
                {
                    follower2 = me.Followers.Where(user => user.followedBy == id).Single();
                    following2 = otherusr.Following.Where(user => user.following == myId).Single();
                    me.Followers.Remove(follower2);
                    otherusr.Following.Remove(following2);
                    _db.SaveChanges();
                    return new JsonResult(new { status = "true" });
                }
                catch (Exception)
                { }
              

            }
            return new JsonResult(new { status = "false", message = "token invalide" });
        }
    }
}