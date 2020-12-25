using ErosionFinder.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

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
            
            if (IsPropertyType<IEnumerable<NonConformingRelation>>(property))
            {
                property.ShouldSerialize = 
                    instance => ShouldSerializeNonConformingRelations(instance);
            }

            if (IsPropertyType<IEnumerable<ArchitecturalRule>>(property))
            {
                property.ShouldSerialize =
                    instance => ShouldSerializeFollowedRules(instance);
            }

            if (IsPropertyType<IEnumerable<ArchitecturalViolationOccurrence>>(property))
            {
                property.ShouldSerialize =
                    instance => ShouldSerializeTransgressedRules(instance);
            }

            return property;
        }

        private bool ShouldSerializeNonConformingRelations(object instance)
        {
            if (instance is ArchitecturalViolationOccurrence occurrence)
                return occurrence.NonConformingRelations.Any();

            return true;
        }

        private bool ShouldSerializeFollowedRules(object instance)
        {
            if (instance is ArchitecturalConformanceCheck occurrence)
                return occurrence.FollowedRules.Any();

            return true;
        }

        private bool ShouldSerializeTransgressedRules(object instance)
        {
            if (instance is ArchitecturalConformanceCheck occurrence)
                return occurrence.TransgressedRules.Any();

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsPropertyType<T>(JsonProperty prop) where T : class
            => prop.PropertyType.Equals(typeof(T));
    }
}