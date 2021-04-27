using AutoMapper;
using DatingApp.API.Data.Interfaces;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly ISetting _setting;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repository,
                              ISetting setting,
                              IMapper mapper)
        {
            _repository = repository;
            _setting = setting;
            _mapper = mapper;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _repository.UserExists(dto.UserName))
                return BadRequest("Use another userName because it is used by another one");

            var user = _mapper.Map<User>(dto);
            user.UserName = user.UserName.ToLower();

            var result = await _repository.Register(user, dto.Password);

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _repository.Login(dto.UserName, dto.Password);

            if (user == null)
                return Unauthorized();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.Key));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var descriptor = new SecurityTokenDescriptor()
            {
                SigningCredentials = credentials,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                IssuedAt = DateTime.UtcNow,
                Issuer = _setting.Issuer,
                Audience = _setting.Audience
            };

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(descriptor);

            return Ok(new
            {
                Token = handler.WriteToken(securityToken),
                User = _mapper.Map<UserForListDto>(user)
            });
        }

        [HttpPost("UserExist")]

        public async Task<IActionResult> UserExists(string userName)
        {
            return Ok(await _repository.UserExists(userName));
        }
    }
}
