//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using Microsoft.OData.Edm.Annotations;

namespace Microsoft.OData.Edm.Library.Annotations
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
