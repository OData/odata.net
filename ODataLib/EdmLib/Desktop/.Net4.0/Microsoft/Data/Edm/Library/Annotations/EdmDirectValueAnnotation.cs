//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.Data.Edm.Annotations;

namespace Microsoft.Data.Edm.Library.Annotations
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
