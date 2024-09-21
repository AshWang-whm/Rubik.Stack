using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rubik.Infrastructure.Utils.Common.Objects
{
    public static class ObjectUtil
    {
        public static T DeepCopy<T>(this T self)
            where T : class,new()
        {
            var serialized = JsonSerializer.Serialize(self);
            return JsonSerializer.Deserialize<T>(serialized)??new T();
        }
    }
}
