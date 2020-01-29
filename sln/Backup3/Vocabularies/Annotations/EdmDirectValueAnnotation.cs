// <copyright file="EdmDirectValueAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
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
