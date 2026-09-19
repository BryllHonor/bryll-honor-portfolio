namespace BryllHonorPortfolio.Models;

public class LoginOptions
{
    public string Username { get; set; } = "admin";
    public string PasswordHash { get; set; } = "";
}

/// <summary>Bound from the login form.</summary>
public class LoginViewModel
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string RedirectTo { get; set; } = "/";
}
