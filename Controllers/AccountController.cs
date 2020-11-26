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
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

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
            public ActionResult<User> Register([FromBody]RegisterPayload registerPayload)
            {
            
                try
                {
                    MailAddress m = new MailAddress(registerPayload.Email);
                }   
                catch (Exception ex)
                {
                    
                    return new JsonResult(new { status = "false", message = "email format "+ registerPayload.Email });
                }
            
            
                try
                {
                    var existingUserWithMail = _db.Users
                .Any(u => u.Email == registerPayload.Email);

                    if (existingUserWithMail)
                    {
                        return new JsonResult(new { status = "false", message = "An account with this email already exists " });
                    }

                var userToCreate = new User
                {
                    Email = registerPayload.Email,
                    FirstName = registerPayload.FirstName,
                    LastName = registerPayload.LastName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerPayload.Password),
                    Role = "SimpleUser",
                    };
               
                _db.Users.Add(userToCreate);
                    
                    

                    _db.SaveChanges();

                    return Ok(new { status = true, user = userToCreate });
                }
                catch (Exception ex)
                {
                return new JsonResult(new { status = "false", message = ""+ ex.Message });
            }
            
        }

            [AllowAnonymous]
            [Route("login")]
            [HttpPost]
            public async Task<IActionResult> Login([FromBody]LoginPayload loginPayload)
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
                        token= tokenString

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
                };

                var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                  _config["Jwt:Issuer"],
                  claims,
                  expires: DateTime.Now.AddDays(30),
                  signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        
    }
}
