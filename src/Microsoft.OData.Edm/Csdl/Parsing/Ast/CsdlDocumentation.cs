//---------------------------------------------------------------------
// <copyright file="CsdlDocumentation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL documentation.
    /// </summary>
    internal class CsdlDocumentation : CsdlElement
    {
        private readonly string summary;
        private readonly string longDescription;

        public CsdlDocumentation(string summary, string longDescription, CsdlLocation location)
            : base(location)
        {
            this.summary = summary;
            this.longDescription = longDescription;
        }

        public string Summary
        {
            get { return this.summary; }
        }

        public string LongDescription
        {
            get { return this.longDescription; }
        }
    }
}
