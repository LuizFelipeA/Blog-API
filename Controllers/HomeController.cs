using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [ApiController]
    [Route("api/")]
    public class HomeController : ControllerBase
    {
        [HttpGet("health-check")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}