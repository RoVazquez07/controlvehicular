using ControlVehicular.Configuration;

namespace ControlVehicular.Services;

public static class PaginacionReglas
{
    public static (int Pagina, int Tamano) Normalizar(int pagina, int tamano, SqlServerOpciones opciones)
    {
        var paginaSegura = pagina < 1 ? 1 : pagina;
        var tamanoSeguro = tamano < 1
            ? opciones.TamanoPaginaPredeterminado
            : Math.Min(tamano, opciones.TamanoPaginaMaximo);

        return (paginaSegura, tamanoSeguro);
    }
}
