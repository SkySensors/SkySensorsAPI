using Npgsql;
using System.Data;

namespace SkySensorsAPI.InfrastureServices;

public interface IPostgreSqlInfrastureService
{
	T ExecuteQueryAsync<T>(Func<IDbConnection, T> query);
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

	public T ExecuteQueryAsync<T>(Func<IDbConnection, T> query)
	{
		try
		{
			using IDbConnection con = new NpgsqlConnection(configuration.GetConnectionString("Postgres"));
			return query(con);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error executing query in database");
			return default;
		}
	}
}