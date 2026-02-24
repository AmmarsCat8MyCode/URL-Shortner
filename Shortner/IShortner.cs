public interface IShortner
{
    string urlShortner(string url, DateTime? timeLimit);
    string urlRedirect(string userURL);
    Task<string> urlShortnerAsync(string url, DateTime? timeLimit);
    Task<string> urlRedirectAsync(string userURL);
    Task<int> getClickCount(string code);
}
