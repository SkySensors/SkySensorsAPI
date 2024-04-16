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
	bool ExecuteProcedure(Action<IDbConnection> proc);
}

public class PostgreSqlInfrastureService(
	IConfiguration configuration,
	ILogger<PostgreSqlInfrastureService> logger) : IPostgreSqlInfrastureService
{


	public bool ExecuteProcedure(Action<IDbConnection> proc)
	{
		try
		{
			Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
			SqlMapper.AddTypeHandler(new NpgsqlTypeHandler<PhysicalAddress>(NpgsqlDbType.MacAddr));
			using IDbConnection con = new NpgsqlConnection(configuration.GetConnectionString("Postgres"));
			proc(con);
			return true;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error executing procedure in database");
			return false;
		}
	}

	public async Task<T> ExecuteQueryAsync<T>(Func<IDbConnection, Task<T>> query)
	{
		try
		{
			Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
			SqlMapper.AddTypeHandler(new NpgsqlTypeHandler<PhysicalAddress>(NpgsqlDbType.MacAddr));
			using IDbConnection con = new NpgsqlConnection(configuration.GetConnectionString("Postgres"));
			return await query(con);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error executing query in database");
			return default;
		}
	}
}