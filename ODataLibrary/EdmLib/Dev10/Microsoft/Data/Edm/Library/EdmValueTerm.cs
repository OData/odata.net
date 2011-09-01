//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM value term.
    /// </summary>
    public class EdmValueTerm : EdmElement, IEdmValueTerm
    {
        private readonly string namespaceName;
        private readonly string namespaceUri;
        private readonly string localName;
        private readonly IEdmTypeReference type;

        /// <summary>
        /// Initializes a new instance of the EdmValueTerm class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the term.</param>
        /// <param name="namespaceUri">Namespace URI of the term.</param>
        /// <param name="localName">Name of the term within the namespace.</param>
        public EdmValueTerm(string namespaceName, string namespaceUri, string localName)
        {
            this.namespaceName = namespaceName;
            this.namespaceUri = namespaceUri;
            this.localName = localName;
        }

        /// <summary>
        /// Initializes a new instance of the EdmValueTerm class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the term.</param>
        /// <param name="namespaceUri">Namespace URI of the term.</param>
        /// <param name="localName">Name of the term within the namespace.</param>
        /// <param name="type">Type of the term.</param>
        public EdmValueTerm(string namespaceName, string namespaceUri, string localName, IEdmTypeReference type)
        {
            this.namespaceName = namespaceName ?? string.Empty;
            this.namespaceUri = namespaceUri ?? string.Empty;
            this.localName = localName;
            this.type = type;
        }

        /// <summary>
        /// Gets the namespace of this term.
        /// </summary>
        public string Namespace
        {
            get { return this.namespaceName; }
        }

        /// <summary>
        /// Gets the namespace URI of this term.
        /// </summary>
        public string NamespaceUri
        {
            get { return this.namespaceUri; }
        }

        /// <summary>
        /// Gets the local name of this term.
        /// </summary>
        public string Name
        {
            get { return this.localName; }
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
        /// Gets the schema element kind of this term.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.ValueTerm; }
        }
    }
}
