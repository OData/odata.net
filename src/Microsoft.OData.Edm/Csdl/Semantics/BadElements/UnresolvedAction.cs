//---------------------------------------------------------------------
// <copyright file="UnresolvedAction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Represents information about an EDM action that failed to resolve.
    /// </summary>
    internal class UnresolvedAction : UnresolvedOperation, IEdmAction
    {
        public UnresolvedAction(string qualifiedName, string errorMessage, EdmLocation location)
            : base(qualifiedName, errorMessage, location)
        {
        }

        public new EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Action; }
        }
    }
}
