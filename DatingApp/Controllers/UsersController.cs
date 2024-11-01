using API.Helpers;
using AutoMapper;
using DatingApp.Data;
using DatingApp.Dtos;
using DatingApp.Helper;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DatingApp.Controllers
{
  
    [Authorize]
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
   
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository datingRepository, IMapper mapper) 
        { 
            _datingRepository = datingRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userFromRepo = await _datingRepository.GetUser(currentUserId);
            userParams.UserId = currentUserId;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }
            var users = await _datingRepository.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize,
            users.TotalCount, users.TotalPages);
            return Ok(usersToReturn );
        }
        [HttpGet("{id}",Name ="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _datingRepository.GetUser(id); 
            var userToReturn = _mapper.Map<UserForDetailedDto>(user);
            return Ok(userToReturn);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdate) 
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
            {
                return Unauthorized();
            }
            var userFromRepo = await _datingRepository.GetUser(id);
            _mapper.Map(userForUpdate, userFromRepo);
            if(await _datingRepository.SaveAll())
            {
                return NoContent();
            }
            throw new Exception($"failed to save {id}");
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult>LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
            {
                return Unauthorized();
            }
            var like = await _datingRepository.GetLike(id,recipientId);
            if (like != null)
            {
                return BadRequest("You already liked this user");
            }
            if(await _datingRepository.GetUser(recipientId) == null)
            {
                return NotFound();
            }
            like = new Like()
            {
                LikerId = id,
                LikeeId = recipientId
            };
            _datingRepository.Add<Like>(like);
            if(await _datingRepository.SaveAll())
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
