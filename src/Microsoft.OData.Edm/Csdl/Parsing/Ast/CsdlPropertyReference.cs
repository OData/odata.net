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
        private readonly string propertyName;

        public CsdlPropertyReference(string propertyName, CsdlLocation location)
            : base(location)
        {
            this.propertyName = propertyName;
        }

        public string PropertyName
        {
            get { return this.propertyName; }
        }
    }
}
