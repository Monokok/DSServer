using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        [HttpGet("token")]
        [Authorize]
        public IActionResult GetToken()
        {
            // Получение токена из заголовка Authorization
            if (Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.ToString().Replace("Bearer ", string.Empty);
                return Ok(new { Token = token });
            }
            return BadRequest("Token not found");
        }
    }
}
