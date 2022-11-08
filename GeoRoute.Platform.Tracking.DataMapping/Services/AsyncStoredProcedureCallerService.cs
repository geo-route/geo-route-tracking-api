using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using GeoRoute.Platform.Tracking.DataMapping.Abstract;

namespace GeoRoute.Platform.Tracking.DataMapping.Services;

public sealed class AsyncStoredProcedureCallerService : StoredProcedureCallerBase, IAsyncStoredProcedureCaller
{
    private const string AsyncSuffix = "Async";

    public AsyncStoredProcedureCallerService(IMSSqlConnectionService service, string connectionString) : base(service, connectionString)
    {
    }

    public async Task<TValue> QueryFirstAsync<TValue>(string procedure, object[] args) where TValue : class
    {
        this.VerifyConnection();
        var procedureName = HandleAsyncSuffix(procedure);

        var @params = this.BuildDynamicParametersBag(args);
        return await this._connection
            .QuerySingleOrDefaultAsync<TValue>($"[dbo].[{procedureName}]", @params, commandType: CommandType.StoredProcedure)
            .ConfigureAwait(false);
    }

    public async Task ExecuteQueryAsync(string procedure, object[] args)
    {
        this.VerifyConnection();
        var procedureName = HandleAsyncSuffix(procedure);

        var @params = this.BuildDynamicParametersBag(args);
        await this._connection.ExecuteAsync($"[dbo].[{procedureName}]", @params, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
    }

    public async Task<IEnumerable<TValue>> QueryAsync<TValue>(string procedure, object[] args) where TValue : class
    {
        this.VerifyConnection();
        var procedureName = HandleAsyncSuffix(procedure);

        var @params = this.BuildDynamicParametersBag(args);
        return await this._connection.QueryAsync<TValue>($"[dbo].[{procedureName}]", @params, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
    }

    private static string HandleAsyncSuffix(string procedureName)
    {
        if(procedureName.EndsWith(AsyncSuffix)) {
            return procedureName[..^AsyncSuffix.Length];
        }

        return procedureName;
    }
}
