using System.ComponentModel.DataAnnotations;

namespace ControlVehicular.Web.Models;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingresa un correo válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}

public sealed class RegisterViewModel
{
    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public bool Agreed { get; set; }
}

public sealed class ForgotPasswordViewModel
{
    public string? Email { get; set; }
}
