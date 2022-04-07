namespace Core.Utilities.Security.Models;

public interface IAccessToken
{
    DateTime Expiration { get; set; }
    string Token { get; set; }
}