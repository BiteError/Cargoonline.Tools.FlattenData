using Cargoonline.Tools.FlattenData.Tests.TestModels;
using Xunit;

namespace Cargoonline.Tools.FlattenData.Tests.Tests
{
    public class AdditionalWrapperTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(101)]
        [InlineData(-50)]
        [InlineData(null)]
        public void TestForNullableIntValue(int? expected)
        {
            var foo = new Foo();
            foo.SetAdditional(FooAdditionalType.IntAdditional, expected);
            var actual = foo.GetAdditional<int?>(FooAdditionalType.IntAdditional);

            Assert.Equal(expected, actual);
        }
        [Theory]
        [InlineData("str")]
        [InlineData("str str str")]
        [InlineData("")]
        [InlineData(null)]
        public void TestForStringValue(string expected)
        {
            var foo = new Foo();
            foo.SetAdditional(FooAdditionalType.StringAdditional, expected);
            var actual = foo.GetAdditional<string>(FooAdditionalType.StringAdditional);

            Assert.Equal(expected, actual);
        }
    }
}
