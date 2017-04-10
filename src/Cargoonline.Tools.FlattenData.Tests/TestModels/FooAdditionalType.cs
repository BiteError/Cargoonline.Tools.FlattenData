using System;

namespace Cargoonline.Tools.FlattenData.Tests.TestModels
{
    public enum FooAdditionalType
    {
        [TypeConstraint(typeof(int?), typeof(Foo))] IntAdditional,

        [TypeConstraint(typeof(string), typeof(Foo))] StringAdditional
    }
}