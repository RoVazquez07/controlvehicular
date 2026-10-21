using System.Security.Claims;
using ControlVehicular.Configuration;
using ControlVehicular.Data;
using ControlVehicular.DTOs;
using ControlVehicular.Errores;
using ControlVehicular.Interfaces;
using ControlVehicular.Models;
using ControlVehicular.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ControlVehicular.Services;

public sealed class VehiculoService : IVehiculoService
{
    private readonly IVehiculoRepositorio _repositorio;
    private readonly IAuthorizationService _autorizacion;
    private readonly SqlServerOpciones _sql;

    public VehiculoService(
        IVehiculoRepositorio repositorio,
        IAuthorizationService autorizacion,
        IOptions<SqlServerOpciones> sql)
    {
        _repositorio = repositorio;
        _autorizacion = autorizacion;
        _sql = sql.Value;
    }

    public async Task<VehiculoRespuesta?> ObtenerPorIdAsync(int id, ClaimsPrincipal usuario, CancellationToken cancellationToken = default)
    {
        var entidad = await _repositorio.ObtenerPorIdSpAsync(id, cancellationToken);
        if (entidad is null)
        {
            return null;
        }

        await AsegurarRecursoAsync(usuario, entidad);
        return MapeoVehiculo.ARespuesta(entidad);
    }

    public async Task<ResultadoPaginado<VehiculoRespuesta>> ListarAsync(
        VehiculoConsulta consulta,
        ClaimsPrincipal usuario,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(consulta.OrdenarPor)
            && !IdentificadoresSql.TryObtenerColumnaOrden(consulta.OrdenarPor, out _))
        {
            throw new ErrorValidacionNegocioException("Campo de orden no permitido.");
        }

        var (pagina, tamano) = PaginacionReglas.Normalizar(consulta.Pagina, consulta.TamanoPagina, _sql);
        var filtro = new VehiculoFiltro
        {
            Pagina = pagina,
            TamanoPagina = tamano,
            Placa = consulta.Placa,
            OrdenarPor = consulta.OrdenarPor,
            Direccion = consulta.Direccion,
            RegistradoPor = usuario.IsInRole(RolesAplicacion.Administrador) ? null : usuario.Identity?.Name
        };

        var resultado = await _repositorio.ListarAsync(filtro, cancellationToken);
        return new ResultadoPaginado<VehiculoRespuesta>
        {
            Items = resultado.Items.Select(MapeoVehiculo.ARespuesta).ToList(),
            Total = resultado.Total,
            Pagina = resultado.Pagina,
            TamanoPagina = resultado.TamanoPagina
        };
    }

    public async Task<VehiculoRespuesta> CrearAsync(
        VehiculoCrearSolicitud solicitud,
        ClaimsPrincipal usuario,
        CancellationToken cancellationToken = default)
    {
        if (!IdentificadoresSql.EsEstadoPermitido(solicitud.Estado))
        {
            throw new ErrorValidacionNegocioException("Estado no permitido.");
        }

        var registradoPor = usuario.Identity?.Name
            ?? throw new ErrorValidacionNegocioException("No se pudo determinar el usuario autenticado.");

        var entidad = new Vehiculo
        {
            Placa = solicitud.Placa.Trim(),
            Marca = solicitud.Marca.Trim(),
            Modelo = solicitud.Modelo.Trim(),
            Anio = solicitud.Anio,
            Estado = solicitud.Estado.Trim(),
            FechaRegistroUtc = DateTime.UtcNow,
            RegistradoPor = registradoPor
        };

        var creado = await _repositorio.InsertarConBitacoraAsync(entidad, cancellationToken);
        return MapeoVehiculo.ARespuesta(creado);
    }

    public async Task CambiarEstadoAsync(int id, string estado, ClaimsPrincipal usuario, CancellationToken cancellationToken = default)
    {
        if (!IdentificadoresSql.EsEstadoPermitido(estado))
        {
            throw new ErrorValidacionNegocioException("Estado no permitido.");
        }

        var entidad = await _repositorio.ObtenerPorIdSpAsync(id, cancellationToken);
        if (entidad is null)
        {
            throw new RecursoNoAutorizadoException();
        }

        await AsegurarRecursoAsync(usuario, entidad);
        var actualizado = await _repositorio.CambiarEstadoSpAsync(id, estado.Trim(), cancellationToken);
        if (!actualizado)
        {
            throw new RecursoNoAutorizadoException();
        }
    }

    private async Task AsegurarRecursoAsync(ClaimsPrincipal usuario, Vehiculo entidad)
    {
        var decision = await _autorizacion.AuthorizeAsync(usuario, entidad, Politicas.RecursoVehiculo);
        if (!decision.Succeeded)
        {
            throw new RecursoNoAutorizadoException();
        }
    }
}
