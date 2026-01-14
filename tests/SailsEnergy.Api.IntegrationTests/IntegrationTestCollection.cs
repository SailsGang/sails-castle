using SailsEnergy.Api.IntegrationTests.Fixtures;

namespace SailsEnergy.Api.IntegrationTests;

[CollectionDefinition("Integration")]
public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}
