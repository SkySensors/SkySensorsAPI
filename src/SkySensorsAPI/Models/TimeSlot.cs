using System.Net.NetworkInformation;

namespace SkySensorsAPI.Models
{
	public class TimeSlot
	{
		public int SecondsNumber {  get; set; }
		public PhysicalAddress MacAddress {  get; set; }


		public TimeSlotDto ToTimeSlotDTO()
		{
			return new TimeSlotDto()
			{
				SecondsNumber = this.SecondsNumber
			};
		}
	}
}
