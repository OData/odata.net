//---------------------------------------------------------------------
// <copyright file="EdmDirectValueAnnotationBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents the combination of an EDM annotation with an immediate value and the element to which it is attached.
    /// </summary>
    public class EdmDirectValueAnnotationBinding : IEdmDirectValueAnnotationBinding
    {
        private readonly IEdmElement element;
        private readonly string namespaceUri;
        private readonly string name;
        private readonly object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDirectValueAnnotationBinding"/> class.
        /// </summary>
        /// <param name="element">Element to which the annotation is attached.</param>
        /// <param name="namespaceUri">Namespace URI of the annotation.</param>
        /// <param name="name">Name of the annotation within the namespace.</param>
        /// <param name="value">Value of the annotation</param>
        public EdmDirectValueAnnotationBinding(IEdmElement element, string namespaceUri, string name, object value)
        {
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(namespaceUri, "namespaceUri");
            EdmUtil.CheckArgumentNull(name, "name");

            this.element = element;
            this.namespaceUri = namespaceUri;
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDirectValueAnnotationBinding"/> class.
        /// </summary>
        /// <param name="element">Element to which the annotation is attached.</param>
        /// <param name="namespaceUri">Namespace URI of the annotation.</param>
        /// <param name="name">Name of the annotation within the namespace.</param>
        public EdmDirectValueAnnotationBinding(IEdmElement element, string namespaceUri, string name)
        {
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(namespaceUri, "namespaceUri");
            EdmUtil.CheckArgumentNull(name, "name");

            this.element = element;
            this.namespaceUri = namespaceUri;
            this.name = name;
        }

        /// <summary>
        /// Gets the element to which the annotation is attached.
        /// </summary>
        public IEdmElement Element
        {
            get { return this.element; }
        }

        /// <summary>
        /// Gets the namespace Uri of the annotation.
        /// </summary>
        public string NamespaceUri
        {
            get { return this.namespaceUri; }
        }

        /// <summary>
        /// Gets the local name of the annotation.
        /// </summary>
        public string Name
        {
            get { return this.name; }
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
