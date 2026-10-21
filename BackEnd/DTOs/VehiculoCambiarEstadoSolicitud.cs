using System.ComponentModel.DataAnnotations;

namespace ControlVehicular.DTOs;

public sealed class VehiculoCambiarEstadoSolicitud
{
    [Required]
    [MaxLength(20)]
    public string Estado { get; set; } = string.Empty;
}
