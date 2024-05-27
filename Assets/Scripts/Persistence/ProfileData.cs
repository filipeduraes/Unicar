namespace Unicar.Persistence
{
    public record ProfileData(string Name, string Email, string Password)
    {
        public string Name { get; } = Name;
        public string Email { get; } = Email;
        public string Password { get; } = Password;
    }
}