namespace ControlVehicular.DTOs;

public class LoginRespuesta
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiracionUtc { get; set; }
}
