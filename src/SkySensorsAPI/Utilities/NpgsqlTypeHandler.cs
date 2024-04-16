using Dapper;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace SkySensorsAPI.Utilities;

/// <summary>
/// Used to convert parameters as <typeparamref name="T"/> to <paramref name="dbType"/> and used to insert or read <typeparamref name="T"/> as <paramref name="dbType"/> into or from the database 
/// </summary>
/// <typeparam name="T"> C# type the <paramref name="dbType"/> should be parsed to </typeparam>
/// <param name="dbType">Postgres data type <typeparamref name="T"/> should be handled like</param>
public class NpgsqlTypeHandler<T>(NpgsqlDbType dbType) : SqlMapper.TypeHandler<T>
{
	public override T? Parse(object value)
	{
		if (value == null || value == DBNull.Value)
		{
			return default;
		}
		if (value is not T)
		{
			throw new ArgumentException($"Unable to convert {value.GetType().FullName} to {typeof(T).FullName}", nameof(value));
		}
		T result = (T)value;
		return result;
	}

	public override void SetValue(IDbDataParameter parameter, T? value)
	{
		parameter.Value = value;
		parameter.DbType = DbType.Object;
		if (parameter is NpgsqlParameter npgsqlPara)
		{
			npgsqlPara.NpgsqlDbType = dbType;
		}
	}
}
