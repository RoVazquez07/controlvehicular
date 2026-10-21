namespace ControlVehicular.Data;

public static class IdentificadoresSql
{
    private static readonly Dictionary<string, string> ColumnasOrden = new(StringComparer.OrdinalIgnoreCase)
    {
        ["id"] = "Id",
        ["placa"] = "Placa",
        ["marca"] = "Marca",
        ["modelo"] = "Modelo",
        ["anio"] = "Anio",
        ["estado"] = "Estado",
        ["fecharegistro"] = "FechaRegistroUtc"
    };

    private static readonly HashSet<string> EstadosPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "Activo",
        "Inactivo",
        "Mantenimiento"
    };

    public static bool TryObtenerColumnaOrden(string? solicitado, out string columnaSql)
    {
        if (!string.IsNullOrWhiteSpace(solicitado) && ColumnasOrden.TryGetValue(solicitado.Trim(), out var columna))
        {
            columnaSql = columna;
            return true;
        }

        columnaSql = "Id";
        return false;
    }

    public static string ObtenerDireccionOrden(string? solicitado) =>
        string.Equals(solicitado, "desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

    public static bool EsEstadoPermitido(string? estado) =>
        !string.IsNullOrWhiteSpace(estado) && EstadosPermitidos.Contains(estado.Trim());
}
