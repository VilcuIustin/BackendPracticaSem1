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
                    .Include(user => user.ProfilePic)
                    .Where(user => id == user.Id)
                    .Single();

                long noFollowers, noFollowing;
                var aux = user.Followers.Where(follower => follower.status == true);
                noFollowers = aux.Count();
                aux = user.Following.Where(follower => follower.status == true);
                noFollowing = aux.Count();
                if (user.ProfilePic == null)
                {
                    return new JsonResult(new
                    {
                        status = "true",
                        username = user.FirstName + " " + user.LastName,
                        nrPosts = user.MyPosts.Count(),
                        nrFollowers = noFollowers,
                        nrFollowing = noFollowing,
                    });
                }
                return new JsonResult(new
                {
                    status = "true",
                    username = user.FirstName + " " + user.LastName,
                    nrPosts = user.MyPosts.Count(),
                    nrFollowers = noFollowers,
                    nrFollowing = noFollowing,
                    profileImg = "https://localhost:44355/images/" + user.ProfilePic.ImgUrl
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

        [HttpPost("updatephoto")]
        public async Task<IActionResult> modifyProfileImg([FromForm] IFormFileCollection profilePicture)
        {
            var curentUser = HttpContext.User;
            if (curentUser.HasClaim(claim => claim.Type == "Id"))
            {
                long idMe;

                bool rez = long.TryParse(curentUser.FindFirst(claim => claim.Type == "Id").Value, out idMe);
                if (!rez)
                    return new JsonResult(new { status = "false", message = "Can't parse the id" });
                var me = _db.Users.Find(idMe);
                if (me == null)
                {
                    return new JsonResult(new { status = "false", message = "Can't find you" });
                }
                if (profilePicture.Count != 1)
                    return new JsonResult(new { status = "false", message = "Too many pictures for one profile image" });
                var imgurl = uploadImages(profilePicture);
                if (imgurl.Result.paths.Count != 1)
                    return new JsonResult(new { status = "false", message = "Sorry but there is a problem " + imgurl.Result.errors.ElementAt(0) });
                me.ProfilePic = new ImgURL
                {
                    ImgUrl = imgurl.Result.paths.ElementAt(0),
                };
                _db.SaveChanges();
                return new JsonResult(new { status = "true", message = "Profile picture updated!" });
            }
            return new JsonResult(new { status = "false", message = "Token is invalid" });

        }


        #region Post


        [HttpPost("post")]
        public async Task<ActionResult<Post>> addPost([FromForm] PostPayload postPayload)
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

                    ICollection<ImgURL> imagesToAdd = new List<ImgURL>();


                    if (postPayload.Photos != null)
                    {
                        var uploadImgResult = await uploadImages(postPayload.Photos);
                        if (uploadImgResult.errors == null && uploadImgResult.paths == null)
                        {
                            return new JsonResult(new { status = "false", message = "Our server used for scanning photos for viruses is down" });
                        }

                        foreach (string path in uploadImgResult.paths)
                        {
                            imagesToAdd.Add(new ImgURL { ImgUrl = path });
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
                        DTPost = DateTime.Now,
                        Images = imagesToAdd
                    };



                    _db.Posts.Add(post);
                    await _db.SaveChangesAsync();
                    user.MyPosts.Add(new PostId
                    {
                        postId = post.Id,
                        userId = user.Id
                    });

                    await _db.SaveChangesAsync();
                    return new JsonResult(new { status = "true", post = post });
                }
                catch (Exception ex)
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
                    if (ex.HResult == -2147467259)    //error for server down 
                    {
                        return new UploadImgResult();
                    }
                    errorList.Add(" Scanner exception " + ex.Message);
                    images.ToList<IFormFile>().Remove(file);

                }
            }

            foreach (IFormFile file in images)
            {
                string path = Path.Combine(Path.GetRandomFileName());
                path += System.IO.Path.GetExtension(file.FileName);
                using (var stream = System.IO.File.Create(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Images", path)))
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
                User user1;
                try
                {
                    user1 = _db.Users.Where(user => user.Id == idUser).Include(following => following.Following).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    return new JsonResult(new { status = false, message = ex.Message });

                }

                var user = await _db.Users.Where(user => user.Id == id).Include(post => post.MyPosts).SingleOrDefaultAsync();
                if (user == null)
                    return new JsonResult(new { status = "false", message = "User not found" });
                if (idUser == id)
                {
                    //  List<Post> posts = new List<Post>(user.MyPosts.Reverse().Skip((pageNumber - 1) * pageSize)
                    //         .Take(pageSize));
                    List<Post> posts = new List<Post>();
                    var aux = user.MyPosts.Reverse().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    foreach (var ids in aux)
                        posts.Add(_db.Posts.Where(post => post.Id == ids.postId).Include(post => post.Images).Single());

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
                    foreach (var ids in user.MyPosts.Reverse().Skip((pageNumber - 1) * pageSize).Take(pageSize))
                        posts.Add(_db.Posts.Where(post => post.Id == ids.postId).Include(post => post.Images).Single());

                    return posts;
                }


            }
            return BadRequest();

        }


        #endregion

    }
}
