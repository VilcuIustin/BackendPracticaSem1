using Backend.Entities;
using Backend.Entities.Models;
using Backend.Payloads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using nClam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BackendContext _db;
        private readonly IConfiguration _configuration;

        public UserController(BackendContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [HttpGet("getuser/{id}")]
        public async Task<ActionResult<User>> GetUserById([FromRoute] long id)
        {
            try
            {
                var user = _db.Users
                    .Include(user => user.MyPosts)
                    .Include(user => user.Followers)
                    .Include(user => user.Following)
                    .Where(user => id == user.Id)
                    .Single();
                return new JsonResult(new
                {
                    status = "true",
                    username = user.FirstName + " " + user.LastName,
                    nrPosts = user.MyPosts.Count(),
                    nrFollowers = user.Followers.Count(),
                    nrFollowing = user.Following.Count()            // trebuie sa adaug la return si poza de profil
                });

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
            if (currentUser.HasClaim(claims => claims.Type == "Role"))
            {
                if ((currentUser.Claims.FirstOrDefault(c => c.Type == "Role").Value) == "Admin")
                    return _db.Users
                        .Include(user => user.MyPosts).ToList();
            }
            return new JsonResult(new { status = "false" });
        }

        #region Fallow

        [HttpGet("getfollowers/{id}")]
        public async Task<ActionResult<List<UserId>>> GetFallowers([FromRoute] long id)
        {
            return _db.Users.Include(user => user.Followers).Where(user => user.Id == id).Single().Followers.ToList();

        }
        [HttpGet("getfollowing/{id}")]
        public async Task<ActionResult<List<UserId>>> GetFallowing([FromRoute] long id)
        {
            return _db.Users.Include(user => user.Following).Where(user => user.Id == id).Single().Followers.ToList();

        }




        [HttpPost("fallow/{id}")]
        public async Task<ActionResult> Fallow([FromRoute] long id)     //trebuie rescris pentru SignalR
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(claims => claims.Type == "Id"))
            {
                long idUser;
                bool re = long.TryParse((currentUser.Claims.FirstOrDefault(c => c.Type == "Id").Value), out idUser);
                if (!re)
                    return BadRequest("Can't parse id");
                var userForFollowing = _db.Users.Include(user => user.Following).Where(user => user.Id == idUser).Single();
                try
                {
                    var userForFollow = _db.Users.Include(user => user.Followers).Where(user => user.Id == id).Single();
                 //   userForFollowing.Following.Add(userForFollow);
                 //   userForFollow.Followers.Add(userForFollowing);
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
        public async Task<ActionResult<Post>> addPost( [FromForm] PostPayload postPayload)
        {


            if (postPayload.Photos == null && String.IsNullOrEmpty(postPayload.Text))
            {
                return new JsonResult(new { status = "false", message = "post is empty" });
            }
            else
            {
                try
                {
                    var user = _db.Users.Include(user => user.MyPosts).Where(user => user.Id == postPayload.UserId).Single();
                   
                    ICollection<ImgURL> imagesToAdd=new List<ImgURL>();


                    if (postPayload.Photos != null)
                    {
                        var uploadImgResult = await uploadImages(postPayload.Photos);
                        if(uploadImgResult.errors==null && uploadImgResult.paths == null)
                        {
                            return new JsonResult(new { status = "false", message = "Our server used for scanning photos for viruses is down" });
                        }

                        foreach (string path in uploadImgResult.paths)
                        {
                            imagesToAdd.Add(new ImgURL { ImgUrl=path });
                        }
                    }
                    else
                    {
                        imagesToAdd = null;
                    }
                    var post = new Post
                    {
                        IdUser = postPayload.UserId,
                        Text = postPayload.Text,
                        Images = imagesToAdd
                    };


                  
                    _db.Posts.Add(post);
                    
                    user.MyPosts.Add(new PostId
                    {
                        postId = post.Id,
                        userId= user.Id
                    });
                    
                    await _db.SaveChangesAsync();
                    return new JsonResult(new { status = "true", post = post });
                }
                catch (Exception)
                {
                    return new JsonResult(new { status = "false", message = "user not found" });
                }
            }
        }


        public async Task<UploadImgResult> uploadImages(IFormFileCollection images)
        {
            List<string> filesPath = new List<string>();
            List<string> errorList = new List<string>();

            foreach (IFormFile file in images)
            {
                var ms = new MemoryStream();
                var clam = new ClamClient(this._configuration["ClamAVServer:URL"],
                                  Convert.ToInt32(this._configuration["ClamAVServer:Port"]));
                file.OpenReadStream().CopyTo(ms);
                byte[] fileBytes = ms.ToArray();
                try
                {
                    var scanResult = await clam.SendAndScanFileAsync(fileBytes);
                    switch (scanResult.Result)
                    {
                        case ClamScanResults.Clean:
                            errorList.Add("");
                            break;
                        case ClamScanResults.VirusDetected:
                            errorList.Add("virus detected");
                            images.ToList<IFormFile>().Remove(file);
                            break;
                        default:
                            errorList.Add("unknown error");
                            images.ToList<IFormFile>().Remove(file);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if(ex.HResult== -2147467259)    //error for server down 
                    {
                        return new UploadImgResult();
                    }
                    errorList.Add(" Scanner exception " + ex.Message);
                    images.ToList<IFormFile>().Remove(file);

                }
            }

            foreach (IFormFile file in images)
            {
                string path = Path.Combine("D:\\imagesServer", Path.GetRandomFileName());
                path +=  System.IO.Path.GetExtension(file.FileName);
                using (var stream = System.IO.File.Create(path))
                {
                    await file.CopyToAsync(stream);
                    filesPath.Add(path);
                }
            }
            return new UploadImgResult { paths = filesPath, errors = errorList };

        }
        [HttpPut("editpost")]
        //Needs to update photos
        public async Task<ActionResult<Post>> EditPost([FromBody] PostPayload postPayload)  
        {
            Post postToEdit;
            try
            {
                postToEdit = _db.Posts.Where(postare => postare.Id == postPayload.Id).Single();
            }
            catch (Exception)
            {
                return new JsonResult(new { status = "false", message = "post does not exist" });
            }
            if (postPayload.UserId == postToEdit.IdUser)
            {
                if (postPayload.Text == null && postPayload.Photos == null)
                {
                    return new BadRequestResult();
                }
                else
                {
                    //postToEdit.Images = postPayload.Photos;
                    postToEdit.Text = postPayload.Text;
                    await _db.SaveChangesAsync();
                    return new JsonResult(new { status = "true" });
                }
            }
            else
            {
                if (_db.Users.Where(user => user.Id == postPayload.UserId).Single().Role == "admin")
                {
                 //   postToEdit.Images = postPayload.Photos;
                    postToEdit.Text = postPayload.Text;
                    await _db.SaveChangesAsync();
                    return new JsonResult(new { status = "true" });
                }
                else
                {
                    return new JsonResult(new { status = "false", message = "you are not the post creator or an admin" });
                }
            }

        }

        [HttpGet("getpost")]
        public async Task<ActionResult<ICollection<Post>>> getPosts(long id, int pageSize, int pageNumber)
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(claims => claims.Type == "Id"))
            {
                long idUser;
                long.TryParse(currentUser.Claims.FirstOrDefault(c => c.Type == "Id").Value, out idUser);
                var user1 = _db.Users.Where(user => user.Id == idUser).Include(following => following.Following).FirstOrDefault();
                var user = await _db.Users.Where(user => user.Id == id).Include(post => post.MyPosts).SingleOrDefaultAsync();
                if (user == null)
                    return new JsonResult(new { status = "false", message = "User not found" });
                if (idUser == id)
                {
                    //  List<Post> posts = new List<Post>(user.MyPosts.Reverse().Skip((pageNumber - 1) * pageSize)
                    //         .Take(pageSize));
                    List<Post> posts = new List<Post>();
                    foreach (var ids in user.MyPosts.Reverse())
                       posts.Add( _db.Posts.Where(post => post.Id == ids.postId).Single());

                    return posts;
                }
                var userFoud = user1.Following.Where(following => following.following == user.Id).FirstOrDefault();
                if (userFoud == null)
                {
                    return new JsonResult(new { status = "false", message = "This profile is private. " });
                }
                else
                {
                    List<Post> posts = new List<Post>();
                    foreach (var ids in user.MyPosts.Reverse())
                       posts.Add( _db.Posts.Where(post => post.Id == ids.postId).Single());

                    return posts;
                }


            }
            return BadRequest();

        }


        #endregion

    }
}
