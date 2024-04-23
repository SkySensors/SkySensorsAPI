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
	private string connectionStr = "Username=" + Environment.GetEnvironmentVariable("DB_USERNAME") + 
		";Password=" + Environment.GetEnvironmentVariable("DB_PASS") + 
		";Host=" + Environment.GetEnvironmentVariable("DB_HOST") + 
		";Port=" + Environment.GetEnvironmentVariable("DB_PORT") + 
		";Database=" + Environment.GetEnvironmentVariable("DB_NAME") + 
		";Pooling=true;Connection Lifetime=0;Application Name=SkySensorAPI";
	public async Task<T> ExecuteQueryAsync<T>(Func<IDbConnection, Task<T>> query)
	{
		DefaultTypeMap.MatchNamesWithUnderscores = true;
		SqlMapper.AddTypeHandler(new NpgsqlTypeHandler<PhysicalAddress>(NpgsqlDbType.MacAddr));
		using IDbConnection con = new NpgsqlConnection(connectionStr);
		return await query(con);
	}
}
