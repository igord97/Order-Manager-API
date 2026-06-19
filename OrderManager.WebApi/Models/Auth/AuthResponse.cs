namespace OrderManager.WebApi.Models.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }
}