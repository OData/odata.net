//---------------------------------------------------------------------
// <copyright file="ODataPropertyMaterializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Used to materialize a property from an <see cref="ODataMessageReader"/>.
    /// </summary>
    internal sealed class ODataPropertyMaterializer : ODataMessageReaderMaterializer
    {
        /// <summary>Current value being materialized; possibly null.</summary>
        private object currentValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataPropertyMaterializer"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="materializerContext">The materializer context.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="singleResult">The single result.</param>
        public ODataPropertyMaterializer(ODataMessageReader reader, IODataMaterializerContext materializerContext, Type expectedType, bool? singleResult)
            : base(reader, materializerContext, expectedType, singleResult)
        {
        }

        /// <summary>
        /// Current value being materialized; possibly null.
        /// </summary>
        internal override object CurrentValue
        {
            get { return this.currentValue; }
        }

        /// <summary>
        /// Reads a value from the message reader.
        /// </summary>
        /// <param name="expectedClientType">The expected client type being materialized into.</param>
        /// <param name="expectedReaderType">The expected type for the underlying reader.</param>
        protected override void ReadWithExpectedType(IEdmTypeReference expectedClientType, IEdmTypeReference expectedReaderType)
        {
            ODataProperty property = this.messageReader.ReadProperty(expectedReaderType);
            Type underlyingExpectedType = Nullable.GetUnderlyingType(this.ExpectedType) ?? this.ExpectedType;

            object propertyValue = property.Value;
            if (expectedClientType.IsCollection())
            {
                Debug.Assert(WebUtil.IsCLRTypeCollection(underlyingExpectedType, this.MaterializerContext.Model) || (SingleResult.HasValue && !SingleResult.Value), "expected type must be collection or single result must be false");

                // We are here for two cases: 
                // (1) Something like Execute<ICollection<T>>, in which case the underlyingExpectedType is ICollection<T>
                // (2) Execute<T> with the bool singleValue = false, in which case underlyingExpectedType is T
                Type collectionItemType = this.ExpectedType;
                Type collectionICollectionType = ClientTypeUtil.GetImplementationType(underlyingExpectedType, typeof(ICollection<>));
                object collectionInstance;

                if (collectionICollectionType != null)
                {
                    // Case 1
                    collectionItemType = collectionICollectionType.GetGenericArguments()[0];
                    collectionInstance = this.CollectionValueMaterializationPolicy.CreateCollectionPropertyInstance(property, underlyingExpectedType);
                }
                else
                {
                    // Case 2
                    collectionICollectionType = typeof(ICollection<>).MakeGenericType(new Type[] { collectionItemType });
                    collectionInstance = this.CollectionValueMaterializationPolicy.CreateCollectionPropertyInstance(property, collectionICollectionType);
                }

                bool isElementNullable = expectedClientType.AsCollection().ElementType().IsNullable;
                this.CollectionValueMaterializationPolicy.ApplyCollectionDataValues(
                    property,
                    collectionInstance,
                    collectionItemType,
                    ClientTypeUtil.GetAddToCollectionDelegate(collectionICollectionType),
                    isElementNullable);

                this.currentValue = collectionInstance;
            }
            else if (expectedClientType.IsComplex())
            {
                ODataComplexValue complexValue = propertyValue as ODataComplexValue;
                Debug.Assert(this.MaterializerContext.Model.GetOrCreateEdmType(underlyingExpectedType).ToEdmTypeReference(false).IsComplex(), "expectedType must be complex type");

                this.ComplexValueMaterializationPolicy.MaterializeComplexTypeProperty(underlyingExpectedType, complexValue);
                this.currentValue = complexValue.GetMaterializedValue();
            }
            else if (expectedClientType.IsEnum())
            {
                this.currentValue = this.EnumValueMaterializationPolicy.MaterializeEnumTypeProperty(underlyingExpectedType, property);
            }
            else
            {
                Debug.Assert(this.MaterializerContext.Model.GetOrCreateEdmType(underlyingExpectedType).ToEdmTypeReference(false).IsPrimitive(), "expectedType must be primitive type");
                this.currentValue = this.PrimitivePropertyConverter.ConvertPrimitiveValue(property.Value, this.ExpectedType);
            }
        }
    }
}
