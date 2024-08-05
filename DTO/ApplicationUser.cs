using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MinimalApi.DTO;

public class ApplicationUser : IdentityUser<string>
{
    [Required(ErrorMessage = "FullName is required.")]
    [MaxLength(150)]
    public required string FullName { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public int TokenBalance { get; set; }
}
