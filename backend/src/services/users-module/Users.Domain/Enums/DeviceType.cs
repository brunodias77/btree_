namespace Users.Domain.Enums;

/// <summary>
/// Tipo de dispositivo do usuário.
/// </summary>
public enum DeviceType
{
    /// <summary>
    /// Desconhecido.
    /// </summary>
    Desconhecido = 0,

    /// <summary>
    /// Computador desktop.
    /// </summary>
    Desktop = 1,

    /// <summary>
    /// Dispositivo móvel (smartphone).
    /// </summary>
    Mobile = 2,

    /// <summary>
    /// Tablet.
    /// </summary>
    Tablet = 3,

    /// <summary>
    /// Smart TV.
    /// </summary>
    SmartTV = 4,

    /// <summary>
    /// Outros dispositivos.
    /// </summary>
    Outro = 5
}
