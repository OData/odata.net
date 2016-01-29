//---------------------------------------------------------------------
// <copyright file="ODataCollectionMaterializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Used to materialize a collection of primitive or complex values from an <see cref="ODataCollectionMaterializer"/>.
    /// </summary>
    internal sealed class ODataCollectionMaterializer : ODataMessageReaderMaterializer
    {
        /// <summary>Current value being materialized; possibly null.</summary>
        private object currentValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataCollectionMaterializer"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="materializerContext">The materializer context.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="singleResult">The single result.</param>
        public ODataCollectionMaterializer(ODataMessageReader reader, IODataMaterializerContext materializerContext, Type expectedType, bool? singleResult)
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
            if (!expectedClientType.IsCollection())
            {
                throw new DataServiceClientException(DSClient.Strings.AtomMaterializer_TypeShouldBeCollectionError(expectedClientType.FullName()));
            }

            Type underlyingExpectedType = Nullable.GetUnderlyingType(this.ExpectedType) ?? this.ExpectedType;
            bool isClrCollection = WebUtil.IsCLRTypeCollection(underlyingExpectedType, this.MaterializerContext.Model);
            Debug.Assert(isClrCollection || (SingleResult.HasValue && !SingleResult.Value), "expected type must be collection or single result must be false");
            
            // We are here for two cases: 
            // (1) Something like Execute<ICollection<T>>, in which case the underlyingExpectedType is ICollection<T>
            // (2) Execute<T> with the bool singleValue = false, in which case underlyingExpectedType is T
            Type collectionItemType = underlyingExpectedType;
            Type collectionICollectionType = ClientTypeUtil.GetImplementationType(underlyingExpectedType, typeof(ICollection<>));
            
            if (collectionICollectionType != null)
            {
                // Case 1 : Something like Execute<ICollection<T>>, in which case the underlyingExpectedType is ICollection<T>
                collectionItemType = collectionICollectionType.GetGenericArguments()[0];
            }
            else
            {
                // Case 2 : Execute<T> with the bool singleValue = false, in which case underlyingExpectedType is T
                collectionICollectionType = typeof(ICollection<>).MakeGenericType(new Type[] { collectionItemType });
            }

            Type clrCollectionType = WebUtil.GetBackingTypeForCollectionProperty(collectionICollectionType);
            object collectionInstance = this.CollectionValueMaterializationPolicy.CreateCollectionInstance((IEdmCollectionTypeReference)expectedClientType, clrCollectionType);

            // Enumerator over our collection reader was created, then ApplyDataCollections was refactored to 
            // take an enumerable instead of a ODataCollectionValue. Enumerator is being used as a bridge
            ODataCollectionReader collectionReader = messageReader.CreateODataCollectionReader();
            NonEntityItemsEnumerable collectionEnumerable = new NonEntityItemsEnumerable(collectionReader);

            bool isElementNullable = expectedClientType.AsCollection().ElementType().IsNullable;
            this.CollectionValueMaterializationPolicy.ApplyCollectionDataValues(
                collectionEnumerable,
                null /*wireTypeName*/,
                collectionInstance,
                collectionItemType,
                ClientTypeUtil.GetAddToCollectionDelegate(collectionICollectionType),
                isElementNullable);

            this.currentValue = collectionInstance;
        }

        /// <summary>
        /// Class that wraps the collection reader to get values from the collection reader
        /// </summary>
        private class NonEntityItemsEnumerable : IEnumerable, IEnumerator
        {
            /// <summary>
            /// Collection Reader
            /// </summary>
            private readonly ODataCollectionReader collectionReader;

            /// <summary>
            /// Initializes a new instance of the <see cref="NonEntityItemsEnumerable"/> class.
            /// </summary>
            /// <param name="collectionReader">The collection reader.</param>
            internal NonEntityItemsEnumerable(ODataCollectionReader collectionReader)
            {
                this.collectionReader = collectionReader;
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <returns>The current element in the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.-or- The collection was modified after the enumerator was created.</exception>
            public object Current
            {
                get { return this.collectionReader.Item; }
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            public IEnumerator GetEnumerator()
            {
                return this;
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                // Move to the next value
                bool read = this.collectionReader.Read();
                while (read && this.collectionReader.State != ODataCollectionReaderState.Value)
                {
                    read = this.collectionReader.Read();
                }

                return read;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                throw new InvalidOperationException(DSClient.Strings.AtomMaterializer_ResetAfterEnumeratorCreationError);
            }
        }
    }
}
