//---------------------------------------------------------------------
// <copyright file="AmbiguousOperationBindingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class AmbiguousOperationBindingTests
    {
        [Fact]
        public void AmbigiousOperationBindingShouldReferToFirstOperationAlwaysWhenNotNull()
        {
            var action1 = new EdmAction("DS", "name", EdmCoreModel.Instance.GetBoolean(false));
            action1.AddParameter("param", EdmCoreModel.Instance.GetBoolean(false));
            var function = new EdmFunction("DS2", "name2", EdmCoreModel.Instance.GetBoolean(false), true, new EdmPathExpression("path1"), true);
            AmbiguousOperationBinding ambigiousOperationBinding = new AmbiguousOperationBinding(action1, function);

            Assert.Equal("DS", ambigiousOperationBinding.Namespace);
            Assert.Equal("name", ambigiousOperationBinding.Name);
            Assert.Null(ambigiousOperationBinding.Return?.Type);
            Assert.Single(ambigiousOperationBinding.Parameters);
            Assert.Equal(EdmSchemaElementKind.Action, ambigiousOperationBinding.SchemaElementKind);
            Assert.False(ambigiousOperationBinding.IsBound);
            Assert.Null(ambigiousOperationBinding.EntitySetPath);
        }

        [Fact]
        public void AmbiguousOperationBindingShouldForwardFindParameterToFirst()
        {
            var action = new EdmAction("NS", "TestAction", EdmCoreModel.Instance.GetBoolean(false));
            action.AddParameter("p1", EdmCoreModel.Instance.GetBoolean(false));
            action.AddParameter("p2", EdmCoreModel.Instance.GetBoolean(false));
            var function = new EdmFunction("NS2", "TestFunction", EdmCoreModel.Instance.GetBoolean(false));
            AmbiguousOperationBinding ambiguous = new AmbiguousOperationBinding(action, function);

            Assert.NotNull(ambiguous.FindParameter("p1"));
            Assert.NotNull(ambiguous.FindParameter("p2"));
            Assert.Null(ambiguous.FindParameter("notfound"));
        }

        [Fact]
        public void AmbiguousOperationBindingShouldReturnNullForReturnProperty()
        {
            var function1 = new EdmFunction("NS", "Func", EdmCoreModel.Instance.GetBoolean(false));
            var function2 = new EdmFunction("NS", "Func", EdmCoreModel.Instance.GetBoolean(false));
            AmbiguousOperationBinding ambiguous = new AmbiguousOperationBinding(function1, function2);

            Assert.Null(ambiguous.Return);
        }

        [Fact]
        public void AmbiguousOperationBindingShouldUseFirstOperationProperties()
        {
            var action1 = new EdmAction("NS", "Action1", EdmCoreModel.Instance.GetBoolean(false), true, new EdmPathExpression("entitySetPath"));
            var action2 = new EdmAction("NS2", "Action2", EdmCoreModel.Instance.GetBoolean(false), false, null);
            AmbiguousOperationBinding ambiguous = new AmbiguousOperationBinding(action1, action2);

            Assert.Equal("NS", ambiguous.Namespace);
            Assert.Equal("Action1", ambiguous.Name);
            Assert.Equal(action1.FullName, ambiguous.FullName);
            Assert.Equal(action1.Parameters, ambiguous.Parameters);
            Assert.Equal(action1.IsBound, ambiguous.IsBound);
            Assert.Equal(action1.EntitySetPath, ambiguous.EntitySetPath);
            Assert.Equal(EdmSchemaElementKind.Action, ambiguous.SchemaElementKind);
        }

        [Fact]
        public void AmbiguousOperationBindingShouldWorkWithFunctionAsFirst()
        {
            var function1 = new EdmFunction("NS", "Func1", EdmCoreModel.Instance.GetBoolean(false), true, new EdmPathExpression("entitySetPath"), true);
            var action2 = new EdmAction("NS2", "Action2", EdmCoreModel.Instance.GetBoolean(false), false, null);
            AmbiguousOperationBinding ambiguous = new AmbiguousOperationBinding(function1, action2);

            Assert.Equal("NS", ambiguous.Namespace);
            Assert.Equal("Func1", ambiguous.Name);
            Assert.Equal(function1.FullName, ambiguous.FullName);
            Assert.Equal(function1.Parameters, ambiguous.Parameters);
            Assert.Equal(function1.IsBound, ambiguous.IsBound);
            Assert.Equal(function1.EntitySetPath, ambiguous.EntitySetPath);
            Assert.Equal(EdmSchemaElementKind.Function, ambiguous.SchemaElementKind);
        }
    }
}
