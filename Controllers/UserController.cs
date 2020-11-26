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
        public async Task<ActionResult<List<Post>>> GetUserById([FromRoute] int id ) 
        {
            try
            {
                return _db.Users.Where(user => id == user.Id).Single().MyPosts.ToList();
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
                    return _db.Users.ToList();
            }
            return new JsonResult(new { status = "false" });
        }



        [HttpGet("getfallow")]
        public async Task<ActionResult<User>> GetFallow([FromBody] PostPayload postPayload)
        {

            return Ok();        // de implementat

        }




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
                    var user = _db.Users.Where(user => user.Id == postPayload.UserId).Single();
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
                    // await TryUpdateModelAsync<User>(user, "", a => user.MyPosts.Add());
                    if (user.MyPosts == null)
                        user.MyPosts = new List<Post>();
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

       // [HttpGet("getpost")]
       // public async Task<ActionResult> GetPost([FromBody] )

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
            return Ok();
        }












    }
}
