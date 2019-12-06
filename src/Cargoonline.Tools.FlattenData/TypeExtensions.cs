using System;
using System.Reflection;

namespace Cargoonline.Tools.FlattenData
{
    internal static class TypeExtensions
    {
        public static bool CorrectFor(this Type currentType, Type expectedType)
        {
            return expectedType.EqualTo(currentType) || expectedType.UnderlyingTypeEqualTo(currentType);
        }

        private static bool EqualTo(this Type t1, Type t2)
        {
            return t1 == t2 || t2.GetTypeInfo().IsSubclassOf(t1);
        }

        private static bool UnderlyingTypeEqualTo(this Type t1, Type t2)
        {
            return t1.GetTypeInfo().IsGenericType && t1.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                   (Nullable.GetUnderlyingType(t1) == t2 || t2.GetTypeInfo().IsSubclassOf(Nullable.GetUnderlyingType(t1)));
        }
    }
}