namespace URL_Shortner.Shortner
{
    public class Shortnerer : IShortner
    {
        public string urlShortner(string url)
        {
                     //shortURL URL
            Dictionary<string, string> urls = new();

            const string allLetterNums = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            
            char[] randomizedCode = new char[6];

            Random random = new();

            string shortURL = string.Empty;

            do
            {
                for (int i = 0; i < 6; i++)
                {
                    randomizedCode[i] = allLetterNums[random.Next(0, allLetterNums.Length)];
                }

                shortURL = new string(randomizedCode);
            }
            while (!urls.ContainsKey(shortURL));

            urls[shortURL] = url;

            return shortURL;
        }
    }
}
