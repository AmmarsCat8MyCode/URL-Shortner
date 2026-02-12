using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using URL_Shortner.Shortner;

namespace URL_Shortner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class URLController : ControllerBase
    {
        private readonly IShortner _shortner;
        
        public URLController(IShortner shortnerer)
        {
            _shortner = shortnerer;
        }

        [HttpGet]
        [Route("generate")]
        public IActionResult Random6(string url, DateTime? timeLimit)
        {
            string result = _shortner.urlShortner(url, timeLimit);

            return Ok(result);
        }

        [HttpGet("r/{userURL}")]
        public IActionResult Redirect(string userURL)
        {
            string result = _shortner.urlRedirect(userURL);
            return Redirect(result);
        }
    }
}
