using ErosionFinder.Extensions;
using ErosionFinder.Data.Exceptions;
using ErosionFinder.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace ErosionFinder.Extensions.Tests
{
    public class NamespacesGroupingMethodExtensionsTest
    {
        [Fact]
        [Trait(nameof(NamespacesGroupingMethodExtensions.CheckIfItsValid), "ExplicitlyGrouped_Error_NullReference")]
        public void CheckIfItsValid_ExplicitlyGrouped_Error_NullReference()
        {
            NamespacesExplicitlyGrouped namespacesExplicitlyGrouped = null;
            
            var result = Record.Exception(() => 
            {
                namespacesExplicitlyGrouped.CheckIfItsValid("key");
            });

            Assert.Null(result);
        }

        [Fact]
        [Trait(nameof(NamespacesGroupingMethodExtensions.CheckIfItsValid), "ExplicitlyGrouped_Error_EmptyNamespaces")]
        public void CheckIfItsValid_ExplicitlyGrouped_Error_EmptyNamespaces()
        {
            var key = "key";
            var namespacesExplicitlyGrouped = new NamespacesExplicitlyGrouped(new List<string>());
            
            var result = Assert.Throws<ConstraintsException>(() => 
            {
                namespacesExplicitlyGrouped.CheckIfItsValid(key);
            });

            Assert.Equal(ConstraintsError.NamespaceNotFoundForLayer(key).Key, result.Key);
        }

        [Fact]
        [Trait(nameof(NamespacesGroupingMethodExtensions.CheckIfItsValid), "ExplicitlyGrouped_Success")]
        public void CheckIfItsValid_ExplicitlyGrouped_Success()
        {
            var namespaces = new List<string>() { "test" };
            var namespacesExplicitlyGrouped = new NamespacesExplicitlyGrouped(namespaces);
            
            var result = Record.Exception(() => 
            {
                namespacesExplicitlyGrouped.CheckIfItsValid("key");
            });

            Assert.Null(result);
        }

        [Fact]
        [Trait(nameof(NamespacesGroupingMethodExtensions.CheckIfItsValid), "RegExGrouped_Error_NullReference")]
        public void CheckIfItsValid_RegExGrouped_Error_NullReference()
        {
            NamespacesRegularExpressionGrouped namespacesRegExGrouped = null;
            
            var result = Record.Exception(() => 
            {
                namespacesRegExGrouped.CheckIfItsValid("key", new List<string>());
            });

            Assert.Null(result);
        }

        [Fact]
        [Trait(nameof(NamespacesGroupingMethodExtensions.CheckIfItsValid), "RegExGrouped_Error_EmptyNamespaces")]
        public void CheckIfItsValid_RegExGrouped_Error_EmptyNamespaces()
        {
            var key = "key";
            var regEx = new Regex(@"(Test)(.+)(\w*(Service([s]{1})?)\b)");
            var namespacesRegExGrouped = new NamespacesRegularExpressionGrouped(regEx);
            
            var result = Assert.Throws<ConstraintsException>(() => 
            {
                namespacesRegExGrouped.CheckIfItsValid(key, new List<string>());
            });

            Assert.Equal(ConstraintsError.NamespaceNotFoundForLayer(key).Key, result.Key);
        }

        [Fact]
        [Trait(nameof(NamespacesGroupingMethodExtensions.CheckIfItsValid), "RegExGrouped_Success")]
        public void CheckIfItsValid_RegExGrouped_Success()
        {
            var namespaces = new List<string>() { "Test.Service" };
            var regEx = new Regex(@"(Test)(.+)(\w*(Service([s]{1})?)\b)");
            var namespacesRegExGrouped = new NamespacesRegularExpressionGrouped(regEx);
            
            var result = Record.Exception(() => 
            {
                namespacesRegExGrouped.CheckIfItsValid("key", namespaces);
            });

            Assert.Null(result);
        }
    }
}
