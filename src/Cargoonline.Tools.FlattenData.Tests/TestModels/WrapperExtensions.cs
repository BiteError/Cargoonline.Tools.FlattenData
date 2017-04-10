namespace Cargoonline.Tools.FlattenData.Tests.TestModels
{
    static class WrapperExtensions
    {
        public static FlatDataWrapper<IAdditionalEntity> GetAdditionalWrapper(this IAdditionalEntity entity)
        {
            return new FlatDataWrapper<IAdditionalEntity>(entity, e => e.Additional, null);
        }
    }
}