//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsAction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlAction
    /// </summary>
    internal class CsdlSemanticsAction : CsdlSemanticsOperation, IEdmAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsdlSemanticsAction"/> class.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="action">The action.</param>
        public CsdlSemanticsAction(CsdlSemanticsSchema schema, CsdlAction action)
            : base(schema, action)
        {
        }

        public override EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.Action;
    }
}
