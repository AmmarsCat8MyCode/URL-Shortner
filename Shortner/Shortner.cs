using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace URL_Shortner.Shortner
{
    public class Shortnerer : IShortner
    {

        private readonly AppDbContext _context;

        public Shortnerer (AppDbContext context)
        {
            _context = context;
        }

        private class UrlContainer
        {
            public string link;
            public DateTime dateCreated;
            public DateTime? expire;
        }

                    //short  long
        Dictionary<string, UrlContainer> UrlStorage = new();
        Dictionary<string, string> shortUrlsStorage = new();
        Random random = new();
        string tinyURLDomain = "www.tinyclone.com/";

        //Token Bucket
        private class Bucket
        {
            public double Tokens;
            public DateTime LastRefill;
        }

        private readonly Dictionary<string, Bucket> _buckets = new();
        private const int Capacity = 5;
        private const double RefillPerSecond = 2.0 / 60;

        public string urlShortner(string url, DateTime? timeLimit) //TEMPORARY METHOD
        {

            const string allLetterNums = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            
            char[] randomizedCode = new char[6];

            string shortURL = string.Empty;

            var now = DateTime.UtcNow;
           
            if(shortUrlsStorage.TryGetValue(url, out var existing))
                return tinyURLDomain + existing;

            do
            {
                for (int i = 0; i < 6; i++)
                {
                    randomizedCode[i] = allLetterNums[random.Next(0, allLetterNums.Length)];
                }

                shortURL = new string(randomizedCode);
            }
            while (UrlStorage.ContainsKey(shortURL));

            UrlStorage[shortURL] = new UrlContainer
            {
                link = url,
                dateCreated = now,
                expire = timeLimit
            };

            shortUrlsStorage[url] = shortURL;

            return tinyURLDomain + shortURL;
        }

        private string GenerateCode(int size)
        {
            const string allLetterNums = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            char[] randomizedCode = new char[size];

            Random random = new();

            string shortURL = string.Empty;

            for (int i = 0; i < size; i++)
            {
                randomizedCode[i] = allLetterNums[random.Next(0, allLetterNums.Length)];
            }
            return new string(randomizedCode);
        }

        public string urlRedirect(string userURL)
        {
            userURL = WebUtility.UrlDecode(userURL);



            if (!UrlStorage.ContainsKey(userURL))
                return "Does Not Exit";

            if (UrlStorage[userURL].expire != null && UrlStorage[userURL].expire < DateTime.UtcNow)
                return "Expired";

            return UrlStorage[userURL].link;
        }
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<string> urlShortnerAsync(string url, DateTime? timeLimit) //PERSISTENCE METHOD
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }

            var existing = await _context.URLs.FirstOrDefaultAsync(u => u.longUrl == url);

            if (existing != null)
                return tinyURLDomain + existing.code;

            string code;

            do
            {
                code = GenerateCode(6);
            } while (await _context.URLs.AnyAsync(u => u.code == code));

            var urlStorage = new global::UrlContainer 
            {
                code = code,
                longUrl = url,
                dateCreated = DateTime.UtcNow,
                expire = timeLimit
            };

            _context.URLs.Add(urlStorage);
            await _context.SaveChangesAsync();

            return tinyURLDomain + code;
        }
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<string> urlRedirectAsync(string userURL)
        {
            var container = await _context.URLs.FirstOrDefaultAsync(u => u.code == userURL);

            if (container == null)
                throw new Exception("Does Not Exist");

            if (container.expire.HasValue && container.expire.Value < DateTime.UtcNow)
                throw new Exception("Expired");

            return container.longUrl;
        }

        public bool AllowRequest(string userId) //Minutes
        {
            if (!_buckets.ContainsKey(userId))
            {
                _buckets[userId] = new Bucket
                {
                    Tokens = Capacity,
                    LastRefill = DateTime.UtcNow,
                };
            }

            var bucket = _buckets[userId];
            var now = DateTime.UtcNow;

            var secondsPassed = (now - bucket.LastRefill).TotalSeconds;

            if (secondsPassed > 0)
            {
                bucket.Tokens = Math.Min(Capacity, (int)(bucket.Tokens + secondsPassed * RefillPerSecond));
                bucket.LastRefill = now;
            }

            if(bucket.Tokens > 0)
            {
                bucket.Tokens--;
                return true;
            }
                
            return false;
        }
    }
}
