using Microsoft.Build.Locator;
using System;
using Xunit;

namespace ErosionFinder.Tests.Fixture
{
    public class MSBuildLocatorFixture : IDisposable
    {
        public MSBuildLocatorFixture()
        {
            Console.WriteLine("a");
            if (MSBuildLocator.CanRegister)
            {
                Console.WriteLine("b");
                MSBuildLocator.RegisterDefaults();
            }
        }

        public void Dispose()
        {
            Console.WriteLine("c");
            MSBuildLocator.Unregister();
            GC.SuppressFinalize(this);
        }
    }

    [CollectionDefinition("MSBuildCollection")]
    public class MSBuildCollection : ICollectionFixture<MSBuildLocatorFixture>
    { }
}