//---------------------------------------------------------------------
// <copyright file="CsdlDirectValueAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL annotation.
    /// </summary>
    internal class CsdlDirectValueAnnotation : CsdlElement
    {
        private readonly string namespaceName;
        private readonly string name;
        private readonly string value;
        private readonly bool isAttribute;

        public CsdlDirectValueAnnotation(string namespaceName, string name, string value, bool isAttribute, CsdlLocation location)
            : base(location)
        {
            this.namespaceName = namespaceName;
            this.name = name;
            this.value = value;
            this.isAttribute = isAttribute;
        }

        public string NamespaceName
        {
            get { return this.namespaceName; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Value
        {
            get { return this.value; }
        }

        public bool IsAttribute
        {
            get { return this.isAttribute; }
        }
    }
}
