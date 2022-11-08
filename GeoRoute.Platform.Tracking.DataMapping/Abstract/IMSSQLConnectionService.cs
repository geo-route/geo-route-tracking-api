using System.Data.SqlClient;

namespace GeoRoute.Platform.Tracking.DataMapping.Abstract;

public interface IMSSqlConnectionService
{
    SqlConnection CreateConnection(string connectionString);
}
