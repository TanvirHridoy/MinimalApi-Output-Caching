using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Models;

public class UserRegistrationModel
{
    [Required]
    public required string Email { get; set; }
    [Required]
    public required string FullName { get; set; }

    [Required]
    public required string Password { get; set; }

    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public required string ConfirmPassword { get; set; }
}
