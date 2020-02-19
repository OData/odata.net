//---------------------------------------------------------------------
// <copyright file="CsdlKey.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL key.
    /// </summary>
    internal class CsdlKey : CsdlElement
    {
        private readonly List<CsdlPropertyReference> properties;

        public CsdlKey(IEnumerable<CsdlPropertyReference> properties, CsdlLocation location)
            : base(location)
        {
            this.properties = new List<CsdlPropertyReference>(properties);
        }

        public IEnumerable<CsdlPropertyReference> Properties
        {
            get { return this.properties; }
        }
    }
}
