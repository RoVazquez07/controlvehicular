using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace ControlVehicular.Data;

public static class SqlScriptRunner
{
    public static async Task EjecutarConGoAsync(SqlConnection conexion, string script, CancellationToken cancellationToken = default)
    {
        var lotes = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        foreach (var lote in lotes)
        {
            var sql = lote.Trim();
            if (sql.Length == 0)
            {
                continue;
            }

            await using var comando = new SqlCommand(sql, conexion)
            {
                CommandTimeout = 60
            };
            await comando.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
