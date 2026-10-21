using System.ComponentModel.DataAnnotations;

namespace ControlVehicular.DTOs;

public sealed class VehiculoCrearSolicitud
{
    [Required]
    [RegularExpression(@"^[A-Z0-9-]{3,20}$")]
    [MaxLength(20)]
    public string Placa { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string Marca { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string Modelo { get; set; } = string.Empty;

    [Range(1950, 2100)]
    public int Anio { get; set; }

    [Required]
    [MaxLength(20)]
    public string Estado { get; set; } = "Activo";
}
