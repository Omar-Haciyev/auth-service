using TestProject.API.Helpers.Interfaces;

namespace TestProject.API.Helpers.Classes;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrWhiteSpace(providedPassword))
            throw new ArgumentException("Provided password cannot be empty.", nameof(providedPassword));

        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }
}