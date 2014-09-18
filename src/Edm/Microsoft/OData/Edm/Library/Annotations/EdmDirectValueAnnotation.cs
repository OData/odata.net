//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Microsoft.OData.Edm.Annotations;

namespace Microsoft.OData.Edm.Library.Annotations
{
    /// <summary>
    /// Represents an EDM annotation with an immediate native value.
    /// </summary>
    public class EdmDirectValueAnnotation : EdmNamedElement, IEdmDirectValueAnnotation
    {
        private readonly object value;
        private readonly string namespaceUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDirectValueAnnotation"/> class.
        /// </summary>
        /// <param name="namespaceUri">Namespace URI of the annotation.</param>
        /// <param name="name">Name of the annotation within the namespace.</param>
        /// <param name="value">Value of the annotation</param>
        public EdmDirectValueAnnotation(string namespaceUri, string name, object value)
            : this(namespaceUri, name)
        {
            EdmUtil.CheckArgumentNull(value, "value");
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDirectValueAnnotation"/> class.
        /// </summary>
        /// <param name="namespaceUri">Namespace URI of the annotation.</param>
        /// <param name="name">Name of the annotation within the namespace.</param>
        internal EdmDirectValueAnnotation(string namespaceUri, string name)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(namespaceUri, "namespaceUri");
            this.namespaceUri = namespaceUri;
        }

        /// <summary>
        /// The namespace Uri of the annotation.
        /// </summary>
        public string NamespaceUri
        {
            get { return this.namespaceUri; }
        }

        /// <summary>
        /// Gets the value of this annotation.
        /// </summary>
        public object Value
        {
            get { return this.value; }
        }
    }
}
