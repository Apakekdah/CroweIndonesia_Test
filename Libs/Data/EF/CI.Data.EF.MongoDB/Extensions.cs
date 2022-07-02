using System;
using System.Collections.Concurrent;

namespace CI.Data.EF.MongoDB
{
    internal static class Extensions
    {

        private static readonly ConcurrentDictionary<Type, bool> IsSimpleTypeCache = new ConcurrentDictionary<Type, bool>();

        public static bool IsSimpleType(this Type type)
        {
            Func<Type, bool> IsNullableSimpleType = x =>
            {
                var underlyingType = GetNullable(x);
                return underlyingType != null && IsSimpleType(underlyingType);
            };

            return IsSimpleTypeCache.GetOrAdd(type, t =>
                type.IsPrimitive ||
                type.IsEnum ||
                type == typeof(string) ||
                type == typeof(decimal) ||
                type == typeof(DateTime) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(TimeSpan) ||
                type == typeof(Guid) ||
                IsNullableSimpleType(type));
        }

        public static bool IsNullable(this Type type)
        {
            var underlyingType = GetNullable(type);
            return underlyingType != null;
        }

        public static Type GetNullable(this Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType;
        }
    }
}