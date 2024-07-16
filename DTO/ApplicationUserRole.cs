using Microsoft.AspNetCore.Identity;

namespace MinimalApi.DTO;

public class ApplicationUserRole:IdentityRole<string>
{
    public string? Description { get; set; }
}
