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
	public async Task<T> ExecuteQueryAsync<T>(Func<IDbConnection, Task<T>> query)
	{
		DefaultTypeMap.MatchNamesWithUnderscores = true;
		SqlMapper.AddTypeHandler(new NpgsqlTypeHandler<PhysicalAddress>(NpgsqlDbType.MacAddr));
		using IDbConnection con = new NpgsqlConnection(configuration.GetConnectionString("Postgres"));
		return await query(con);
	}
}