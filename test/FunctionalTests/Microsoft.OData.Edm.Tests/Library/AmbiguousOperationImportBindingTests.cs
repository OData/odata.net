//---------------------------------------------------------------------
// <copyright file="AmbiguousOperationImportBindingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class AmbiguousOperationImportBindingTests
    {
        [Fact]
        public void AmbigiousOperationBindingShouldReferToFirstOperationAlwaysWhenNotNull()
        {
            var container1 =new EdmEntityContainer("n", "d");
            var container2 =new EdmEntityContainer("n", "d");
            var action1Import = new EdmActionImport(container1, "name", new EdmAction("n", "name", null));
            var functionImport = new EdmFunctionImport(container2, "name", new EdmFunction("n", "name", EdmCoreModel.Instance.GetString(true)));
            var ambigiousOperationBinding = new AmbiguousOperationImportBinding(action1Import, functionImport);
            ambigiousOperationBinding.ContainerElementKind.Should().Be(EdmContainerElementKind.ActionImport);
            ambigiousOperationBinding.Name.Should().Be("name");
            ambigiousOperationBinding.EntitySet.Should().BeNull();
            ambigiousOperationBinding.Container.Should().Be(container1);
        }
    }
}
