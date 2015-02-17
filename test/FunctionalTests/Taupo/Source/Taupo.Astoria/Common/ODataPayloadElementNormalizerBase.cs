//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementNormalizerBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Base implementation of the payload element normalizer contract
    /// </summary>
    public abstract class ODataPayloadElementNormalizerBase : ODataPayloadElementReplacingVisitor, IODataPayloadElementNormalizer
    {
        /// <summary>
        /// Normalizes the given payload root
        /// </summary>
        /// <param name="rootElement">The payload to normalize</param>
        /// <returns>The normalized payload</returns>
        public virtual ODataPayloadElement Normalize(ODataPayloadElement rootElement)
        {
            return rootElement.Accept(this);
        }

        /// <summary>
        /// Converts the complex instance into an entity instance based on the metadata
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public override ODataPayloadElement Visit(ComplexInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            if (payloadElement.Annotations.OfType<EntitySetAnnotation>().Any())
            {
                var entityInstance = new EntityInstance(payloadElement.FullTypeName, payloadElement.IsNull);
                entityInstance.Properties = this.VisitCollection(payloadElement.Properties);
                return payloadElement.ReplaceWith(entityInstance);
            }

            return base.Visit(payloadElement);
        }
       
        /// <summary>
        /// Converts the ComplexInstanceCollection into an EntitySetInstance based on if the element type of the collection is entity instance
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public override ODataPayloadElement Visit(ComplexInstanceCollection payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var replaced = (ComplexInstanceCollection)base.Visit(payloadElement);
            ExceptionUtilities.CheckObjectNotNull(replaced, "ComplexInstanceCollection Expected");
            if (replaced.All(e => e.ElementType == ODataPayloadElementType.EntityInstance) && replaced.Count > 0)
            {
                var entitySet = new EntitySetInstance(replaced.Cast<EntityInstance>().ToArray());
                return replaced.ReplaceWith(entitySet);
            }

            return replaced;
        }

        /// <summary>
        /// Converts the ComplexMultiValue into an EntitySetInstance based on if the base returns a set of EntityInstances
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public override ODataPayloadElement Visit(ComplexMultiValue payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var replaced = (ComplexMultiValue)base.Visit(payloadElement);
            ExceptionUtilities.CheckObjectNotNull(replaced, "ComplexMultiValue Expected");
            if (replaced.All(e => e.ElementType == ODataPayloadElementType.EntityInstance) && replaced.Count > 0)
            {
                var entitySet = new EntitySetInstance(replaced.Cast<EntityInstance>().ToArray());
                return replaced.ReplaceWith(entitySet);
            }

            return replaced;
        }

        /// <summary>
        /// Converts the ComplexMultiValueProperty into a NavigationPropertyInstance if the base returned an EntitySetInstance
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public override ODataPayloadElement Visit(ComplexMultiValueProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var replacedCollection = this.Recurse(payloadElement.Value);
            if (!this.ShouldReplace(payloadElement.Value, replacedCollection))
            {
                return payloadElement;
            }

            var complexmulti = replacedCollection as ComplexMultiValue;
            if (complexmulti != null)
            {
                return payloadElement.ReplaceWith(new ComplexMultiValueProperty(payloadElement.Name, complexmulti));
            }

            var entityset = replacedCollection as EntitySetInstance;
            ExceptionUtilities.CheckObjectNotNull(entityset, "Replaced collection should be either ComplexMultiValue or EntitySetInstance");
            return payloadElement.ReplaceWith(new NavigationPropertyInstance(payloadElement.Name, replacedCollection));
        }

        /// <summary>
        /// Converts the complex property into a navigation property based on if the base returned an entity instance
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public override ODataPayloadElement Visit(ComplexProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var replaced = (ComplexProperty)base.Visit(payloadElement);
            ExceptionUtilities.CheckObjectNotNull(replaced, "ComplexProperty Expected");
            if (replaced.Value.ElementType == ODataPayloadElementType.EntityInstance)
            {
                var navigation = new NavigationPropertyInstance(replaced.Name, replaced.Value);
                return replaced.ReplaceWith(navigation);
            }

            return replaced;
        }

        /// <summary>
        /// Replaces the empty collection property with a more specific type
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public override ODataPayloadElement Visit(EmptyCollectionProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var memberPropertyAnnotation = payloadElement.Annotations.OfType<MemberPropertyAnnotation>().SingleOrDefault();
            if (memberPropertyAnnotation != null)
            {
                var memberProperty = memberPropertyAnnotation.Property;
                ExceptionUtilities.CheckObjectNotNull(memberProperty, "Member property annotation was null");

                var collectionType = memberProperty.PropertyType as CollectionDataType;
                ExceptionUtilities.CheckObjectNotNull(collectionType, "Property was not a collection type");

                var primitiveType = collectionType.ElementDataType as PrimitiveDataType;
                if (primitiveType != null)
                {
                    var primitiveCollection = new PrimitiveMultiValue(payloadElement.Value.FullTypeName, payloadElement.Value.IsNull);
                    return payloadElement.ReplaceWith(new PrimitiveMultiValueProperty(payloadElement.Name, primitiveCollection));
                }

                var complexType = collectionType.ElementDataType as ComplexDataType;
                ExceptionUtilities.CheckObjectNotNull(complexType, "Collection element type was neither primitive nor complex: '{0}'", collectionType.ElementDataType);
                var complexCollection = new ComplexMultiValue(payloadElement.Value.FullTypeName, payloadElement.Value.IsNull);
                return payloadElement.ReplaceWith(new ComplexMultiValueProperty(payloadElement.Name, complexCollection));
            }

            var navigationPropertyAnnotation = payloadElement.Annotations.OfType<NavigationPropertyAnnotation>().SingleOrDefault();
            if (navigationPropertyAnnotation == null)
            {
                return payloadElement;
            }

            var entitySet = payloadElement.Value.ReplaceWith(new EntitySetInstance());
            return payloadElement.ReplaceWith(new NavigationPropertyInstance(payloadElement.Name, entitySet));
        }

        /// <summary>
        /// Replaces the empty untyped collection with a more specific type
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public override ODataPayloadElement Visit(EmptyUntypedCollection payloadElement)
        {
            // TODO: handle once service operations are implemented
            return payloadElement;
        }

        /// <summary>
        /// Replaces the null property instance with a more specific type
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public override ODataPayloadElement Visit(NullPropertyInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var memberPropertyAnnotation = payloadElement.Annotations.OfType<MemberPropertyAnnotation>().SingleOrDefault();
            if (memberPropertyAnnotation != null)
            {
                var memberProperty = memberPropertyAnnotation.Property;
                ExceptionUtilities.CheckObjectNotNull(memberProperty, "Member property annotation was null");

                var primitiveType = memberProperty.PropertyType as PrimitiveDataType;
                if (primitiveType != null)
                {
                    return payloadElement.ReplaceWith(new PrimitiveProperty(payloadElement.Name, payloadElement.FullTypeName, null));
                }

                var complexType = memberProperty.PropertyType as ComplexDataType;
                if (complexType != null)
                {
                    return payloadElement.ReplaceWith(new ComplexProperty(payloadElement.Name, new ComplexInstance(payloadElement.FullTypeName, true)));
                }

                var collectionType = memberProperty.PropertyType as CollectionDataType;
                ExceptionUtilities.CheckObjectNotNull(collectionType, "Property type was not primitive, complex, or collection");

                primitiveType = collectionType.ElementDataType as PrimitiveDataType;
                if (primitiveType != null)
                {
                    return payloadElement.ReplaceWith(new PrimitiveMultiValueProperty(payloadElement.Name, new PrimitiveMultiValue(payloadElement.FullTypeName, true)));
                }

                complexType = collectionType.ElementDataType as ComplexDataType;
                ExceptionUtilities.CheckObjectNotNull(complexType, "Collection element type was not primitive or complex");
                return payloadElement.ReplaceWith(new ComplexMultiValueProperty(payloadElement.Name, new ComplexMultiValue(payloadElement.FullTypeName, true)));
            }

            var navigationPropertyAnnotation = payloadElement.Annotations.OfType<NavigationPropertyAnnotation>().SingleOrDefault();
            if (navigationPropertyAnnotation == null)
            {
                return payloadElement;
            }

            return payloadElement.ReplaceWith(new NavigationPropertyInstance(payloadElement.Name, new ExpandedLink()));
        }
       
        /// <summary>
        /// Converts the primitive value into the correct CLR type based on the metadata
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public override ODataPayloadElement Visit(PrimitiveValue payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            if (payloadElement.ClrValue == null)
            {
                return payloadElement;
            }

            var dataTypeAnnotation = payloadElement.Annotations.OfType<DataTypeAnnotation>().SingleOrDefault();
            if (dataTypeAnnotation == null)
            {
                return payloadElement;
            }

            var primitiveType = dataTypeAnnotation.DataType as PrimitiveDataType;
            
            // Handles the case where the there is an empty string value and its a different datatype
            if (primitiveType == null)
            {
                return payloadElement;
            }

            var clrType = primitiveType.GetFacet<PrimitiveClrTypeFacet>();

            if (clrType.Value.IsAssignableFrom(payloadElement.ClrValue.GetType()))
            {
                return payloadElement;
            }

            var converted = this.ConvertPrimitiveValue(payloadElement.ClrValue, clrType.Value);
            var replacement = new PrimitiveValue(payloadElement.FullTypeName, converted);
            return payloadElement.ReplaceWith(replacement);
        }

        /// <summary>
        /// Converts the given primitive value to the expected type
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="expectedType">The expected type based on the metadata</param>
        /// <returns>The converted object</returns>
        protected virtual object ConvertPrimitiveValue(object value, Type expectedType)
        {
            return Convert.ChangeType(value, expectedType, CultureInfo.InvariantCulture);
        }
    }
}