using SkySensorsAPI.Models.DTO;
using SkySensorsAPI.Models.Infrastructure;
using SkySensorsAPI.Repositories;
using System.Net.NetworkInformation;

namespace SkySensorsAPI.DomainServices;

public interface ITimeSlotDomainService
{
	public Task<TimeSlotDTO> UpsertTimeSlot(PhysicalAddress macAddress);
}

public class TimeSlotDomainService(ITimeSlotRepository timeSlotRepository) : ITimeSlotDomainService
{
	public async Task<TimeSlotDTO> UpsertTimeSlot(PhysicalAddress macAddress)
	{
		// Find the time schedule that would fit for this device
		// Check if schedule already exists for this device
		TimeSlot? timeSlot = await timeSlotRepository.GetMacAddressTimeSlot(macAddress);

		// If timeslot already exists, then return it
		if (timeSlot != null)
		{
			return new TimeSlotDTO { SecondsNumber = timeSlot.Value.SecondsNumber };
		}

		// If not, then find the least used timeslot
		int secondsNumber = await timeSlotRepository.GetBestTimeSlot();

		await timeSlotRepository.InsertTimeSlot(macAddress, secondsNumber);

		return new TimeSlotDTO { SecondsNumber = secondsNumber };
	}
}
