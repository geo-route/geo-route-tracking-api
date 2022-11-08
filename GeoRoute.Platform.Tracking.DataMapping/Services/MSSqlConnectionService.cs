using System.Data.SqlClient;
using GeoRoute.Platform.Tracking.DataMapping.Abstract;

namespace GeoRoute.Platform.Tracking.DataMapping.Services;

public class MSSqlConnectionService : IMSSqlConnectionService
{
    public SqlConnection CreateConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
    }
}
