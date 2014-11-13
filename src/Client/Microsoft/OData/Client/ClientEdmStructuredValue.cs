//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.OData.Client
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.OData.Edm.Values;

    /// <summary>
    /// Implementation of <see cref="IEdmStructuredValue"/> which wraps client-side objects.
    /// </summary>
    internal sealed class ClientEdmStructuredValue : IEdmStructuredValue
    {
        /// <summary>The structured value this instance is wrapping.</summary>
        private readonly object structuredValue;

        /// <summary>The client-side metadata about this value.</summary>
        private readonly ClientTypeAnnotation typeAnnotation;

        /// <summary>The model.</summary>
        private readonly ClientEdmModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientEdmStructuredValue"/> class.
        /// </summary>
        /// <param name="structuredValue">The structured value.</param>
        /// <param name="model">The model.</param>
        /// <param name="clientTypeAnnotation">The client type annotation.</param>
        public ClientEdmStructuredValue(object structuredValue, ClientEdmModel model, ClientTypeAnnotation clientTypeAnnotation)
        {
            Debug.Assert(structuredValue != null, "entity != null");
            Debug.Assert(model != null, "model != null");
            Debug.Assert(clientTypeAnnotation != null, "clientTypeAnnotation != null");

            if (clientTypeAnnotation.EdmType.TypeKind == EdmTypeKind.Complex)
            {
                // TODO: nullable?
                this.Type = new EdmComplexTypeReference((IEdmComplexType)clientTypeAnnotation.EdmType, true);
            }
            else
            {
                Debug.Assert(clientTypeAnnotation.EdmType.TypeKind == EdmTypeKind.Entity, "Only complex and entity values supported");
                
                // TODO: nullable?
                this.Type = new EdmEntityTypeReference((IEdmEntityType)clientTypeAnnotation.EdmType, true);
            }

            this.structuredValue = structuredValue;
            this.typeAnnotation = clientTypeAnnotation;
            this.model = model;
        }

        /// <summary>
        /// Gets the type of this value.
        /// </summary>
        public IEdmTypeReference Type
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public EdmValueKind ValueKind
        {
            get { return EdmValueKind.Structured; }
        }

        /// <summary>
        /// Gets the property values of this structured value.
        /// </summary>
        public IEnumerable<IEdmPropertyValue> PropertyValues
        {
            get { return this.typeAnnotation.Properties().Select(this.BuildEdmPropertyValue); }
        }

        /// <summary>
        /// Finds the value corresponding to the provided property name.
        /// </summary>
        /// <param name="propertyName">Property to find the value of.</param>
        /// <returns>
        /// The found property, or null if no property was found.
        /// </returns>
        public IEdmPropertyValue FindPropertyValue(string propertyName)
        {
            var propertyAnnotation = this.typeAnnotation.GetProperty(propertyName, true);
            if (propertyAnnotation == null)
            {
                return null;
            }

            return this.BuildEdmPropertyValue(propertyAnnotation);
        }

        /// <summary>
        /// Builds an edm property value from the given annotation.
        /// </summary>
        /// <param name="propertyAnnotation">The property annotation.</param>
        /// <returns>The property value</returns>
        private IEdmPropertyValue BuildEdmPropertyValue(ClientPropertyAnnotation propertyAnnotation)
        {
            var propertyValue = propertyAnnotation.GetValue(this.structuredValue);
            var edmValue = this.ConvertToEdmValue(propertyValue, propertyAnnotation.EdmProperty.Type);
            return new EdmPropertyValue(propertyAnnotation.EdmProperty.Name, edmValue);
        }

        /// <summary>
        /// Converts a clr value to an edm value.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="edmPropertyType">Type of the property.</param>
        /// <returns>
        /// The converted value
        /// </returns>
        private IEdmValue ConvertToEdmValue(object propertyValue, IEdmTypeReference edmPropertyType)
        {
            Debug.Assert(edmPropertyType != null, "edmPropertyType != null");

            if (propertyValue == null)
            {
                return EdmNullExpression.Instance;
            }

            if (edmPropertyType.IsStructured())
            {
                return new ClientEdmStructuredValue(propertyValue, this.model, this.model.GetClientTypeAnnotation(edmPropertyType.Definition));
            }

            if (edmPropertyType.IsCollection())
            {
                var collectionType = edmPropertyType as IEdmCollectionTypeReference;
                Debug.Assert(collectionType != null, "collectionType != null");
                var elements = ((IEnumerable)propertyValue).Cast<object>().Select(v => this.ConvertToEdmValue(v, collectionType.ElementType()));
                return new ClientEdmCollectionValue(collectionType, elements);
            }

            var primitiveType = edmPropertyType as IEdmPrimitiveTypeReference;
            Debug.Assert(primitiveType != null, "Type was not structured, collection, or primitive");

            return EdmValueUtils.ConvertPrimitiveValue(propertyValue, primitiveType).Value;
        }
    }
}
