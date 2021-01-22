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
using static Backend.Enums;

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

        [HttpGet("getName/{id}")]
        public async Task<ActionResult> getName( long id)
        {
            var user = _db.Users.FirstOrDefault(user => user.Id == id);
            if (user == null)
                return new JsonResult(new { status = false, message="user not found" });
            return new JsonResult(new { status = true, firstname = user.FirstName, lastname = user.LastName });
        } 


        [HttpGet("getuser/{id}")]
        public async Task<ActionResult<User>> GetUserById([FromRoute] long id)
        {
            try
            {
                var user = _db.Users
                    .Include(user => user.MyPosts)
                    .Include(user => user.Friends)
                    .Include(user => user.ProfilePic)
                    .Where(user => id == user.Id)
                    .Single();

                long noFollowers;
                var aux = user.Friends.Where(follower => follower.status == true);
                noFollowers = aux.Count();

                if (user.ProfilePic == null)
                {
                    return new JsonResult(new
                    {
                        status = "true",
                        username = user.FirstName + " " + user.LastName,
                        nrPosts = user.MyPosts.Count(),
                        nrFollowers = noFollowers,
                       

                    });
                }
                return new JsonResult(new
                {
                    status = "true",
                    username = user.FirstName + " " + user.LastName,
                    nrPosts = user.MyPosts.Count(),
                    nrFollowers = noFollowers,
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
                        .Include(user => user.MyPosts)
                        .Include(user => user.Friends)
                        .ToList();
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
                if (imgurl.Result.paths == null || imgurl.Result.paths.Count != 1)
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

        [HttpGet("search")]
        public async Task<ActionResult> search(string name, int pageSize, int pageNumber)
        {
            var curentUser = HttpContext.User;
            if (curentUser.HasClaim(claim => claim.Type == "Id"))
            {
                long idMe;

                bool rez = long.TryParse(curentUser.FindFirst(claim => claim.Type == "Id").Value, out idMe);
                if (!rez)
                    return new JsonResult(new { status = "false", message = "Can't parse the id" });


                var acc = _db.Users.Include(user => user.ProfilePic).Where(user => user.FullName.Contains(name)).Skip((pageNumber - 1) * pageSize).Take(pageSize);
                List<ProfileModel> profiles = new List<ProfileModel>();
                profiles = acc.Select(c => new ProfileModel
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    ImgUrl = "https://localhost:44355/images/" + c.ProfilePic.ImgUrl,

                }).ToList();
                foreach (var profile in profiles)
                {
                    //FriendStatus.Add(await areFriends(idMe, profile.Id));
                    var status=await areFriends(idMe, profile.Id);
                    profile.friendStatus = status;
                }
                
                List<Enums.FollowType> FriendStatus = new List<Enums.FollowType>();
                
                return new JsonResult(new { status = true, result = profiles });
            }
            return new JsonResult(new { status = false, result = "id not found" });
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

        [HttpGet("getProfilePic")]
        public async Task<ActionResult> getPhoto()
        {
            long id = -1;
            var userContext = HttpContext.User;
            if (userContext.HasClaim(claim => claim.Type == "Id"))
            {
                if (!long.TryParse(userContext.Claims.FirstOrDefault(claim => claim.Type == "Id").Value, out id))
                    return new JsonResult(new { status = false, message = "token invalid" });
                try
                {
                    var user = _db.Users.Include(user => user.ProfilePic).Where(user => user.Id == id).Single();
                    return new JsonResult(new { status = true, image = user.ProfilePic.ImgUrl });
                }
                catch (Exception ex)
                {
                    return new JsonResult(new { status = false, message = "user not found" });

                }
            }
            return new JsonResult(new { status = false, message = "token invalid" });



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
        public async Task<ActionResult> getPosts(long id, int pageSize, int pageNumber)
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(claims => claims.Type == "Id"))
            {
                long idUser;
                long.TryParse(currentUser.Claims.FirstOrDefault(c => c.Type == "Id").Value, out idUser);
                User user1;
                try
                {
                    user1 = _db.Users.Where(user => user.Id == idUser).Include(following => following.Friends).FirstOrDefault();
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
                    List<bool> liked = new List<bool>();
                    foreach (var ids in aux)
                    {
                      
                        var po = _db.Posts.Where(post => post.Id == ids.postId).Include(post=> post.UserLiked).Include(post => post.Images);
                        Post p = po.Single();
                        posts.Add(p);
                        
                        if ((p.UserLiked.Where(usr=> usr.Id== idUser).FirstOrDefault() ) != null)
                        {
                            liked.Add(true);
                        }
                        else
                        {
                            liked.Add(false);
                        }
                    }

                    return new JsonResult(new { status = "true", posts = posts, nrPost = user.MyPosts.Count(), liked= liked });

                }
                var friends = await areFriends(user1.Id, user.Id);
                if (friends == FollowType.NotFriends || friends==FollowType.Pending || friends == FollowType.Wait)
                {
                    return new JsonResult(new { status = "false", message = "This profile is private. " });
                }
                else
                {
                    List<Post> posts = new List<Post>();
                    List<bool> liked = new List<bool>();
                    foreach (var ids in user.MyPosts.Reverse().Skip((pageNumber - 1) * pageSize).Take(pageSize))
                    {
                        var aux = _db.Posts.Where(post => post.Id == ids.postId).Include(post => post.Images);
                        var post= aux.Single();
                        posts.Add(post);
                        if(aux.Include(post => post.UserLiked).Where(usr => usr.IdUser == idUser).FirstOrDefault()!=null)
                        {
                            liked.Add(true);
                        }
                        else
                        {
                            liked.Add(false);
                        }

                    }
                        

                    return new JsonResult(new { status = "true", posts = posts, nrPost = user.MyPosts.Count() });

                }


            }
            return BadRequest();

        }

        [HttpPatch("like")]
        public async Task<ActionResult> Like([FromBody]IdPayload idPost)
        {
            long id;
            if (HttpContext.User.HasClaim(claim => claim.Type == "Id"))
            {
                if (!long.TryParse(HttpContext.User.Claims.First(claim => claim.Type == "Id").Value, out id))
                {
                    return new JsonResult(new { status = false, message = "Can't parse id" });
                }
                var userPost = _db.Posts.Where(post => post.Id == idPost.id).Include(post => post.UserLiked).FirstOrDefault();
                if (userPost == null)
                {
                    return new JsonResult(new { status = false, message = "Can't find this post" });
                }

                var friendStatus = await areFriends(id, userPost.IdUser);
                switch (friendStatus)
                {
                    case FollowType.UserNotFound:

                        return new JsonResult(new { status = false, message = "one of the users not found" });
                    case FollowType.NotFriends:
                    case FollowType.Pending:
                        return new JsonResult(new { status = false, message = "you can't add a like to a post that you can't see it" });
                    case FollowType.Friends:
                    case FollowType.Same:
                        var userFound = userPost.UserLiked.Where(user => user.Id == id).SingleOrDefault();
                        var me = _db.Users.Where(usr => usr.Id == id).SingleOrDefault();
                        if (userFound == null)
                        {
                            userPost.UserLiked.Add(me);
                            userPost.NrLikes++;
                        }
                        else
                        {
                            userPost.UserLiked.Remove(me);
                            userPost.NrLikes--;
                        }
                        break;
                }
                _db.SaveChanges();
                return Ok();
            }
            return new JsonResult(new { status = false, message = "bad token" });

        }



        public async Task<Enums.FollowType> areFriends(long id1, long id2)
        {
            if (id1 == id2)
            {
                return FollowType.Same;
            }
            try
            {
                User user1 = _db.Users.Include(user => user.Friends)
                  .Where(user => user.Id == id1).Single();
                User user2 = _db.Users.Include(user => user.Friends)
                    .Where(user => user.Id == id2).Single();
                
                Friend? a1 = null;

             
                a1 = user1.Friends.Where(user => user.User1.Id == id2 && user.User2.Id == id1).FirstOrDefault();
                if(a1!=null && a1?.status==false )
                {
                    if (a1.sended == true)
                    {
                        return FollowType.Pending;
                    }
                    else
                        return FollowType.Wait;
                }
                if(a1 != null && a1?.status == true)
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

        #endregion
        #region comment
        [HttpPost("addComm")]
        public async Task<ActionResult> addComment([FromForm] CommentPayload payload)
        {
            if (payload.Image == null && payload.Message == null)
            {
                return new JsonResult(new { status = false, message = " you can't add a comment that is empty" });
            }
            var userContext = HttpContext.User;
            if (userContext.HasClaim(claim => claim.Type == "Id"))
            {
                long id = -1;
                if (!long.TryParse(userContext.Claims.First(claim => claim.Type == "Id").Value, out id))
                {
                    return new JsonResult(new { status = false, message = "parse id failed" });
                }
                Post post;
                try
                {
                    post = _db.Posts.Include(post => post.PostComment).ThenInclude(comm => comm.SubComment).Where(post => post.Id == payload.postId).Single();

                }
                catch (Exception ex)
                {
                    return new JsonResult(new { status = false, message = "post not found" });

                }
                try
                {
                    _db.Users.Where(user => user.Id == id).Single();

                }
                catch (Exception)
                {

                    return new JsonResult(new { status = false, message = "you are not in database" });
                }
                if (payload.Message == null && payload.Image == null)
                    return new JsonResult(new { status = false, message = " comment is empty" });

                var rez = await areFriends(id, post.IdUser);
                switch (rez)
                {
                    case FollowType.UserNotFound:

                        return new JsonResult(new { status = false, message = "one of the users not found" });
                        break;
                    case FollowType.NotFriends:
                    case FollowType.Pending:
                        return new JsonResult(new { status = false, message = "you can't add a comment to a post that you can't see it" });
                        break;
                    case FollowType.Friends:
                    case FollowType.Same:
                        string link = null;
                        ImgURL aux = null;
                        if (payload.Image != null)
                        {
                            var uploadImgResult = await uploadImages(payload.Image);
                            if (uploadImgResult.errors == null && uploadImgResult.paths == null)
                            {
                                return new JsonResult(new { status = "false", message = "Our server used for scanning photos for viruses is down" });
                            }
                            link = uploadImgResult.paths.ElementAt(0);
                            aux = new ImgURL
                            {
                                ImgUrl = link,
                            };
                        }

                        if (payload.CommentId == null)
                        {
                            post.nrComm++;
                            post.PostComment.Add(new Comment
                            {
                                Image = aux,
                                Message = payload.Message,
                                UserId = id,
                            });
                        }
                        else
                        {
                            Comment comm;
                            try
                            {
                                comm = post.PostComment.Where(post => post.Id == payload.CommentId).Single();

                                comm.SubComment.Add(new Comment
                                {
                                    Image = aux,
                                    Message = payload.Message,
                                    UserId = id,
                                });
                            }
                            catch (Exception ex)
                            {

                                return new JsonResult(new { status = false, message = "Comment not found" });
                            }
                        }
                        await _db.SaveChangesAsync();
                        return new JsonResult(new { status = true, message = "comment added" });
                        break;
                }
            }
            return new JsonResult(new { status = false, message = " token invalid" });
        }


        [HttpGet("getComm")]
        public async Task<ActionResult> getComm(long postId, int pageSize, int pageNumber, int mod, long commId)    // mod==1 for comments and mod==2 for subcoments
        {
            if (HttpContext.User.HasClaim(claim => claim.Type == "Id"))
            {
                long id = -1;
                if (!long.TryParse(HttpContext.User.Claims.First(claim => claim.Type == "Id").Value, out id))
                {
                    return new JsonResult(new { status = false, message = "can't get your id" });
                }
                //return new JsonResult(new { nush = _db.Posts.ToList() });
                Post post;
                try
                {
                    post = _db.Posts.Where(p => p.Id == postId)
                        .Include(p => p.PostComment)
                        .ThenInclude(p => p.Image)
                        .Include(p => p.PostComment)
                        .ThenInclude(p => p.SubComment)
                        .ThenInclude(p => p.Image)
                        .Single();
                }
                catch (Exception ex)
                {
                    return new JsonResult(new { status = false, message = "post not found" });
                }
                var rez = await areFriends(id,post.IdUser );


                switch (rez)
                {
                    case FollowType.UserNotFound:

                        return new JsonResult(new { status = false, message = "one of the users not found" });
                        break;
                    case FollowType.NotFriends:
                    case FollowType.Pending:
                        return new JsonResult(new { status = false, message = "you can't get a comment to a post that you can't see it" });
                        break;
                    case FollowType.Friends:
                    case FollowType.Same:
                        if (mod == 1)
                        {
                            var comments = post.PostComment.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                            List<(string, string)> usersDet = new List<(string, string)>();
                            foreach (var aux in comments)
                            {
                                try
                                {
                                    var userAux = _db.Users.Include(usr => usr.ProfilePic).Where(usr => usr.Id == aux.UserId).Single();
                                    usersDet.Add((userAux.FirstName + " " + userAux.LastName, userAux.ProfilePic.ImgUrl));
                                }
                                catch (Exception)
                                {
                                    comments.Remove(aux);
                                }


                            }

                            return new JsonResult(new { status = true, comments = comments, detUsers = usersDet, nrComments = post.PostComment.Count() });
                        }
                        else if (mod == 2)
                        {
                            try
                            {
                                var comm = post.PostComment.Where(comm => comm.Id == commId).Single();
                                var subcomm = comm.SubComment.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                                List<(string, string)> usersDet = new List<(string, string)>();
                                foreach (var aux in subcomm)
                                {
                                    try
                                    {
                                        var userAux = _db.Users.Include(usr => usr.ProfilePic).Where(usr => usr.Id == aux.UserId).Single();
                                        usersDet.Add((userAux.FirstName + " " + userAux.LastName, userAux.ProfilePic.ImgUrl));
                                    }
                                    catch (Exception)
                                    {
                                        subcomm.Remove(aux);
                                    }


                                }
                                return new JsonResult(new { status = true, subcomments = subcomm, detUsers = usersDet, nrComments = post.PostComment.Count() });
                            }
                            catch (Exception)
                            {
                                return new JsonResult(new { status = false, message = "comment not found" });
                            }

                        }


                        break;
                }
                return new JsonResult(new { status = false, message = "Something went wrong" });

            }
            return new JsonResult(new { status = false, message = "token invalid" });
        }

        #endregion

        #region notification

        [HttpGet("notifications")]
        public async Task<ActionResult> getNotification(int pageNo, int pagesize)
        {

            if (HttpContext.User.HasClaim(claim => claim.Type == "Id"))
            {
                long id = -1;
                if (!long.TryParse(HttpContext.User.Claims.First(claim => claim.Type == "Id").Value, out id))
                {
                    return new JsonResult(new { status = false, message = "can't get your id" });
                }

                User me = _db.Users.Include(u => u.notifications).Where(u => u.Id == id).FirstOrDefault();
                if (me == null)
                {
                    return new StatusCodeResult(500);
                }
                var notifications= me.notifications.Skip(pageNo * pagesize).Take(pagesize).ToList();
                List<NotificationPayload> notif = new List<NotificationPayload>();
                
                foreach(var notification in notifications)
                {
                    if (notification.status)
                    {
                        me.newNotifications--;
                        notification.status = false;
                    }
                        

                    var aux = _db.Users.Include(u => u.ProfilePic).Where(u=> u.Id==notification.idSender).FirstOrDefault();
                    if (aux == null)
                    {
                        notif.Add(new NotificationPayload()
                        {
                            FullName = "Deleted User",
                            id = -1,
                            image= "unknown.jpg",
                            link = notification.NotificationPath,

                        }) ;
                    }
                    else
                    {
                        notif.Add(new NotificationPayload()
                        {
                            FullName = aux.FullName,
                            id = aux.Id,
                            image = aux.ProfilePic.ImgUrl,
                            link = notification.NotificationPath,

                        });
                    }
                }
                _db.SaveChanges();
                return new JsonResult(new {status=true, notifications = notif });
                

            }
            
            return BadRequest();

            }

        #endregion



    }
}
