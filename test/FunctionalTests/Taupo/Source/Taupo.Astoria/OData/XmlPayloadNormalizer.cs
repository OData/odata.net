//---------------------------------------------------------------------
// <copyright file="XmlPayloadNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Payload element normalizer for the atom/xml format
    /// </summary>
    public class XmlPayloadNormalizer : ODataPayloadElementNormalizerBase
    {
        private bool isRoot;

        /// <summary>
        /// Normalizes the given payload root
        /// </summary>
        /// <param name="rootElement">The payload to normalize</param>
        /// <returns>The normalized payload</returns>
        public override ODataPayloadElement Normalize(ODataPayloadElement rootElement)
        {
            this.isRoot = true;
            return base.Normalize(rootElement);
        }

        /// <summary>
        /// Normalizes complex multi-value properties, potentially replacing them with complex collections if the metadata indicates the payload is from a service operation
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public override ODataPayloadElement Visit(ComplexMultiValueProperty payloadElement)
        {
            var replaced = base.Visit(payloadElement);
            if (replaced.ElementType == ODataPayloadElementType.ComplexMultiValueProperty)
            {
                payloadElement = (ComplexMultiValueProperty)replaced;
                if (this.ShouldReplaceWithCollection(payloadElement, payloadElement.Value.IsNull, payloadElement.Value.FullTypeName))
                {
                    return payloadElement
                        .ReplaceWith(new ComplexInstanceCollection(payloadElement.Value.ToArray()))
                        .WithAnnotations(new CollectionNameAnnotation() { Name = payloadElement.Name });
                }
            }

            return replaced;
        }

        /// <summary>
        /// Normalizes primitive multi-value properties, potentially replacing them with primitive collections if the metadata indicates the payload is from a service operation
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public override ODataPayloadElement Visit(PrimitiveMultiValueProperty payloadElement)
        {
            var replaced = base.Visit(payloadElement);
            if (replaced.ElementType == ODataPayloadElementType.PrimitiveMultiValueProperty)
            {
                payloadElement = (PrimitiveMultiValueProperty)replaced;
                if (this.ShouldReplaceWithCollection(payloadElement, payloadElement.Value.IsNull, payloadElement.Value.FullTypeName))
                {
                    return payloadElement
                        .ReplaceWith(new PrimitiveCollection(payloadElement.Value.ToArray()))
                        .WithAnnotations(new CollectionNameAnnotation() { Name = payloadElement.Name });
                }
            }

            return replaced;
        }

        /// <summary>
        /// Normalizes complex properties, potentially replacing them with collections if the metadata indicates the payload is from a service operation
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public override ODataPayloadElement Visit(ComplexProperty payloadElement)
        {
            var replaced = base.Visit(payloadElement);
            if (replaced.ElementType == ODataPayloadElementType.ComplexProperty)
            {
                payloadElement = (ComplexProperty)replaced;

                // if the payload looks like
                //   <Foo>
                //     <element m:type="Edm.Int32">3</element>
                //   </Foo>
                // or
                //   <Foo>
                //     <element m:type="Complex">
                //       <Bar>3</Bar>
                //     </element>
                //   </Foo>
                // then it may be deserialized as a complex instance with exactly 1 property, when it should be a collection of size 1
                //
                if (this.ShouldReplaceWithCollection(payloadElement, payloadElement.Value.IsNull, payloadElement.Value.FullTypeName))
                {
                    // only replace if there is exactly 1 property
                    if (payloadElement.Value.Properties.Count() == 1)
                    {
                        // get the single property and check to see if its name is 'element'
                        var property = payloadElement.Value.Properties.Single();
                        if (property.Name == ODataConstants.CollectionItemElementName)
                        {
                            // determine whether it is a primitive or complex value based on the kind of property
                            ODataPayloadElementCollection collection = null;
                            if (property.ElementType == ODataPayloadElementType.PrimitiveProperty)
                            {
                                var primitiveProperty = (PrimitiveProperty)property;
                                collection = new PrimitiveCollection(primitiveProperty.Value);
                            }
                            else if (property.ElementType == ODataPayloadElementType.ComplexProperty)
                            {
                                var complexProperty = (ComplexProperty)property;
                                collection = new ComplexInstanceCollection(complexProperty.Value);
                            }

                            // if it was primitive or complex, replace it
                            if (collection != null)
                            {
                                return payloadElement
                                    .ReplaceWith(collection)
                                    .WithAnnotations(new CollectionNameAnnotation() { Name = payloadElement.Name });
                            }
                        }
                    }
                }
            }

            return replaced;
        }

        /// <summary>
        /// Normalizes primitive properties, potentially replacing them with collections if the metadata indicates the payload is from a service operation
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public override ODataPayloadElement Visit(PrimitiveProperty payloadElement)
        {
            var replaced = base.Visit(payloadElement);
            if (replaced.ElementType == ODataPayloadElementType.PrimitiveProperty)
            {
                payloadElement = (PrimitiveProperty)replaced;

                // if the payload looks like
                // <Foo />
                // then it will be deserialized as a primitive property, when it could be an empty collection
                //
                if (this.ShouldReplaceWithCollection(payloadElement, payloadElement.Value.IsNull, payloadElement.Value.FullTypeName))
                {
                    // if the value is an empty string
                    var stringValue = payloadElement.Value.ClrValue as string;
                    if (stringValue != null && stringValue.Length == 0)
                    {
                        // get the element data type. Note that this must succeed based on the checks performed earlier
                        var dataType = ((CollectionDataType)payloadElement.Annotations.OfType<DataTypeAnnotation>().Single().DataType).ElementDataType;

                        // determine whether to return a complex or primitive collection based on the data type
                        ODataPayloadElementCollection collection;
                        if (dataType is PrimitiveDataType)
                        {
                            collection = new PrimitiveCollection();
                        }
                        else
                        {
                            ExceptionUtilities.Assert(dataType is ComplexDataType, "Data type was neither primitive nor complex");
                            collection = new ComplexInstanceCollection();
                        }

                        // return the replacement
                        return payloadElement
                            .ReplaceWith(collection)
                            .WithAnnotations(new CollectionNameAnnotation() { Name = payloadElement.Name });
                    }
                }
            }

            return replaced;
        }

        /// <summary>
        /// Wrapper for recursively visiting the given element.
        /// </summary>
        /// <param name="element">The element to visit</param>
        /// <returns>The result of visiting the element</returns>
        protected override ODataPayloadElement Recurse(ODataPayloadElement element)
        {
            var oldValue = this.isRoot;
            try
            {
                this.isRoot = false;
                return base.Recurse(element);
            }
            finally
            {
                this.isRoot = oldValue;
            }
        }

        private bool ShouldReplaceWithCollection(PropertyInstance property, bool isNull, string collectionTypeName)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");

            // only replace root elements with no null-marker or type name
            bool shouldReplace = this.isRoot && !isNull && collectionTypeName == null;

            // only replace if it is from a service operation which returns a collection of complex or primitive types
            if (shouldReplace)
            {
                var functionAnnotation = property.Annotations.OfType<FunctionAnnotation>().SingleOrDefault();
                if (functionAnnotation != null)
                {
                    var dataTypeAnnotation = property.Annotations.OfType<DataTypeAnnotation>().SingleOrDefault();
                    if (dataTypeAnnotation != null)
                    {
                        ExceptionUtilities.CheckObjectNotNull(dataTypeAnnotation.DataType, "DataType Annotation doesn't contain a DataType");

                        var collectionType = dataTypeAnnotation.DataType as CollectionDataType;
                        if (collectionType != null)
                        {
                            return collectionType.ElementDataType is ComplexDataType || collectionType.ElementDataType is PrimitiveDataType;
                        }
                    }
                }
            }
            
            return false;
        }
    }
}
