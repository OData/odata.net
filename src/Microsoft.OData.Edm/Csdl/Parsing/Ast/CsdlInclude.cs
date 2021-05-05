//---------------------------------------------------------------------
// <copyright file="CsdlInclude.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    internal class CsdlInclude : CsdlElement
    {
        public CsdlInclude(string alias, string ns, CsdlLocation location)
            : base(location)
        {
            Alias = alias;
            Namespace = ns;
        }

        public string Alias { get; }

        public string Namespace { get; }
    }
}
