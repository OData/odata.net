//---------------------------------------------------------------------
// <copyright file="EdmTypedDirectValueAnnotationBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents the combination of an EDM annotation with an immediate value and the element to which it is attached.
    /// </summary>
    /// <typeparam name="T">Type of the annotation value.</typeparam>
    public class EdmTypedDirectValueAnnotationBinding<T> : EdmNamedElement, IEdmDirectValueAnnotationBinding
    {
        private readonly IEdmElement element;
        private readonly T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypedDirectValueAnnotationBinding{T}"/> class.
        /// </summary>
        /// <param name="element">Element to which the annotation is attached.</param>
        /// <param name="value">Value of the annotation</param>
        public EdmTypedDirectValueAnnotationBinding(IEdmElement element, T value)
            : base(ExtensionMethods.TypeName<T>.LocalName)
        {
            this.element = element;
            this.value = value;
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
            get { return EdmConstants.InternalUri; }
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
