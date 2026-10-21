using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ControlVehicular.Data;

public static class ParametrosSql
{
    public static DynamicParameters Crear() => new();

    public static DynamicParameters AddNVarChar(this DynamicParameters parametros, string nombre, string? valor, int longitud)
    {
        parametros.Add(nombre, new ParametroSqlTipado(new SqlParameter(nombre, SqlDbType.NVarChar, longitud)
        {
            Value = valor is null ? DBNull.Value : valor
        }));
        return parametros;
    }

    public static DynamicParameters AddInt(this DynamicParameters parametros, string nombre, int? valor)
    {
        parametros.Add(nombre, new ParametroSqlTipado(new SqlParameter(nombre, SqlDbType.Int)
        {
            Value = valor is null ? DBNull.Value : valor.Value
        }));
        return parametros;
    }

    public static DynamicParameters AddDateTime2(this DynamicParameters parametros, string nombre, DateTime valor)
    {
        parametros.Add(nombre, new ParametroSqlTipado(new SqlParameter(nombre, SqlDbType.DateTime2)
        {
            Precision = 0,
            Value = valor
        }));
        return parametros;
    }

    public static DynamicParameters AddOutputInt(this DynamicParameters parametros, string nombre)
    {
        parametros.Add(nombre, dbType: DbType.Int32, direction: ParameterDirection.Output);
        return parametros;
    }

    private sealed class ParametroSqlTipado : SqlMapper.ICustomQueryParameter
    {
        private readonly SqlParameter _parametro;

        public ParametroSqlTipado(SqlParameter parametro)
        {
            _parametro = parametro;
        }

        public void AddParameter(IDbCommand command, string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _parametro.ParameterName = name;
            }

            command.Parameters.Add(_parametro);
        }
    }
}
