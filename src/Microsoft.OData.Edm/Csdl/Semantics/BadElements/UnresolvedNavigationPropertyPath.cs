//---------------------------------------------------------------------
// <copyright file="UnresolvedNavigationPropertyPath.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    using Microsoft.OData.Edm.Validation;

    /// <summary>
    /// Represents a navigation property path that could not be resolved.
    /// </summary>
    internal class UnresolvedNavigationPropertyPath : BadNavigationProperty, IUnresolvedElement
    {
        public UnresolvedNavigationPropertyPath(IEdmStructuredType startingType, string path, EdmLocation location)
            : base(startingType, path, new[] { new EdmError(location, EdmErrorCode.BadUnresolvedNavigationPropertyPath, Edm.Strings.Bad_UnresolvedNavigationPropertyPath(path, startingType.FullTypeName())) })
        {
        }
    }
}