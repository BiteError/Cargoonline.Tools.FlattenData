using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Cargoonline.Tools.FlattenData
{
    public static class EnumExtensions
    {
        private static readonly ConcurrentDictionary<Enum, TypeConstraintAttribute> Attributes =
            new ConcurrentDictionary<Enum, TypeConstraintAttribute>();

        private static readonly Dictionary<Enum, Dictionary<Type, bool>> RelatedToTypes =
            new Dictionary<Enum, Dictionary<Type, bool>>();

        public static TReturn GetAttributeOfType<TEnum, TReturn>(this TEnum enumVal) where TReturn : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(TReturn), false);
            return (attributes.Length > 0) ? (TReturn)attributes[0] : null;
        }

        public static void CheckValueType<TEnum, TValue>(this TEnum valueType)
        {
            valueType.CheckValueType(typeof(TValue));
        }

        public static void CheckValueType<TEnum>(this TEnum valueType, Type currentType)
        {
            var expectedType = valueType.GetExpectedType();

            if (currentType.CorrectFor(expectedType))
            {
                return;
            }

            throw new ArgumentException("Incorrect type", valueType.ToString());
        }

        public static bool RelatedToTypeOfEntity<T>(this Enum valueType, T entity)
        {
            if (!RelatedToTypes.ContainsKey(valueType))
            {
                RelatedToTypes[valueType] = new Dictionary<Type, bool>();
            }

            var attributeOfType = GetTypeConstraintAttribute(valueType);

            Type entityType = entity.GetType();

            if (!RelatedToTypes[valueType].ContainsKey(entityType))
            {
                RelatedToTypes[valueType][entityType] = !attributeOfType.ForEntitiesTypes.Any() ||
                                                  attributeOfType.ForEntitiesTypes.Any(
                                                      t => entityType.IsSubclassOf(t) || entityType == t);
            }

            return RelatedToTypes[valueType][entityType];
        }

        private static TypeConstraintAttribute GetTypeConstraintAttribute<TEnum>(TEnum valueType)
        {
            return GetTypeConstraintAttribute(valueType as Enum);
        }

        private static TypeConstraintAttribute GetTypeConstraintAttribute(Enum valueType)
        {
            if (!Attributes.ContainsKey(valueType))
            {
                var addedConstraint = valueType.GetAttributeOfType<Enum, TypeConstraintAttribute>();
                Attributes.TryAdd(valueType, addedConstraint);
            }

            var attributeOfType = Attributes[valueType];
            return attributeOfType;
        }

        public static Type GetValueType<TEnum>(this TEnum valueType)
        {
            var attributeOfType = GetTypeConstraintAttribute(valueType);
            return attributeOfType.ValueType;
        }

        public static Type GetExpectedType<TEnum>(this TEnum valueType)
        {
            var attributeOfType = GetTypeConstraintAttribute(valueType);
            var expectedType = attributeOfType.ValueType;
            return expectedType;
        }

        public static bool IsNullable<TEnum>(this TEnum enumVal)
        {
            var type = GetTypeConstraintAttribute(enumVal).ValueType;
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}