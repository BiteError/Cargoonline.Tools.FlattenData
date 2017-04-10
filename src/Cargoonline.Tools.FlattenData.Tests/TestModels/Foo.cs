namespace Cargoonline.Tools.FlattenData.Tests.TestModels
{
    public interface IAdditionalEntity
    {
        string Additional { get; set; }
    }

    public class Foo: IAdditionalEntity
{
        public string Additional { get; set; }
    }
}
