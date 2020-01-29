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
        private readonly CsdlAnnotations annotations;
        private readonly CsdlSemanticsSchema context;

        public CsdlSemanticsAnnotations(CsdlSemanticsSchema context, CsdlAnnotations annotations)
        {
            this.context = context;
            this.annotations = annotations;
        }

        public CsdlSemanticsSchema Context
        {
            get { return this.context; }
        }

        public CsdlAnnotations Annotations
        {
            get { return this.annotations; }
        }
    }
}
