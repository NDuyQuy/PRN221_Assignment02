using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace CoffeeShop.Helper
{
    public class JsonHelper
    {
        public static string SerializeObject(object obj)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = JsonUpperCaseContractResolver.Instance,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static T? DeserializeObject<T>(string json) where T:class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public class JsonUpperCaseContractResolver : DefaultContractResolver
    {
        public static readonly JsonUpperCaseContractResolver Instance = new();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            property.PropertyName = property.PropertyName?.ToUpperInvariant();
            return property;
        }
    }
}
