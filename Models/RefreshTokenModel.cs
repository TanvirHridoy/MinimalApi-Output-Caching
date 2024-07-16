using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Models;

public class RefreshTokenModel
{
    [Required]
    public string AccessToken { get; set; }

    public string? RefreshToken { get; set; }
}
