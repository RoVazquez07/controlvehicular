using ControlVehicular.DTOs;
using ControlVehicular.Models;

namespace ControlVehicular.Security;

public static class MapeoVehiculo
{
    public static VehiculoRespuesta ARespuesta(Vehiculo vehiculo) => new()
    {
        Id = vehiculo.Id,
        Placa = vehiculo.Placa,
        Marca = vehiculo.Marca,
        Modelo = vehiculo.Modelo,
        Anio = vehiculo.Anio,
        Estado = vehiculo.Estado,
        FechaRegistroUtc = vehiculo.FechaRegistroUtc
    };
}
