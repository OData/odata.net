//---------------------------------------------------------------------
// <copyright file="UnresolvedEntityContainer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class UnresolvedEntityContainer : BadEntityContainer, IUnresolvedElement
    {
        public UnresolvedEntityContainer(string name, EdmLocation location)
            : base(name, new EdmError[] { new EdmError(location, EdmErrorCode.BadUnresolvedEntityContainer, Edm.Strings.Bad_UnresolvedEntityContainer(name)) })
        {
        }
    }
}
