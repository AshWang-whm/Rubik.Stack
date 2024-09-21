using System;
using System.Linq;
using System.Reflection;

namespace Rubik.Infrastructure.Utils.Common.Types
{
    public static class TypesUtil
    {
        public static bool IsNullableType(this Type type)
        => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static bool IsConcrete(this Type type, bool isContainAbstract = false)
        {
            if (isContainAbstract)
                return type is { IsGenericTypeDefinition: false, IsInterface: false };

            return type is { IsGenericTypeDefinition: false, IsInterface: false, IsAbstract: false };
        }

        public static bool IsImplementerOfGeneric(this Type type, Type genericType)
        {
            if (!genericType.GetTypeInfo().IsGenericType)
                return false;

            if (genericType.IsInterface)
                return type.IsImplementerOfGenericInterface(genericType);

            return type.IsImplementerOfGenericClass(genericType);
        }

        private static bool IsImplementerOfGenericInterface(this Type type, Type genericType)
        {
            return type.GetInterfaces().Any(interfaceType =>
            {
                var current = interfaceType.GetTypeInfo().IsGenericType ?
                    interfaceType.GetGenericTypeDefinition() : interfaceType;
                return current == genericType;
            });
        }

        private static bool IsImplementerOfGenericClass(this Type type, Type genericType)
        {
            var currentType = type.GetTypeInfo().IsGenericType ?
                type.GetGenericTypeDefinition() : type;
            if (currentType == genericType)
                return true;

            var baseType = currentType.BaseType;
            if (baseType == null)
                return false;

            return baseType.IsImplementerOfGenericClass(genericType);
        }
    }
}
