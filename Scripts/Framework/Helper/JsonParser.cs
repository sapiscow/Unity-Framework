using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sapiscow
{
    public static class JsonParser
    {
        #region JSON Settings
        private static readonly JsonSerializerSettings _defaultSettings = new()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new JsonCustomResolver()
        };

        private static readonly JsonSerializerSettings _indentedSettings = new()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new JsonCustomResolver(),
            Formatting = Formatting.Indented
        };
        #endregion

        /// <summary>
        /// Generate a JSON representation of the public fields of an object.
        /// </summary>
        public static string ToJson(object obj, bool isPrettyPrint = false)
            => JsonConvert.SerializeObject(obj, isPrettyPrint ? _indentedSettings : _defaultSettings);

        /// <summary>
        /// Create an object from its JSON representation.
        /// </summary>
        public static T FromJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json)) return default;

            return JsonConvert.DeserializeObject<T>(json, _defaultSettings);
        }

        /// <summary>
        /// Overwrite data in an object by reading from its JSON representation.
        /// </summary>
        public static void FromJsonOverwrite<T>(string json, T obj)
        {
            if (!string.IsNullOrEmpty(json))
                JsonConvert.PopulateObject(json, obj, _defaultSettings);
        }

        internal class JsonCustomResolver : DefaultContractResolver
        {
            /// <summary>
            /// Enable Property Serialization.
            /// </summary>
            protected override IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
            {
                List<JsonProperty> properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => base.CreateProperty(p, memberSerialization))
                    .Union(type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                        .Select(f => base.CreateProperty(f, memberSerialization)))
                    .ToList();

                properties.ForEach(p => { p.Writable = true; p.Readable = true; });

                return properties;
            }

            /// <summary>
            /// Enable Dictionary Serialization.
            /// </summary>
            protected override JsonDictionaryContract CreateDictionaryContract(System.Type objectType)
            {
                JsonDictionaryContract contract = base.CreateDictionaryContract(objectType);
                System.Type keyType = contract.DictionaryKeyType;

                if (keyType.BaseType == typeof(System.Enum))
                    contract.DictionaryKeyResolver = propName => ((int)System.Enum.Parse(keyType, propName)).ToString();

                return contract;
            }
        }
    }
}