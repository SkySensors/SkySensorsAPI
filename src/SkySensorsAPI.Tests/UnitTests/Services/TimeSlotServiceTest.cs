using AutoFixture.NUnit3;
using SkySensorsAPI.ApplicationServices;
using SkySensorsAPI.Models.DTO;
using SkySensorsAPI.Models.Infrastructure;
using SkySensorsAPI.Repositories;
using System.Net.NetworkInformation;
using NSubstitute;
using FluentAssertions;
using NSubstitute.ReturnsExtensions;

namespace SkySensorsAPI.Tests.UnitTests.Services;

internal class TimeSlotServiceTest
{
	private const int validSecondNumber = 9;
	private const string validMacAddressStr = "00:00:00:00:00:00";
	private readonly PhysicalAddress validMacAddress = PhysicalAddress.Parse(validMacAddressStr);
	private readonly TimeSlot validTimeSlot = new(PhysicalAddress.Parse(validMacAddressStr), validSecondNumber);
	private readonly TimeSlotDTO validTimeSlotDTO = new() { SecondsNumber = validSecondNumber }; // Needed to get the default interval 

	[Test, AutoDomainData]
	public async Task UpsertTimeSlot_WhenTimeSlotAlreadyExists_ReturnsTimeSlotDTO(
	[Frozen] ITimeSlotRepository timeSlotRepository,
	TimeSlotAppService sut)
	{
		// Arrange
		timeSlotRepository.GetMacAddressTimeSlot(validMacAddress)
			.Returns(validTimeSlot);

		// Act
		TimeSlotDTO result = await sut.UpsertTimeSlot(validMacAddress);

		// Assert
		result.Should().NotBeNull();
		result.SecondsNumber.Should().Be(validTimeSlot.SecondsNumber);
		result.IntervalSeconds.Should().Be(validTimeSlotDTO.IntervalSeconds);
		_ = timeSlotRepository.Received(1).GetMacAddressTimeSlot(validMacAddress);
		_ = timeSlotRepository.Received(0).GetBestTimeSlot();
		_ = timeSlotRepository.Received(0).InsertTimeSlot(validMacAddress, validSecondNumber);
	}

	[Test, AutoDomainData]
	public async Task UpsertTimeSlot_WhenTimeSlotNotExists_ReturnsTimeSlotDTO(
	[Frozen] ITimeSlotRepository timeSlotRepository,
	TimeSlotAppService sut)
	{
		// Arrange
		timeSlotRepository.GetMacAddressTimeSlot(validMacAddress)
			.ReturnsNull();
		timeSlotRepository.GetBestTimeSlot()
			.Returns(validSecondNumber);
		timeSlotRepository.InsertTimeSlot(validMacAddress, validSecondNumber)
			.Returns(Task.CompletedTask);

		// Act
		TimeSlotDTO result = await sut.UpsertTimeSlot(validMacAddress);

		// Assert
		result.Should().NotBeNull();
		result.SecondsNumber.Should().Be(validSecondNumber);
		result.IntervalSeconds.Should().Be(validTimeSlotDTO.IntervalSeconds);
		_ = timeSlotRepository.Received(1).GetMacAddressTimeSlot(validMacAddress);
		_ = timeSlotRepository.Received(1).GetBestTimeSlot();
		_ = timeSlotRepository.Received(1).InsertTimeSlot(validMacAddress, validSecondNumber);
	}
}
