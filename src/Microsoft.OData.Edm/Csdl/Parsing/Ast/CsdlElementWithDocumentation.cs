//---------------------------------------------------------------------
// <copyright file="CsdlElementWithDocumentation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Base class for CSDL elements that have documentation.
    /// </summary>
    internal abstract class CsdlElementWithDocumentation : CsdlElement
    {
        private readonly CsdlDocumentation documentation;

        public CsdlElementWithDocumentation(CsdlDocumentation documentation, CsdlLocation location)
            : base(location)
        {
            this.documentation = documentation;
        }

        public CsdlDocumentation Documentation
        {
            get { return this.documentation; }
        }

        public override bool HasDirectValueAnnotations
        {
            get { return this.documentation != null || base.HasDirectValueAnnotations; }
        }
    }
}
