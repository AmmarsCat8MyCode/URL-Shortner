using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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
        public async Task<IActionResult> GenerateShortURL(string url, DateTime? timeLimit)
        {
            string result = await _shortner.urlShortnerAsync(url, timeLimit);

            return Ok(result);
        }

        [HttpGet("r/{code}")]
        public async Task<IActionResult> RedirectToOriginal(string code)
        {
            try
            {
                string originalUrl = await _shortner.urlRedirectAsync(code);
                return Redirect(originalUrl);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
