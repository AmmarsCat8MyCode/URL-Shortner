using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URL_Shortner.Shortner;

namespace URL_Shortner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class URLController : ControllerBase
    {
        private static readonly IShortner _shortner = new Shortnerer(); 

        [HttpGet]
        public IActionResult Random6()
        {
            string result = _shortner.urlShortner("wtf");

            return Ok(result);
        }
    }
}
