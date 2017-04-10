using System;

namespace Cargoonline.Tools.FlattenData.Tests.TestModels
{
    public static class SetExtensions
    {
        public static void SetAdditional<TValue>(this IAdditionalEntity entity, FooAdditionalType type, TValue value)
        {
            var wrapper = entity.GetAdditionalWrapper();
            wrapper.Set(type, value);
        }

        public static void SetAdditional<TValue>(this IAdditionalEntity entity, Enum type, TValue value)
        {
            var wrapper = entity.GetAdditionalWrapper();
            wrapper.Set(type, value);
        }
    }
}