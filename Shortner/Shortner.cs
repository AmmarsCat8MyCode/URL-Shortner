namespace URL_Shortner.Shortner
{
    public class Shortnerer : IShortner
    {
        public string RandomizerTest(string url)
        {
            char[] lower = { 'a','b','c','d','e','f','g','h','i','j','k','l','m',
                             'n','o','p','q','r','s','t','u','v','w','x','y','z'};

            char[] upper = {'A','B','C','D','E','F','G','H','I','J','K','L','M',
                            'N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};

            char[] digits = {'0','1','2','3','4','5','6','7','8','9'};
            
            string blank = string.Empty;

            Random random = new();

            for (int i = 0; i <= 1; i++)
            {
                

                switch (random.Next(0,3))
                {
                    case 0:
                        blank += lower[random.Next(0, lower.Length)].ToString() + " " + upper[random.Next(0, upper.Length)].ToString() + " " + digits[random.Next(0, digits.Length)].ToString() + " ";
                    break;

                    case 1:
                        blank += upper[random.Next(0, upper.Length)].ToString() + " " + digits[random.Next(0, digits.Length)].ToString() + " " + lower[random.Next(0, lower.Length)].ToString() + " ";
                    break;

                    case 2:
                        blank += digits[random.Next(0, digits.Length)].ToString() + " " + lower[random.Next(0, lower.Length)].ToString() + " " + upper[random.Next(0, upper.Length)].ToString() + " ";
                    break;

                }
            }

            return blank;
        }
    }
}
