//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents an EDM value term.
    /// </summary>
    public class EdmTerm : EdmNamedElement, IEdmValueTerm
    {
        private readonly string namespaceName;
        private readonly IEdmTypeReference type;
        private readonly string appliesTo;

        /// <summary>
        /// Initializes a new instance of <see cref="EdmTerm"/> class.
        /// The new term will be of the nullable primitive <paramref name="type"/>.
        /// </summary>
        /// <param name="namespaceName">Namespace of the term.</param>
        /// <param name="name">Name of the term.</param>
        /// <param name="type">Type of the term.</param>
        public EdmTerm(string namespaceName, string name, EdmPrimitiveTypeKind type)
            : this(namespaceName, name, type, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EdmTerm"/> class.
        /// The new term will be of the nullable primitive <paramref name="type"/>.
        /// </summary>
        /// <param name="namespaceName">Namespace of the term.</param>
        /// <param name="name">Name of the term.</param>
        /// <param name="type">Type of the term.</param>
        /// <param name="appliesTo">AppliesTo of the term.</param>
        public EdmTerm(string namespaceName, string name, EdmPrimitiveTypeKind type, string appliesTo)
            : this(namespaceName, name, EdmCoreModel.Instance.GetPrimitive(type, true), appliesTo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTerm"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the term.</param>
        /// <param name="name">Name of the term.</param>
        /// <param name="type">Type of the term.</param>
        public EdmTerm(string namespaceName, string name, IEdmTypeReference type)
            : this(namespaceName, name, type, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTerm"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the term.</param>
        /// <param name="name">Name of the term.</param>
        /// <param name="type">Type of the term.</param>
        /// <param name="appliesTo">AppliesTo of the term.</param>
        public EdmTerm(string namespaceName, string name, IEdmTypeReference type, string appliesTo)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(namespaceName, "namespaceName");
            EdmUtil.CheckArgumentNull(type, "type");

            this.namespaceName = namespaceName;
            this.type = type;
            this.appliesTo = appliesTo;
        }

        /// <summary>
        /// Gets the namespace of this term.
        /// </summary>
        public string Namespace
        {
            get { return this.namespaceName; }
        }

        /// <summary>
        /// Gets the kind of this term.
        /// </summary>
        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Value; }
        }

        /// <summary>
        /// Gets the type of this term.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the AppliesTo of this term.
        /// </summary>
        public string AppliesTo
        {
            get { return this.appliesTo; }
        }

        /// <summary>
        /// Gets the schema element kind of this term.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.ValueTerm; }
        }
    }
}
