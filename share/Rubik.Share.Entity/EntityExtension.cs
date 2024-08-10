using Rubik.Share.Entity.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rubik.Share.Entity
{
    public static class EntityExtension
    {
        static readonly JsonSerializerOptions jsonSerializerOptions = new() { ReferenceHandler = ReferenceHandler.IgnoreCycles };

        public static T TreeCopy<T>(this T obj) where T : class,ITreeEntity<T>,new()
        {
            var serialized = JsonSerializer.Serialize(obj, typeof(T), jsonSerializerOptions);
            return JsonSerializer.Deserialize<T>(serialized) ?? new T();
        }
    }
}
