namespace Users.Domain.Enums;

/// <summary>
/// Provedor de autenticação/login.
/// </summary>
public enum LoginProvider
{
    /// <summary>
    /// Login local (email e senha).
    /// </summary>
    Local = 0,

    /// <summary>
    /// Login com Google.
    /// </summary>
    Google = 1,

    /// <summary>
    /// Login com Facebook.
    /// </summary>
    Facebook = 2,

    /// <summary>
    /// Login com Apple.
    /// </summary>
    Apple = 3,

    /// <summary>
    /// Login com Microsoft.
    /// </summary>
    Microsoft = 4,

    /// <summary>
    /// Login com GitHub.
    /// </summary>
    GitHub = 5
}
