using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rubik.Share.Utils.Objects
{
    public static class ObjectUtil
    {
        public static T DeepCopy<T>(this T self)
            where T : class, new()
        {
            var serialized = JsonSerializer.Serialize(self);
            return JsonSerializer.Deserialize<T>(serialized) ?? new T();
        }
    }
}
