using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using URL_Shortner.Shortner;

namespace URL_Shortner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class URLController : ControllerBase
    {
        private readonly IShortner _shortner;
        private readonly ILimiters _limiters;
        Uri uriResult;

        public URLController(IShortner shortnerer, ILimiters limiters)
        {
            _shortner = shortnerer;
            _limiters = limiters;
        }

        private string GetAnonymousId() //Cookies & IP
        {
            if (Request.Cookies.TryGetValue("anonId", out var anonId))
                return anonId;

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unkown";

            anonId = Guid.NewGuid().ToString();
            Response.Cookies.Append("anonId", anonId, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                SameSite = SameSiteMode.None,
                Secure = true
            });

            return anonId;
        }

        //public static bool IsValidHttpUrl(string url, out Uri resultUri)
        //{
        //    if (Uri.TryCreate(url, UriKind.Absolute, out resultUri))
        //    {
        //        return (resultUri.Scheme == Uri.UriSchemeHttp ||
        //                resultUri.Scheme == Uri.UriSchemeHttps);
        //    }
        //    return false;
        //}

        //[HttpGet]
        //[Route("Validate")]
        //public string ValidateURL(string url)
        //{
        //    if (IsValidHttpUrl(url, out uriResult))
        //        return "Valid URL";
        //    else
        //        return "Invalid URL";
        //}


        [HttpGet]
        [Route("Generate")]
        public async Task<IActionResult> GenerateShortURL(string? url, DateTime? timeLimit)
        {
            string userId = GetAnonymousId();

            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/URL/r/";

            if (!_limiters.AllowRequest(userId))
                return StatusCode(StatusCodes.Status429TooManyRequests, "Rate Limit Exceeded. Try Again Later");

            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("Please Provide a URL");


            try
            {
                string result = await _shortner.urlShortnerAsync(url, timeLimit);

                return Ok(baseUrl + result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

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

        [HttpGet("ClickCount/{code}")]
        public async Task<IActionResult> GetClickCount(string code)
        {
            try
            {
                int clickCount = await _shortner.getClickCount(code);
                return Ok(clickCount);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
