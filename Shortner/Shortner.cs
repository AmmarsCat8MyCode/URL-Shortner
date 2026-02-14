using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Net;

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
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

    }
}
