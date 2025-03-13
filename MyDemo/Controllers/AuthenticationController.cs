using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using MyDemo.DAL;

namespace MyDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationDAL _authenticationDAL;
        public AuthenticationController(AuthenticationDAL authenticationDAL) { 
            _authenticationDAL = authenticationDAL;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(AuthDetails authentication)
        {
            try
            {
                var result = await _authenticationDAL.Login(authentication);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(Register register)
        {
            try
            {
                var regiterCheck = await _authenticationDAL.Register(register);
                return Ok(new { status = regiterCheck.Item1, data = regiterCheck.Item2 });
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetSecretData")]
        public IActionResult GetSecretData()
        {
            try
            {
                return Ok(new { message = "This is a protected resource!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            //HttpContext.Request.Headers.TryGetValue("Session-Key", out var userId);
            //if (Request.Cookies.TryGetValue($"AuthToken_{userId}", out string? jwtToken))
            //{
            //    return Ok(new { message = "This is a protected resource!" });
            //}
            //return BadRequest(new { message = "UnAuth" });
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Request.Headers.TryGetValue("Session-Key", out var userId);
            Response.Cookies.Delete($"AuthToken_{userId}");
            return Ok(new { message = "logout" });
        }
    }
}
