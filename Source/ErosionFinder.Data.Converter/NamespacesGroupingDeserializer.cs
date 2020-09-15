using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using ErosionFinder.Data.Models;

namespace ErosionFinder.Data.Converter
{
    public class NamespacesGroupingDeserializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => typeof(NamespacesGroupingMethod).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var groupingByRegularExpression = jsonObject.Properties()
                .Any(p => p.Name.Equals("NamespaceRegexPattern"));

            NamespacesGroupingMethod namespacesGroupingMethod;

            if (groupingByRegularExpression)
            {
                namespacesGroupingMethod = new NamespacesRegularExpressionGrouped();
            }
            else
            {
                namespacesGroupingMethod = new NamespacesExplicitlyGrouped();
            }

            serializer.Populate(jsonObject.CreateReader(), namespacesGroupingMethod);

            return namespacesGroupingMethod;
        }

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }
}