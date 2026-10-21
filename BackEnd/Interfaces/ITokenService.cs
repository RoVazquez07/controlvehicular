using ControlVehicular.DTOs;

namespace ControlVehicular.Interfaces;

public interface ITokenService
{
    LoginRespuesta? Autenticar(LoginSolicitud solicitud);
}
