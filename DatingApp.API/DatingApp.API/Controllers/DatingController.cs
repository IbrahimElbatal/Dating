using AutoMapper;
using DatingApp.API.Data.Interfaces;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DatingController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;

        public DatingController(IDatingRepository repository,
                                IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [ServiceFilter(typeof(LastActiveUserFilter))]
        public async Task<IActionResult> Get([FromQuery] UserParams userParams)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userInDb = await _repository.GetUser(userId);
            userParams.UserId = userId;

            if (string.IsNullOrEmpty(userParams.Gender) && userParams.Likees == false && userParams.Likers == false)
            {
                userParams.Gender = userInDb.Gender == "male" ? "female" : "male";
            }

            var users = await _repository.GetUsers(userParams);

            Response.AddPagination(users);
            return Ok(_mapper.Map<IList<UserForListDto>>(users));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _repository.GetUser(id);
            return Ok(_mapper.Map<UserForDetailsDto>(user));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UserForUpdateDto userForUpdate)
        {
            if (id != int.Parse(User.FindFirstValue(claimType: ClaimTypes.NameIdentifier)))
                return Unauthorized();

            var userInDb = await _repository.GetUser(id);

            _mapper.Map(userForUpdate, userInDb);

            if (await _repository.SaveAll())
                return Ok(userInDb);

            throw new Exception("User with id " + id + " failed to save");
        }

        [HttpPost("user/{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirstValue(claimType: ClaimTypes.NameIdentifier)))
                return Unauthorized();

            var like = await _repository.GetUserLike(id, recipientId);

            if (like != null)
                return BadRequest("You liked this user before");

            var likee = await _repository.GetUser(recipientId);
            if (likee == null)
                return NotFound("No User Found");

            var newLike = new Like()
            {
                LikerId = id,
                LikeeId = recipientId
            };

            _repository.Add(newLike);

            if (await _repository.SaveAll())
            {
                return Ok(new { Message = $"You Liked {likee.KnownAs} successfully" });
            }

            return BadRequest("Failed to like user");
        }
    }
}
