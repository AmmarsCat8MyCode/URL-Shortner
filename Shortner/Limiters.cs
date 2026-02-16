namespace URL_Shortner.Shortner
{
    public class Limiters : ILimiters
    {   
        
        //Token Bucket
        private class Bucket
        {
            public double Tokens;
            public DateTime LastRefill;
        }

        //BUCKET
        private readonly Dictionary<string, Bucket> _buckets = new();
        private const int Capacity = 5;
        private const double RefillPerSecond = 2.0 / 60;

        //FIXED RL
        private readonly Dictionary<string, int> _requests = new();
        private const int MaxRequests = 3;

        public bool AllowRequest(string userId)
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

            if (bucket.Tokens > 0)
            {
                bucket.Tokens--;
                return true;
            }

            return false;
        }
    }
}
