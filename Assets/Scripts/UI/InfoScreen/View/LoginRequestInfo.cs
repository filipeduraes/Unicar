public record LoginRequestInfo(string Email, string Password)
{
    public string Email { get; } = Email;
    public string Password { get; } = Password;
}