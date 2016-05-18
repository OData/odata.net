//---------------------------------------------------------------------
// <copyright file="UnresolvedFunction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Represents information about an EDM function that failed to resolve.
    /// </summary>
    internal class UnresolvedFunction : UnresolvedOperation, IEdmFunction
    {
        public UnresolvedFunction(string qualifiedName, string errorMessage, EdmLocation location)
            : base(qualifiedName, errorMessage, location)
        {
        }

        public bool IsComposable
        {
            get { return false; }
        }

        public new EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Function; }
        }
    }
}
