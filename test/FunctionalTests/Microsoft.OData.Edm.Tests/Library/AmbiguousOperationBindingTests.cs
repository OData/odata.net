//---------------------------------------------------------------------
// <copyright file="AmbiguousOperationBindingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Expressions;
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
            ambigiousOperationBinding.Namespace.Should().Be("DS");
            ambigiousOperationBinding.Name.Should().Be("name");
            ambigiousOperationBinding.ReturnType.Should().BeNull();
            ambigiousOperationBinding.Parameters.Should().HaveCount(1);
            ambigiousOperationBinding.SchemaElementKind.Should().Be(EdmSchemaElementKind.Action);
            ambigiousOperationBinding.IsBound.Should().BeFalse();
            ambigiousOperationBinding.EntitySetPath.Should().BeNull();
        }
    }
}
