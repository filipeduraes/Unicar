using System;

namespace Unicar.Persistence
{
    [Serializable]
    public record ProfileData(string Name, ProfileCredentials Credentials)
    {
        public string Name { get; } = Name;
        public ProfileCredentials Credentials { get; } = Credentials;

        public bool MatchEmails(ProfileData profileData)
        {
            return Credentials.Email.Contains(profileData.Credentials.Email);
        }

        public bool MatchCredentials(ProfileCredentials credentials)
        {
            return Credentials.Email.Contains(credentials.Email) && Credentials.Password == credentials.Password;
        }
    }

    [Serializable]
    public record ProfileCredentials(string Email, string Password)
    {
        public string Email { get; } = Email;
        public string Password { get; } = Password;
    }
}