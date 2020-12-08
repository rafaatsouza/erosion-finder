using Microsoft.Build.Locator;
using System;
using Xunit;

namespace ErosionFinder.Tests.Fixture
{
    public class MSBuildLocatorFixture : IDisposable
    {
        public MSBuildLocatorFixture()
        {
            if (MSBuildLocator.CanRegister)
            {
                MSBuildLocator.RegisterDefaults();
            }
        }

        public void Dispose()
        {
            MSBuildLocator.Unregister();
            GC.SuppressFinalize(this);
        }
    }

    [CollectionDefinition("MSBuildCollection")]
    public class MSBuildCollection : ICollectionFixture<MSBuildLocatorFixture>
    { }
}