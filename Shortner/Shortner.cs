using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;

namespace URL_Shortner.Shortner
{
    public class Shortnerer : IShortner
    {
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

        public string urlShortner(string url, DateTime? timeLimit)
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

        public string urlRedirect(string userURL)
        {
            userURL = WebUtility.UrlDecode(userURL);



            if (!UrlStorage.ContainsKey(userURL))
                return "Does Not Exit";

            if (UrlStorage[userURL].expire != null && UrlStorage[userURL].expire < DateTime.UtcNow)
                return "Expired";

            return UrlStorage[userURL].link;
        }

    }
}
