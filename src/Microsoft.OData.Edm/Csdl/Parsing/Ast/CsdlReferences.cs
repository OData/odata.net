//---------------------------------------------------------------------
// <copyright file="CsdlReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlReference : CsdlElement
    {
        private readonly List<CsdlInclude> includes;
        private readonly List<CsdlIncludeAnnotations> includeAnnotations;

        public CsdlReference(string uri,
            IEnumerable<CsdlInclude> includes,
            IEnumerable<CsdlIncludeAnnotations> includeAnnotations,
            CsdlLocation location)
            : base(location)
        {
            Uri = uri;
            this.includes = new List<CsdlInclude>(includes);
            this.includeAnnotations = new List<CsdlIncludeAnnotations>(includeAnnotations);
        }

        public string Uri { get; }

        public IEnumerable<CsdlInclude> Includes => includes;

        public IEnumerable<CsdlIncludeAnnotations> IncludeAnnotations => includeAnnotations;
    }
}
