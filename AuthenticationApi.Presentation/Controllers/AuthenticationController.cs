using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.SharedLibrary.Logs;
using ProductCatalog.SharedLibrary.Responses;
using System.Net.WebSockets;

namespace AuthenticationApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IUser _user) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<ActionResult<Response>> Register(AppUsersDTO _usersDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _user.Register(_usersDTO);
            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<Response>> Login(LoginDTO _loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _user.Login(_loginDTO);
            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetUserDTO>> GetUserById(int id)
        {
            if (id <= 0)

                return BadRequest("Invalid User Id");
            var user = await _user.GetUserById(id);
            return user.Id > 0 ? Ok(user) : NotFound();
        }
    }
}