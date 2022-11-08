using System.Collections.Generic;
using System.Data;
using Dapper;
using GeoRoute.Platform.Tracking.DataMapping.Abstract;

namespace GeoRoute.Platform.Tracking.DataMapping.Services;

public sealed class StoredProcedureCallerService : StoredProcedureCallerBase, IStoredProcedureCaller
{
    public StoredProcedureCallerService(IMSSqlConnectionService connectionService, string connectionString) : base(connectionService, connectionString)
    {
    }

    public TValue QueryFirst<TValue>(string procedure, object[] args) where TValue : class
    {
        this.VerifyConnection();

        var parameters = this.BuildDynamicParametersBag(args);
        return this._connection.QuerySingleOrDefault<TValue>($"[dbo].[{procedure}]",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public void ExecuteStoredProcedure(string procedure, object[] args)
    {
        this.VerifyConnection();

        var parameters = this.BuildDynamicParametersBag(args);
        this._connection.Execute($"[dbo]].[{procedure}]", parameters, commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<TValue> Stream<TValue>(string procedure, object[] args) where TValue : class
    {
        this.VerifyConnection();

        var parameters = this.BuildDynamicParametersBag(args);
        return this._connection.Query<TValue>($"[dbo].[{procedure}]",
            parameters,
            buffered: false,
            commandType: CommandType.StoredProcedure);
    }
}
