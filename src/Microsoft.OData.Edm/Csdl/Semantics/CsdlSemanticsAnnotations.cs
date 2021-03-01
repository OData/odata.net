//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsAnnotations.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for an out-of-line CSDL Annotations.
    /// </summary>
    internal class CsdlSemanticsAnnotations
    {
        public CsdlSemanticsAnnotations(CsdlSemanticsSchema schema, CsdlAnnotations annotations)
        {
            Schema = schema;
            Annotations = annotations;
        }

        public CsdlSemanticsSchema Schema { get; }

        public CsdlAnnotations Annotations { get; }
    }
}
