//---------------------------------------------------------------------
// <copyright file="CsdlIncludeAnnotations.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlIncludeAnnotations : CsdlElement
    {
        public CsdlIncludeAnnotations(string termNamespace, string qualifier, string targetNamespace, CsdlLocation location)
            : base(location)
        {
            TermNamespace = termNamespace;
            Qualifier = qualifier;
            TargetNamespace = targetNamespace;
        }

        /// <summary>
        /// Get the term namespace.
        /// </summary>
        public string TermNamespace { get; }

        /// <summary>
        /// Gets the qualifier.
        /// </summary>
        public string Qualifier { get; }

        /// <summary>
        /// Gets the target namespace.
        /// </summary>
        public string TargetNamespace { get; }
    }
}
