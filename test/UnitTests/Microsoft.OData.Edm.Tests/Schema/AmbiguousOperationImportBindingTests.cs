//---------------------------------------------------------------------
// <copyright file="AmbiguousOperationImportBindingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
            Assert.Equal(EdmContainerElementKind.ActionImport, ambigiousOperationBinding.ContainerElementKind);
            Assert.Equal("name", ambigiousOperationBinding.Name);
            Assert.Null(ambigiousOperationBinding.EntitySet);
            Assert.Same(container1, ambigiousOperationBinding.Container);
        }
    }
}
