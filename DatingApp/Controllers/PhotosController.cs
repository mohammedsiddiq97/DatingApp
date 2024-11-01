using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.Data;
using DatingApp.Dtos;
using DatingApp.Helper;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace DatingApp.Controllers
{
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _dating;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository dating, IMapper mapper,IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _dating = dating;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;
            Account acc = new Account()
            {
                Cloud = _cloudinaryConfig.Value.CloudName,
                ApiKey = _cloudinaryConfig.Value.ApiKey,
                ApiSecret = _cloudinaryConfig.Value.ApiSecret
            };
            _cloudinary = new Cloudinary(acc);
        }
        [HttpGet("{id}",Name ="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await  _dating.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotosForCreationDto photoForcreation)
        {
            try
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
                {
                    return Unauthorized();
                }
                var userFromRepo = await _dating.GetUser(userId);
                var file = photoForcreation.File;
                var uploadResult = new ImageUploadResult()
                {

                };
                if (file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.Name, stream),
                            Transformation = new Transformation().Width(500)
                            .Height(500).Crop("fill").Gravity("face")
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                    }
                }
                photoForcreation.Url = uploadResult.Url.ToString();
                photoForcreation.PublicId = uploadResult.PublicId;
                var photo = _mapper.Map<Photo>(photoForcreation);
                photo.Description = photoForcreation.Description != null ? photoForcreation.Description : "";
                photo.UserId = userId;
                photo.Url = uploadResult.Url
                    .ToString();
                photo.PublicId = uploadResult.PublicId;
                if (!userFromRepo.Photos.Any(u => u.IsMain))
                {
                    photo.IsMain = true;
                }
                userFromRepo.Photos.Add(photo);
                if (await _dating.SaveAll())
                {
                    var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                    return CreatedAtRoute("GetPhoto", new { userId = userId, id = photo.Id }, photoToReturn);
                }
                return BadRequest("Could not add the photo");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId,int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
            {
                return Unauthorized();
            }
            var user = await _dating.GetUser(userId);
            if(!user.Photos.Any(p=> p.Id == id))
            {
                return Unauthorized();
            }
            var photoFromRepo = await _dating.GetPhoto(id);
            if (photoFromRepo.IsMain)
            {
                return BadRequest("this is already main photo");
            }
            var currentMainPhoto = await _dating.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;
            if (await _dating.SaveAll())
                return NoContent();
            return BadRequest("Could Not save to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
            {
                return Unauthorized();
            }
            var user = await _dating.GetUser(userId);
            if (!user.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }
            var photoFromRepo = await _dating.GetPhoto(id);
            if (photoFromRepo.IsMain)
            {
                return BadRequest("You cannot delete your main photo");
            }
            if (photoFromRepo.PublicId != null)
            {
                var deleteParam = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParam);
                if (result.Result == "ok")
                {
                    _dating.Delete(photoFromRepo);
                }
            }
           if(photoFromRepo.PublicId == null)
            {
                _dating.Delete(photoFromRepo);
            }
            if (await _dating.SaveAll())
            {
                return Ok();
            }
            return BadRequest("Failed to delete the photo");
        }
    }
}
