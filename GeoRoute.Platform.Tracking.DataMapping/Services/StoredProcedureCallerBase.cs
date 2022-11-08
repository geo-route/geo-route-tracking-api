using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Dapper;
using GeoRoute.Platform.Tracking.DataMapping.Abstract;
using GeoRoute.Platform.Tracking.DataMapping.Attributes;
using GeoRoute.Platform.Tracking.DataMapping.Builders;
using JetBrains.Annotations;

namespace GeoRoute.Platform.Tracking.DataMapping.Services;

[PublicAPI]
public abstract class StoredProcedureCallerBase : IDisposable
{
    protected readonly SqlConnection _connection;

    protected StoredProcedureCallerBase(IMSSqlConnectionService connectionService, string connectionString)
    {
        this._connection = connectionService.CreateConnection(connectionString);
    }

    private IEnumerable<TvpMetaData> GetMetaData(string name)
    {
        DataTable? table;

        using(var cmd = new SqlCommand()) {
            table = this.GetSchemaTable(name, cmd);
        }

        if(table == null) {
            throw new InvalidOperationException("Unable to read meta data. Result is not available");
        }

        return GetTvpMetaData(table);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S108:Nested blocks of code should not be left empty", Justification = "Need to read the reader to the end.")]
    private DataTable? GetSchemaTable(string name, SqlCommand cmd)
    {
        cmd.Connection = this._connection;
        cmd.CommandText = $"DECLARE @TVP {name}; SELECT TOP(0) * FROM @TVP";
        cmd.CommandType = CommandType.Text;

        using var reader = cmd.ExecuteReader();
        while(reader.Read()) { }

        return reader.GetSchemaTable();
    }

    private static IEnumerable<TvpMetaData> GetTvpMetaData(DataTable data)
    {
        var metaData = new List<TvpMetaData>();
        var idx = 0;

        foreach(DataRow row in data.Rows) {
            var entry = new TvpMetaData {
                Order = idx++,
                ColumnName = (string)row["ColumnName"]
            };
            metaData.Add(entry);
        }

        return metaData;
    }

    protected DynamicParameters BuildDynamicParametersBag(IReadOnlyList<object> args)
    {
        if(args.Count < 2) {
            throw new InvalidOperationException("Need at least a parameter name and its value");
        }

        return this.InternalBuildParameterBag(args);
    }

    private DynamicParameters InternalBuildParameterBag(IReadOnlyList<object> args)
    {
        DynamicParameters bag;

        if(IsTvpArgument(args)) {
            bag = this.CreateTvpParameterBag(args);
        } else if(IsEnumerableArgument(args)) {
            bag = this.CreateEnumerableParameterBag(args);
        } else {
            bag = BuildParameterBag(args);
        }

        return bag;
    }

    private DynamicParameters CreateTvpParameterBag(IReadOnlyList<object> args)
    {
        var builder = new TvpArgumentBuilder();
        var data = this.GetMetaData(GetTvpNameFromType(args[1].GetType()));

        return builder.WithTvpMetaData(data)
            .WithObject(args[1])
            .WithArgumentName(args[0].ToString())
            .Build();
    }

    private DynamicParameters CreateEnumerableParameterBag(IReadOnlyList<object> args)
    {
        var builder = new TvpArgumentBuilder();
        var enumerable = (IEnumerable)args[1];
        var type = enumerable.GetType().GenericTypeArguments[0];
        var name = GetTvpNameFromType(type);
        var data = this.GetMetaData(name);

        return builder.WithArgumentName(args[0].ToString()!)
            .WithTvpMetaData(data)
            .WithEnumerable(enumerable)
            .Build();
    }

    private static string GetTvpNameFromType(MemberInfo type)
    {
        if(Attribute.GetCustomAttribute(type, typeof(TableValuedParameterAttribute)) is not TableValuedParameterAttribute attr) {
            throw new InvalidOperationException($"TableValuedParameter attribute is missing on {type.Name}");
        }

        return attr.Name;
    }

    private static DynamicParameters BuildParameterBag(IReadOnlyList<object> args)
    {
        var builder = new ArgumentBuilder();

        for(var idx = 0; idx < args.Count; idx += 2) {
            builder.WithArgument(args[idx].ToString()!, args[idx + 1]);
        }

        return builder.Build();
    }

    private static bool IsTvpArgument(IReadOnlyList<object> args)
    {
        var argumentObject = args[1];
        return args.Count == 2 && Attribute.GetCustomAttribute(argumentObject.GetType(), typeof(TableValuedParameterAttribute)) != null;
    }

    private static bool IsEnumerableArgument(IReadOnlyList<object> args)
    {
        var argumentObject = args[1];
        return args.Count == 2 && argumentObject is IEnumerable && argumentObject.GetType().GenericTypeArguments.Length > 0;
    }

    protected void VerifyConnection()
    {
        if(this._connection.State == ConnectionState.Open) {
            return;
        }

        this._connection.Open();
    }

    protected virtual void Dispose(bool disposing)
    {
        if(!disposing) {
            return;
        }

        this._connection.Dispose();
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
