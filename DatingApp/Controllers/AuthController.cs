using AutoMapper;
using DatingApp.Dtos;
using DatingApp.Interface;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    [AllowAnonymous]
   
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper    _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(
                               IConfiguration configuration,
                               IMapper mapper,
                               UserManager<User> userManager,
                               SignInManager<User> signInManager)
        {
            _configuration = configuration;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]UserForRegisterDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            var userToCreate = _mapper.Map<User>(user);

            userToCreate.Interest = "";
            userToCreate.Introduction = "";
            userToCreate.LookingFor = "";
            var result = await _userManager.CreateAsync(userToCreate,user.Password);
            var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);
            if(result.Succeeded)
            {
                return CreatedAtRoute("GetUser", new { controller = "Users", id = userToCreate.Id }, userToReturn);
            }
            return BadRequest(result.Errors);
           
        }
        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn(UserForLogInDto logInDto)
        {
            var user = await _userManager.FindByNameAsync(logInDto.UserName);

            var result = await _signInManager.CheckPasswordSignInAsync(user, logInDto.Password,false);
            if (result.Succeeded)
            {
                var userList = _mapper.Map<UserForListDto>(user);

                return Ok(new
                {
                    token = GenerateJWtToken(user).Result,
                     userList
                });
            }
            return Unauthorized();
           
        }

        private async Task<string> GenerateJWtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };

              var roles = await _userManager.GetRolesAsync(user);
            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        
    }
   
} 
