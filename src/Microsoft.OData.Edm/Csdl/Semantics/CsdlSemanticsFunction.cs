//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsFunction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlFunction
    /// </summary>
    internal class CsdlSemanticsFunction : CsdlSemanticsOperation, IEdmFunction
    {
        private readonly CsdlFunction function;

        public CsdlSemanticsFunction(CsdlSemanticsSchema context, CsdlFunction function)
            : base(context, function)
        {
            this.function = function;
        }

        public bool IsComposable
        {
            get { return this.function.IsComposable; }
        }

        public override EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Function; }
        }
    }
}
