using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HeidelbergCement.Model;
using HeidelbergCement.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HeidelbergCement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly IUserRepository userRepo;

        public TokenController(IConfiguration config, IUserRepository userRepo)
        {
            _configuration = config;
            this.userRepo = userRepo;
        }

        [HttpPost]        
        public  IActionResult Post(User _userData)
        {

            if (_userData != null && _userData.Email != null && _userData.Password != null)
            {
                var user = GetUser(_userData.Email, _userData.Password);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Email", user.Email.ToString())
                   };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        private User GetUser(string email, string password)
        {
            if (userRepo.GetUserByEmailAndPassWord(email, password) != null)
                return new User { Email = email, Password = password };
            else
                return null;            
        }


    }
}
