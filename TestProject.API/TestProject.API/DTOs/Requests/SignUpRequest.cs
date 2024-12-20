namespace TestProject.API.DTOs.Requests;

using System;
using System.ComponentModel.DataAnnotations;

public record SignUpRequest(
    [Required, RegularExpression(@"^[A-Za-z\s]{3,50}$", ErrorMessage = "Name must be between 3 and 50 alphabetic characters.")]
    string Name,

    [Required, DataType(DataType.Date)]
    DateTime DateOfBirth,

    [Required, RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d!@#$%^&*(),.?\"":{}|<>]{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain letters and numbers.")]
    string PasswordHash);
