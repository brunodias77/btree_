using Microsoft.AspNetCore.Identity;

namespace Users.Domain.Identity;


/// <summary>
/// Application user extending ASP.NET Core Identity.
/// Additional user data should be stored in the Profile aggregate.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// Date and time when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date and time of the last update.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}