using Backend.Entities;
using Backend.Entities.Models;
using Backend.Payloads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private IConfiguration _config { get; }
        private readonly BackendContext _db;

        public AccountController(BackendContext db, IConfiguration configuration)
        {
            _config = configuration;
            _db = db;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult<User> Register([FromBody] RegisterPayload registerPayload)
        {

            try
            {
                MailAddress m = new MailAddress(registerPayload.Email);
            }
            catch (Exception)
            {

                return new JsonResult(new { status = false, message = "email format " + registerPayload.Email });
            }


            try
            {
                var existingUserWithMail = _db.Users
            .Any(u => u.Email == registerPayload.Email);

                if (existingUserWithMail)
                {
                    return new JsonResult(new { status = false, message = "An account with this email already exists " });
                }
                string link;
                string gender;
                if (registerPayload.gender == "female")
                {
                    link = "female.jpg";
                    gender = "female";
                }

                else
                {
                    link = "man.jpg";
                    gender = "male";
                }
                   
                var userToCreate = new User
                {
                    Email = registerPayload.Email,
                    ProfilePic = new ImgURL { ImgUrl = link },
                    FirstName = registerPayload.FirstName,
                    LastName = registerPayload.LastName,
                    FullName= registerPayload.FirstName + " "+ registerPayload.LastName,
                    Gender= gender,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerPayload.Password),
                    Role = "SimpleUser",
                };

                _db.Users.Add(userToCreate);



                _db.SaveChanges();

                return Ok(new { status = true, user = userToCreate });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false, message = "" + ex.Message });
            }

        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginPayload loginPayload)
        {
            try
            {
                MailAddress m = new MailAddress(loginPayload.Email);
            }
            catch (FormatException)
            {
                return new JsonResult(new { status = "false", message = "email format" });
            }
            var foundUser = _db.Users
               .SingleOrDefault(u => u.Email == loginPayload.Email);

            if (foundUser != null)
            {
                if (BCrypt.Net.BCrypt.Verify(loginPayload.Password, foundUser.PasswordHash))
                {
                    var tokenString = GenerateJSONWebToken(foundUser);

                    return new JsonResult(new
                    {
                        status = "true",
                        Firstname = foundUser.FirstName,
                        Lastname = foundUser.LastName,
                        Id = foundUser.Id,
                        token = tokenString

                    });
                }
                return BadRequest(new { status = false, message = "Wrong password or email " });
            }
            else
            {
                return BadRequest(new { status = false, message = "No user with this email found" });
            }

        }

        private string GenerateJSONWebToken(User user)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Role", user.Role),
                new Claim("Id", user.Id.ToString()),
                };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddDays(30),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [Authorize]
        [HttpPatch("ChangeName")]
        public async Task<ActionResult> ChangeName([FromBody] ChangeNamePayload changename)
        {
            if(HttpContext.User.HasClaim(claim=> claim.Type == "Id"))
            {
                long id;
                if(!long.TryParse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Id").Value, out id))
                {
                    return Unauthorized("TOKEN INVALID PARSE ID FAILED");
                }
                var user = _db.Users.Include(usr => usr.Friends)
                        .ThenInclude(usr => usr.User1)
                        .Include(usr => usr.Friends)
                        .ThenInclude(usr=> usr.User2)
                    .FirstOrDefault();
                if (user == null)
                {
                    return Unauthorized("User Not Found");
                }

                if(user.FirstName== changename.Firstname && user.LastName== changename.Lastname)
                {
                    return Ok();
                }
                else
                {
                    
                    user.FirstName = changename.Firstname;
                    user.LastName = changename.Lastname;
                    user.FullName = changename.Firstname + " " + changename.Lastname;
                }
                await _db.Friends.Where(usr => usr.User1 == user).ForEachAsync(usr =>
                { usr.User1.FirstName = user.FirstName;
                    usr.User1.LastName = user.LastName;
                    usr.User1.FullName = user.FullName;
                });
                await _db.Friends.Where(usr => usr.User2 == user).ForEachAsync(usr =>
                {
                    usr.User2.FirstName = user.FirstName;
                    usr.User2.LastName = user.LastName;
                    usr.User2.FullName = user.FullName;
                });
               
                await _db.SaveChangesAsync();
                return Ok();
            }
            return Unauthorized("TOKEN INVALID");


        }
        [HttpPost("changepass")]
        public async Task<ActionResult> ChangePass(passwordPayload pass)
        {
            
            if (HttpContext.User.HasClaim(claim => claim.Type == "Id"))
            {
                long id;
                if (!long.TryParse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Id").Value, out id))
                {
                    return Unauthorized("TOKEN INVALID PARSE ID FAILED");
                }
                var user = _db.Users.Where(u=>u.Id==id).FirstOrDefault();
                if (user == null)
                {
                    return BadRequest("User Not Found");
                }
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(pass.Password);
                _db.SaveChanges();
                return Ok();
            }
            return Unauthorized("TOKEN INVALID");
        }

        [HttpPost("changeemail")]
        public async Task<ActionResult> changeEmail(EmailPayload email)
        {

            if (HttpContext.User.HasClaim(claim => claim.Type == "Id"))
            {
                long id;
                if (!long.TryParse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Id").Value, out id))
                {
                    return Unauthorized("TOKEN INVALID PARSE ID FAILED");
                }
                var user = _db.Users.Where(u => u.Id == id).FirstOrDefault();
                if (user == null)
                {
                    return BadRequest("User Not Found");
                }
                user.Email = email.Email;
                _db.SaveChanges();
                return Ok();
            }
            return Unauthorized("TOKEN INVALID");
        }

    }
}
