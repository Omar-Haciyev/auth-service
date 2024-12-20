using System.ComponentModel.DataAnnotations;

namespace TestProject.API.DTOs.Requests;

public record SignInRequest(
    [Required, RegularExpression(@"^[A-Za-z\s]{3,50}$", ErrorMessage = "Name must be between 3 and 50 alphabetic characters.")]
    string Name,
    
    [Required, RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d!@#$%^&*(),.?\"":{}|<>]{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain letters and numbers.")]
    string Password);