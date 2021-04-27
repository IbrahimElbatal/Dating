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
    [ServiceFilter(typeof(LastActiveUserFilter))]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;

        public MessageController(IDatingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> Get(int userId, int id)
        {
            var loggedUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId != loggedUserId)
                return Unauthorized();

            var message = await _repository.GetMessage(id);
            if (message == null)
                return NotFound("No Message found");

            var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
            return Ok(messageToReturn);
        }

        [HttpPost("sender/{userId}/newMessage")]
        public async Task<IActionResult> Create(int userId, CreateMessageDto dto)
        {
            var loggedUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId != loggedUserId)
                return Unauthorized();


            var message = _mapper.Map<Message>(dto);
            _repository.Add(message);
            if (await _repository.SaveAll())
            {
                var sender = await _repository.GetUser(userId);
                var recipient = await _repository.GetUser(dto.RecipientId);
                message.Sender = sender;
                message.Recipient = recipient;
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new { userId = userId, id = message.Id }, messageToReturn);
            }

            return BadRequest("Failed to Create Message");

        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery] MessageParams param)
        {
            var loggedUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userId != loggedUserId)
                return Unauthorized();

            param.UserId = userId;
            var messages = await _repository.GetMessagesForUser(param);
            Response.AddPagination(messages);

            var response = _mapper.Map<IList<MessageToReturnDto>>(messages);
            return Ok(response);
        }

        [HttpGet("user/{userId}/threadWith/{recipientId}")]
        public async Task<IActionResult> GetUserMessageThread(int userId, int recipientId)
        {
            var loggedUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userId != loggedUserId)
                return Unauthorized();

            var messages = await _repository.GetMessageThread(userId, recipientId);

            var response = _mapper.Map<IList<MessageToReturnDto>>(messages);
            return Ok(response);
        }

        [HttpPost("{id}/user/{userId}/delete")]
        public async Task<IActionResult> Delete(int id, int userId)
        {
            var loggedUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userId != loggedUserId)
                return Unauthorized();

            var message = await _repository.GetMessage(id);

            if (message.RecipientDelete == false && message.RecipientId == userId)
                message.RecipientDelete = true;

            if (message.SenderDelete == false && message.SenderId == userId)
                message.SenderDelete = true;


            if (message.SenderDelete == true && message.RecipientDelete == true)
                _repository.Delete(message);

            if (await _repository.SaveAll())
                return Ok();

            return BadRequest("Failed To Delete Message");
        }

        [HttpPost("{id}/user/{userId}/markAsRead")]
        public async Task<IActionResult> MarkAsRead(int id, int userId)
        {
            var loggedUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userId != loggedUserId)
                return Unauthorized();

            var message = await _repository.GetMessage(id);

            if (message.IsRead == false && message.RecipientId == userId)
            {
                message.IsRead = true;
                message.DateRead = DateTime.Now;
            }

            if (await _repository.SaveAll())
                return Ok();

            return BadRequest("Failed To Delete Message");
        }
    }
}
