using Dapper;
using SkySensorsAPI.InfrastureServices;
using SkySensorsAPI.Models.Infrastructure;
using System.Net.NetworkInformation;

namespace SkySensorsAPI.Repositories;

public interface ITimeSlotRepository
{
	Task<TimeSlot?> GetMacAddressTimeSlot(PhysicalAddress macAddress);
	Task<int> GetBestTimeSlot();
	Task InsertTimeSlot(PhysicalAddress macAddress, int secondsNumber);
}

public class TimeSlotRepository(
	IPostgreSqlInfrastureService postgreSqlService) : ITimeSlotRepository
{
	/// <summary>
	/// Used to get time slot by mac address from database
	/// </summary>
	/// <returns>TimeSlot?</returns>
	public async Task<TimeSlot?> GetMacAddressTimeSlot(PhysicalAddress macAddress)
	{
		return await postgreSqlService.ExecuteQueryAsync(
			   (con) => con.QueryFirstOrDefaultAsync<TimeSlot>("select mac_address, seconds_number from time_slots ts where mac_address = @MacAddress;",
			   new
			   {
				   MacAddress = macAddress
			   }));
	}

	/// <summary>
	/// Used to get best time slot from database
	/// </summary>
	/// <returns>an integer which represents the second which the timeslot should be triegged at in a given inteval</returns>
	public async Task<int> GetBestTimeSlot()
	{
		return await postgreSqlService.ExecuteQueryAsync(
			   (con) => con.QueryFirstAsync<int>("SELECT get_possible_time_slot();"));
	}

	/// <summary>
	/// Used to insert time slot to database
	/// </summary>
	public async Task InsertTimeSlot(PhysicalAddress macAddress, int secondsNumber)
	{
		await postgreSqlService.ExecuteQueryAsync(
			   (con) => con.ExecuteAsync("INSERT INTO public.time_slots (mac_address, seconds_number) VALUES(@MacAddress, @SecondsNumber);",
			   new
			   {
				   MacAddress = macAddress,
				   SecondsNumber = secondsNumber
			   }
			   ));
	}
}