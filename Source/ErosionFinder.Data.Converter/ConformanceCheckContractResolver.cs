using ErosionFinder.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ErosionFinder.Data.Converter
{
    public class ConformanceCheckContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(
            Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization)
                .OrderBy(p => p.PropertyName).ThenBy(p => p.UnderlyingName)
                .ToList();
        }

        protected override JsonProperty CreateProperty(
            MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(
                member, memberSerialization);
            
            if (property.PropertyType.Equals(typeof(IEnumerable<NonConformingRelation>)))
            {
                property.ShouldSerialize =
                    instance =>
                    {
                        if (instance is ArchitecturalViolationOccurrence occurrence)
                            return occurrence.NonConformingRelations.Any();

                        return true;
                    };
            }

            return property;
        }
    }
}