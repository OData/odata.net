//---------------------------------------------------------------------
// <copyright file="CsdlNavigationPropertyBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL Navigation Property Binding.
    /// </summary>
    internal class CsdlNavigationPropertyBinding : CsdlElementWithDocumentation
    {
        private readonly string path;
        private readonly string target;

        public CsdlNavigationPropertyBinding(string path, string target, CsdlDocumentation documentation, CsdlLocation location)
            : base(documentation, location)
        {
            this.path = path;
            this.target = target;
        }

        public string Path
        {
            get { return this.path; }
        }

        public string Target
        {
            get { return this.target; }
        }
    }
}
