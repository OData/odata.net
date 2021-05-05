//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsIncludeAnnotations.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsIncludeAnnotations : CsdlSemanticsElement, IEdmIncludeAnnotations
    {
        protected readonly CsdlSemanticsModel model;
        protected CsdlIncludeAnnotations includeAnnotations;

        public CsdlSemanticsIncludeAnnotations(CsdlSemanticsModel model, CsdlIncludeAnnotations includeAnnotations)
            : base(includeAnnotations)
        {
            this.model = model;
            this.includeAnnotations = includeAnnotations;
        }

        public string TermNamespace => includeAnnotations.TermNamespace;

        public string Qualifier => includeAnnotations.Qualifier;

        public string TargetNamespace => includeAnnotations.TargetNamespace;

        public override CsdlSemanticsModel Model => model;

        public override CsdlElement Element => includeAnnotations;
    }
}
