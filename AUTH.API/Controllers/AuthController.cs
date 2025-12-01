using CORE.DTOs;
using CORE.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AUTH.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<AuthBaseResponseDto>> Register(UserRegisterDto dto)
        {
            var responeDto = await _authService.Register(dto);
            if (responeDto.Success == true) return Ok(responeDto);
            return BadRequest(responeDto);
        }
        [HttpPost("Login")]
        public async Task<ActionResult<AuthBaseResponseDto>> Login(UserLoginDto dto)
        {
            var responseDto = await _authService.Login(dto);
            if (responseDto.Success == true) return Ok(responseDto);
            return Unauthorized(responseDto);
        }
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<AuthBaseResponseDto>> RefreshToken([FromBody] string token)
        {
            var responseDto = await _authService.RefreshToken(token);
            if (responseDto.Success == true) return Ok(responseDto);
            return Unauthorized(responseDto);
        }
        [HttpPost("LogOut")]
        public async Task<ActionResult> LogOut([FromBody] string token)
        {
            var result = await _authService.LogOut(token);
            if (result) return Ok(new { Success = true, Message = "Logged Out Successfully" });
            return BadRequest(new { Success = false, Message = "Error During LogOut" });
        }
        [Authorize]
        [HttpGet("Test")]
        public ActionResult Test()
        {
            return Ok("Auth Service is working fine");
        }
    }
}
