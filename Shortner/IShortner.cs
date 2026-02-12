public interface IShortner
{
        string urlShortner(string url, DateTime? timeLimit);
        string urlRedirect(string userURL);
}
