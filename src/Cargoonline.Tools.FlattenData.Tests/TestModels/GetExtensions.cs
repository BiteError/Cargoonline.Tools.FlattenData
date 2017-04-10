using System;

namespace Cargoonline.Tools.FlattenData.Tests.TestModels
{
    public static class GetExtensions
    {
        public static T GetAdditional<T>(this IAdditionalEntity entity, Enum settingType)
        {
            return entity.GetAdditionalWrapper().Get<IAdditionalEntity, T>(settingType, null);
        }

        public static T GetAdditional<T>(this IAdditionalEntity entity, Enum settingType, FlattenDataProvider<IAdditionalEntity> provider)
        {
            return entity.GetAdditionalWrapper().Get<IAdditionalEntity, T>(settingType, provider);
        }
    }
}