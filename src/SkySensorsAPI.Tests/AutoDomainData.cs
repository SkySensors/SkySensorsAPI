using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.NUnit3;

namespace SkySensorsAPI.Tests;

internal class AutoDomainDataAttribute : AutoDataAttribute
{
	public AutoDomainDataAttribute()
		: this(() => CreateDomainFixture())
	{
	}

	public AutoDomainDataAttribute(Func<IFixture> fixtureFactory)
		: base(fixtureFactory)
	{
	}

	public static IFixture CreateDomainFixture() =>
		new Fixture().Customize(new DomainCustomization());
}

internal class DomainCustomization : ICustomization
{
	public void Customize(IFixture fixture)
	{
		new CompositeCustomization(new AutoNSubstituteCustomization()).Customize(fixture);
	}
}
