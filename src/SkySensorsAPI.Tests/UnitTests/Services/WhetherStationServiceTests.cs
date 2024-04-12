﻿using AutoFixture.NUnit3;
using FluentAssertions;
using NSubstitute;
using SkySensorsAPI.DataAccess.Repositories;
using SkySensorsAPI.Services;

namespace SkySensorsAPI.Tests.UnitTests.Services;

internal class WhetherStationServiceTests
{
	[Test, AutoDomainData]
	public async Task GetDummyValue_WhenEverythingIsValid_ReturnsTrue(
		[Frozen] IWheatherStationRepository wheatherStationRepository,
		WhetherStationSqlService sut)
	{
		// Arrange
		wheatherStationRepository.GetWheaterStation().Returns(new object());

		// Act
		bool result = await sut.GetDummyValue();

		// Assert
		result.Should().BeTrue();
		wheatherStationRepository.Received().GetWheaterStation().Should().NotBeNull();
	}
}
