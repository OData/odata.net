//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsDocumentation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlDocumentation.
    /// </summary>
    internal class CsdlSemanticsDocumentation : CsdlSemanticsElement, IEdmDocumentation, IEdmDirectValueAnnotation
    {
        private readonly CsdlDocumentation documentation;
        private readonly CsdlSemanticsModel model;

        public CsdlSemanticsDocumentation(CsdlDocumentation documentation, CsdlSemanticsModel model)
            : base(documentation)
        {
            this.documentation = documentation;
            this.model = model;
        }

        public string Summary
        {
            get { return this.documentation.Summary; }
        }

        public string Description
        {
            get { return this.documentation.LongDescription; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.model; }
        }

        public override CsdlElement Element
        {
            get { return this.documentation; }
        }

        public string NamespaceUri
        {
            get { return EdmConstants.DocumentationUri; }
        }

        public string Name
        {
            get { return EdmConstants.DocumentationAnnotation; }
        }

        object IEdmDirectValueAnnotation.Value
        {
            get { return this; }
        }
    }
}
