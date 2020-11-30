using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Entities;
using Backend.Entities.Models;
using Backend.Payloads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BackendContext _db;
        public UserController(BackendContext db)
        {
            _db = db;       
        }

        [HttpGet("getuser/{id}")]
        public async Task<ActionResult<User>> GetUserById([FromRoute] long id ) 
        {
            try
            {
                return _db.Users
                    .Include(user => user.MyPosts)
                    .Include(user => user.Followers)
                    .Include(user => user.Following)
                    .Where(user => id == user.Id)
                    .Single();
              
            }
            catch (Exception)
            {
                return new JsonResult(new { status = "false", message = "user id not found" });
            } 
        }
        [HttpGet("allUsers")]
        public async Task<ActionResult<List<User>>> AllUsers()
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(claims=> claims.Type=="Role"))
            {
                if((currentUser.Claims.FirstOrDefault(c => c.Type == "Role").Value)=="Admin")
                    return _db.Users
                        .Include(user => user.MyPosts).ToList();
            }
            return new JsonResult(new { status = "false" });
        }

        #region Fallow

        [HttpGet("getfollowers/{id}")]
        public async Task<ActionResult<List<User>>> GetFallowers([FromRoute] long id)
        {
            return _db.Users.Include(user => user.Followers).Where(user => user.Id == id).Single().Followers.ToList();

        }
        [HttpGet("getfollowing/{id}")]
        public async Task<ActionResult<List<User>>> GetFallowing([FromRoute] long id)
        {
            return _db.Users.Include(user => user.Following).Where(user => user.Id == id).Single().Followers.ToList();

        }




        [HttpPost("fallow/{id}")]
        public async Task<ActionResult> Fallow([FromRoute] long id)
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(claims => claims.Type == "Id"))
            {
                long idUser;
                bool re = long.TryParse((currentUser.Claims.FirstOrDefault(c => c.Type == "Id").Value),out idUser);
                if (!re)
                    return BadRequest("Can't parse id");
                var userForFollowing = _db.Users.Include(user => user.Following).Where(user => user.Id == idUser).Single();
                try
                {
                    var userForFollow = _db.Users.Include(user => user.Followers).Where(user => user.Id == id).Single();
                    userForFollowing.Following.Add(userForFollow);
                    userForFollow.Followers.Add(userForFollowing);
                    await _db.SaveChangesAsync();
                }
                catch (InvalidOperationException)
                {
                    return new JsonResult(new { status = "false", message = "user not found" });
                }
                
                return Ok();
            }

            return BadRequest("Id not found");
        }

        #endregion

        #region Post


        [HttpPost("post")]
        public async Task<ActionResult<Post>> addPost([FromBody] PostPayload postPayload)
        {

            if (postPayload.photo == null && String.IsNullOrEmpty(postPayload.Text))
            {
                return new JsonResult(new { status = "false", message = "post is empty" });
            }
            else
            {
                try
                {
                    var user = _db.Users.Include(user=> user.MyPosts).Where(user => user.Id == postPayload.UserId).Single();
                    var usertoUpdate = user;
                    Image photoToAdd = null;
                    if (postPayload.photo.Photo != null)
                    {
                       photoToAdd  = new Image
                        {
                            Photo = postPayload.photo.Photo
                        };
                        _db.Images.Add(photoToAdd);
                    }                
                    var post = new Post
                    {
                        IdUser= postPayload.UserId,
                        Text = postPayload.Text,
                        Img = photoToAdd
                    };
                    
                    
                    user.MyPosts.Add(post);
                    _db.Entry<User>(usertoUpdate).CurrentValues.SetValues(user);

                    _db.Posts.Add(post);
                    await _db.SaveChangesAsync();
                    return new JsonResult(new { status = "true", post = post });
                }
                catch (Exception)
                {
                    return new JsonResult(new { status = "false", message = "user not found" });
                }        
            }
        }

        [HttpPut("editpost")]
        public async Task<ActionResult<Post>> EditPost([FromBody] PostPayload postPayload)
        {
            Post postToEdit;
            try
            {
                postToEdit = _db.Posts.Where(postare => postare.Id == postPayload.Id).Single();
            }
            catch (Exception)
            {
                return new JsonResult(new { stauts = "false", message = "post does not exist" });       
            }
            if (postPayload.UserId == postToEdit.IdUser)
            {
                if (postPayload.Text == null && postPayload.photo == null)
                {
                    return new BadRequestResult();
                }
                else
                {
                    postToEdit.Img = postPayload.photo;
                    postToEdit.Text = postPayload.Text;
                    await _db.SaveChangesAsync();
                    return new JsonResult(new { stauts = "true" });
                }
            }
            else
            {
                if(_db.Users.Where(user => user.Id == postPayload.UserId).Single().Role == "admin")
                {
                    postToEdit.Img = postPayload.photo;
                    postToEdit.Text = postPayload.Text;
                    await _db.SaveChangesAsync();
                    return new JsonResult(new { stauts = "true" });
                }
                else
                {
                     return new JsonResult(new { stauts = "false", message = "you are not the post creator or an admin" });
                }
            }
            
        }

        #endregion

    }
}
