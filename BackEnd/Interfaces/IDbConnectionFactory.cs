using Microsoft.Data.SqlClient;

namespace ControlVehicular.Interfaces;

public interface IDbConnectionFactory
{
    SqlConnection CreateConnection();
    Task<SqlConnection> OpenConnectionAsync(CancellationToken cancellationToken = default);
}
