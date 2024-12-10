//---------------------------------------------------------------------
// <copyright file="CsdlPropertyReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL property reference.
    /// </summary>
    internal class CsdlPropertyReference : CsdlElement
    {
        public CsdlPropertyReference(string propertyName, string propertyAlias, CsdlLocation location)
            : base(location)
        {
            PropertyName = propertyName;
            PropertyAlias = propertyAlias;
        }

        public string PropertyName { get; }
        public string PropertyAlias { get; }
    }
}
