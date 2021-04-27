using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data.Interfaces;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cloudinary = CloudinaryDotNet.Cloudinary;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users/{userId}/photos")]
    public class PhotoController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly ISetting _setting;
        private readonly IMapper _mapper;
        private readonly Cloudinary cloudinary;
        public PhotoController(IDatingRepository repository,
                                ISetting setting,
                                IMapper mapper)
        {
            _repository = repository;
            _setting = setting;
            _mapper = mapper;

            var account = new Account(
            _setting.Cloudinary.Name,
                _setting.Cloudinary.ApiKey,
                _setting.Cloudinary.ApiSecret
                );

            cloudinary = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> Get(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                return Unauthorized();

            var user = await _repository.GetUser(userId);

            var photo = user.Photos.FirstOrDefault(p => p.Id == id);

            return Ok(_mapper.Map<PhotoForListDto>(photo));
        }
        [HttpPost]
        public async Task<IActionResult> UploadPhoto(int userId,
            [FromForm]PhotoForUploadDto photoForUpload)
        {
            if (userId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                return Unauthorized();

            var userToEdit = await _repository.GetUser(userId);

            var file = photoForUpload.File;

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var imageUploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation()
                            .Width(200).Height(200).Crop("fill").Gravity("face")
                    };

                    var result = cloudinary.Upload(imageUploadParams);
                    if (result != null)
                    {
                        photoForUpload.PublicId = result.PublicId;
                        photoForUpload.Url = result.Uri.ToString();
                    }

                    if (userToEdit.Photos.All(p => p.IsMain != true))
                    {
                        photoForUpload.IsMain = true;
                    }

                    var photo = _mapper.Map<Photo>(photoForUpload);
                    userToEdit.Photos.Add(photo);

                    if (await _repository.SaveAll())
                    {
                        var returnPhoto = _mapper.Map<PhotoForListDto>(photo);
                        return CreatedAtRoute("GetPhoto", new { id = photo.Id, userId = userId }, returnPhoto);
                    }
                }
            }

            return BadRequest("Uploading Photo failed");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                return Unauthorized();

            var user = await _repository.GetUser(userId);

            var photo = user.Photos.FirstOrDefault(p => p.Id == id);

            if (photo.IsMain)
                return BadRequest("Can't remove main Photo");

            var deleteParams = new DeletionParams(photo.PublicId);
            var result = cloudinary.Destroy(deleteParams);

            if (result.Result == "ok")
            {
                user.Photos.Remove(photo);

                if (await _repository.SaveAll())
                    return Ok(new { Message = "Photo deleted" });
            }
            return BadRequest("error during photo deletion");

        }

        [HttpPost("{id}")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                return Unauthorized();

            var user = await _repository.GetUser(userId);

            var userMainPhoto = await _repository.GetMainPhoto(userId);
            if (userMainPhoto != null)
                userMainPhoto.IsMain = false;

            var photo = user.Photos.FirstOrDefault(p => p.Id == id);
            if (photo != null)
                photo.IsMain = true;

            if (await _repository.SaveAll())
            {
                var returnPhoto = _mapper.Map<PhotoForListDto>(photo);
                return CreatedAtRoute("GetPhoto", new { userId = userId, id = id }, returnPhoto);
            }

            return BadRequest("Error during setting main photo");
        }
    }
}
