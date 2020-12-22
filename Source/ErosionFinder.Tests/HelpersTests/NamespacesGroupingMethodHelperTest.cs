using ErosionFinder.Data.Exceptions;
using ErosionFinder.Data.Models;
using ErosionFinder.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace ErosionFinder.Extensions.Tests
{
    public class NamespacesGroupingMethodHelperTest
    {
        [Fact(DisplayName = "NamespacesGroupingMethodHelper GetLayersNamespaces - ExplicitlyGrouped - Success")]
        [Trait(nameof(NamespacesGroupingMethodHelper.GetLayersNamespaces), "ExplicitlyGrouped_Success")]
        public void GetLayersNamespaces_ExplicitlyGrouped_Success()
        {
            var key = "Origin";
            var namespaces = new List<string>() { "Test.Origin" };

            var layers = new Dictionary<string, NamespacesGroupingMethod>()
                { { key, new NamespacesExplicitlyGrouped(namespaces) } };

            var result = NamespacesGroupingMethodHelper
                .GetLayersNamespaces(layers, namespaces);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.True(result.ContainsKey(key));
            Assert.Single(result[key]);
            Assert.Equal(namespaces.Single(), result[key].Single());
        }

        [Fact(DisplayName = "NamespacesGroupingMethodHelper GetLayersNamespaces - ExplicitlyGrouped - Error: NamespaceNotFoundForLayer")]
        [Trait(nameof(NamespacesGroupingMethodHelper.GetLayersNamespaces), "ExplicitlyGrouped_Error_NamespaceNotFoundForLayer")]
        public void GetLayersNamespaces_ExplicitlyGrouped_Error_NamespaceNotFoundForLayer()
        {
            var key = "Origin";

            var layers = new Dictionary<string, NamespacesGroupingMethod>()
                { { key, new NamespacesExplicitlyGrouped() } };

            var result = Assert.Throws<ConstraintsException>(() => 
            {
                _ = NamespacesGroupingMethodHelper.GetLayersNamespaces(
                    layers, new List<string>());
            });

            Assert.Equal(ConstraintsError.NamespaceNotFoundForLayer(key).Key, result.Key);
        }

        [Fact(DisplayName = "NamespacesGroupingMethodHelper GetLayersNamespaces - RegExGrouped - Success")]
        [Trait(nameof(NamespacesGroupingMethodHelper.GetLayersNamespaces), "RegExGrouped_Success")]
        public void GetLayersNamespaces_RegExGrouped_Success()
        {
            var key = "Origin";
            var namespaces = new List<string>() { "Test.Origin" };
            var regEx = new Regex(@"(Test)(.+)(\w*(Origin([s]{1})?)\b)");

            var layers = new Dictionary<string, NamespacesGroupingMethod>()
                { { key, new NamespacesRegularExpressionGrouped(regEx) } };

            var result = NamespacesGroupingMethodHelper
                .GetLayersNamespaces(layers, namespaces);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.True(result.ContainsKey(key));
            Assert.Single(result[key]);
            Assert.Equal(namespaces.Single(), result[key].Single());
        }

        [Fact(DisplayName = "NamespacesGroupingMethodHelper GetLayersNamespaces - RegExGrouped - Error: NamespaceNotFoundForLayer")]
        [Trait(nameof(NamespacesGroupingMethodHelper.GetLayersNamespaces), "RegExGrouped_Error_NamespaceNotFoundForLayer")]
        public void GetLayersNamespaces_RegExGrouped_Error_NamespaceNotFoundForLayer()
        {
            var key = "Origin";

            var layers = new Dictionary<string, NamespacesGroupingMethod>()
                { { key, new NamespacesRegularExpressionGrouped() } };

            var result = Assert.Throws<ConstraintsException>(() => 
            {
                _ = NamespacesGroupingMethodHelper.GetLayersNamespaces(
                    layers, new List<string>());
            });

            Assert.Equal(ConstraintsError.NamespaceNotFoundForLayer(key).Key, result.Key);
        }
    }
}
