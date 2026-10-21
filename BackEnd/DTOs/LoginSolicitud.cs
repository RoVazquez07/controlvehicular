using System.ComponentModel.DataAnnotations;

namespace ControlVehicular.DTOs;

public class LoginSolicitud
{
    [Required]
    [MinLength(3)]
    [MaxLength(64)]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$")]
    public string Usuario { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [MaxLength(128)]
    public string Password { get; set; } = string.Empty;
}
