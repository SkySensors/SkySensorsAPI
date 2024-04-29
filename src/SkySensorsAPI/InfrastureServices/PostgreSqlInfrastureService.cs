using Dapper;
using Npgsql;
using NpgsqlTypes;
using SkySensorsAPI.Utilities;
using System.Data;
using System.Net.NetworkInformation;

namespace SkySensorsAPI.InfrastureServices;

public interface IPostgreSqlInfrastureService
{
	Task<T> ExecuteQueryAsync<T>(Func<IDbConnection, Task<T>> query);
}

public class PostgreSqlInfrastureService(IConfiguration configuration) : IPostgreSqlInfrastureService
{
	private readonly string connectionString = "Username=" + configuration.GetValue<string>("DB_USERNAME") +
		";Password=" + configuration.GetValue<string>("DB_PASS") +
		";Host=" + configuration.GetValue<string>("DB_HOST") +
		";Port=" + configuration.GetValue<string>("DB_PORT") +
		";Database=" + configuration.GetValue<string>("DB_NAME") +
		";Pooling=true;Connection Lifetime=0;Application Name=SkySensorAPI";

	/// <summary>
	/// Used to execute PostgreSql query
	/// </summary>
	/// <returns>generic datatype</returns>
	public async Task<T> ExecuteQueryAsync<T>(Func<IDbConnection, Task<T>> query)
	{
		DefaultTypeMap.MatchNamesWithUnderscores = true;
		SqlMapper.AddTypeHandler(new NpgsqlTypeHandler<PhysicalAddress>(NpgsqlDbType.MacAddr));
		using IDbConnection con = new NpgsqlConnection(connectionString);
		return await query(con);
	}
}
