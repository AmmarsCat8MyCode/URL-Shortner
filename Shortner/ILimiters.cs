namespace URL_Shortner.Shortner
{
    public interface ILimiters
    {
        bool AllowRequest(string userId);
    }
}
