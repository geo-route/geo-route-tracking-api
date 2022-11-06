using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;
using GeoRoute.Platform.Tracking.DataMapping.Services;

namespace GeoRoute.Platform.Tracking.DataMapping.Builders;

public class TvpArgumentBuilder
{
    private readonly DataTable _table;
    private IList<TvpMetaData> _metaData;
    private string _argumentName;

    public TvpArgumentBuilder()
    {
        this._table = new DataTable();
        this._argumentName = string.Empty;
        this._metaData = new List<TvpMetaData>();
    }

    public TvpArgumentBuilder WithObject<TValue>(TValue value)
    {
        this.CheckType(value?.GetType());

        foreach(var data in this._metaData) {
            this.AddTvpColumn(value!.GetType(), data.ColumnName);
        }

        this.AddTvpRow(value);
        return this;
    }

    public TvpArgumentBuilder WithEnumerable(IEnumerable? enumerable)
    {
        if(enumerable == null) {
            throw new ArgumentException("Cannot add a NULL enumerable", nameof(enumerable));
        }

        var genericArguments = enumerable.GetType().GetGenericArguments();

        if(genericArguments.Length == 0) {
            throw new ArgumentException("Enumerable has no type arguments", nameof(enumerable));
        }

        this.InternalWithEnumerable(enumerable, genericArguments[0]);
        return this;
    }

    public TvpArgumentBuilder WithArgumentName(string? argumentName)
    {
        if(string.IsNullOrEmpty(argumentName)) {
            throw new ArgumentNullException(nameof(argumentName), "The argument is null or empty");
        }

        this._argumentName = $@"{argumentName}";
        return this;
    }

    private void InternalWithEnumerable(IEnumerable enumerable, Type type)
    {
        this.CheckType(type);

        foreach(var data in this._metaData) {
            this.AddTvpColumn(type, data.ColumnName);
        }

        foreach (var obj in enumerable) {
            this.AddTvpRow(obj);
        }
    }

    public DynamicParameters Build()
    {
        var bag = new DynamicParameters();

        if(string.IsNullOrEmpty(this._argumentName)) {
            throw new InvalidOperationException("A TVP argument needs a name");
        }

        bag.Add(this._argumentName, this._table, DbType.Object);
        return bag;
    }

    private void AddTvpRow<TValue>(TValue @object)
    {
        var row = this._table.NewRow();

        foreach(var propertyInfo in this._metaData.Select(info => GetPropertyInfoFromMetaData(@object?.GetType(), info))) {
            var value = propertyInfo.GetValue(@object);
            row[propertyInfo.Name] = value ?? DBNull.Value;
        }

        this._table.Rows.Add(row);
    }

    private static PropertyInfo GetPropertyInfoFromMetaData(Type? type, TvpMetaData metaData)
    {
        var info = type?.GetProperty(metaData.ColumnName, BindingFlags.Instance | BindingFlags.Public);

        if(info == null) {
            throw new InvalidOperationException($"Unable to find property {metaData.ColumnName} in {type?.Name}");
        }

        return info;
    }

    private void AddTvpColumn(Type type, string metaData)
    {
        var propertyInfo = type.GetProperty(metaData, BindingFlags.Instance | BindingFlags.Public);

        if(propertyInfo == null) {
            throw new InvalidOperationException($"Unable to find property \"{metaData}\" in {type.Name}");
        }

        if (propertyInfo.PropertyType.GenericTypeArguments.Length > 0 && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
            var underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);

            if (underlyingType == null) {
                throw new InvalidOperationException("Nullable property has no underlying type.");
            }

            var column = this._table.Columns.Add(propertyInfo.Name, underlyingType);
            column.AllowDBNull = true;
        } else {
            this._table.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
        }
    }

    private void CheckType(Type? type)
    {
        if (type == null) {
            throw new ArgumentNullException(nameof(type));
        }

        foreach (var metaData in this._metaData.Select(x => x.ColumnName)) {
            var propertyInfo = type.GetProperty(metaData, BindingFlags.Instance | BindingFlags.Public);

            if(propertyInfo == null) {
                throw new InvalidOperationException($"Unable to map property {metaData} to {type.Name}");
            }
        }
    }

    public TvpArgumentBuilder WithTvpMetaData(IEnumerable<TvpMetaData> data)
    {
        this._metaData = data.OrderBy(md => md.Order).ToList();
        return this;
    }
}
