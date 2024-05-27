public record RegisterRequestInfo(string Name, string Email, string Password, string RepeatedPassword)
{
    public string Name { get; } = Name;
    public string Email { get; } = Email;
    public string Password { get; } = Password;
    public string RepeatedPassword { get; } = RepeatedPassword;
}