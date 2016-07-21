//---------------------------------------------------------------------
// <copyright file="KeyBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Class that knows how to bind key values.
    /// </summary>
    internal sealed class KeyBinder
    {
        /// <summary>
        /// Method to bind the value of a key.
        /// TODO: Make it of return type SingleValueQueryNode.
        /// </summary>
        private readonly MetadataBinder.QueryTokenVisitor keyValueBindMethod;

        /// <summary>
        /// Constructs a KeyBinder object using the given function to bind key values.
        /// </summary>
        /// <param name="keyValueBindMethod">Method to call to bind a value in a key.</param>
        internal KeyBinder(MetadataBinder.QueryTokenVisitor keyValueBindMethod)
        {
            this.keyValueBindMethod = keyValueBindMethod;
        }

        /// <summary>
        /// Binds key values to a key lookup on a collection.
        /// </summary>
        /// <param name="collectionNode">Already bound collection node.</param>
        /// <param name="namedValues">The named value tokens to bind.</param>
        /// <param name="model">The model to be used.</param>
        /// <returns>The bound key lookup.</returns>
        internal QueryNode BindKeyValues(CollectionResourceNode collectionNode, IEnumerable<NamedValue> namedValues, IEdmModel model)
        {
            Debug.Assert(namedValues != null, "namedValues != null");
            Debug.Assert(collectionNode != null, "CollectionNode != null");
            Debug.Assert(model != null, "model != null");

            IEdmEntityTypeReference collectionItemType = collectionNode.ItemStructuredType as IEdmEntityTypeReference;

            IEdmEntityType collectionItemEntityType = collectionItemType.EntityDefinition();
            QueryNode keyLookupNode;

            if (TryBindToDeclaredKey(collectionNode, namedValues, model, collectionItemEntityType, out keyLookupNode))
            {
                return keyLookupNode;
            }
            else if (TryBindToDeclaredAlternateKey(collectionNode, namedValues, model, collectionItemEntityType, out keyLookupNode))
            {
                return keyLookupNode;
            }
            else
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues(collectionNode.ItemStructuredType.FullName()));
            }
        }

        /// <summary>
        /// Tries to bind key values to a key lookup on a collection.
        /// </summary>
        /// <param name="collectionNode">Already bound collection node.</param>
        /// <param name="namedValues">The named value tokens to bind.</param>
        /// <param name="model">The model to be used.</param>
        /// <param name="collectionItemEntityType">The type of a single item in a collection to apply the key value to.</param>
        /// <param name="keyLookupNode">The bound key lookup.</param>
        /// <returns>Returns true if binding succeeded.</returns>
        private bool TryBindToDeclaredAlternateKey(CollectionResourceNode collectionNode, IEnumerable<NamedValue> namedValues, IEdmModel model, IEdmEntityType collectionItemEntityType, out QueryNode keyLookupNode)
        {
            IEnumerable<IDictionary<string, IEdmProperty>> alternateKeys = model.GetAlternateKeysAnnotation(collectionItemEntityType);
            foreach (IDictionary<string, IEdmProperty> keys in alternateKeys)
            {
                if (TryBindToKeys(collectionNode, namedValues, model, collectionItemEntityType, keys, out keyLookupNode))
                {
                    return true;
                }
            }

            keyLookupNode = null;
            return false;
        }

        /// <summary>
        /// Tries to bind key values to a key lookup on a collection.
        /// </summary>
        /// <param name="collectionNode">Already bound collection node.</param>
        /// <param name="namedValues">The named value tokens to bind.</param>
        /// <param name="model">The model to be used.</param>
        /// <param name="collectionItemEntityType">The type of a single item in a collection to apply the key value to.</param>
        /// <param name="keyLookupNode">The bound key lookup.</param>
        /// <returns>Returns true if binding succeeded.</returns>
        private bool TryBindToDeclaredKey(CollectionResourceNode collectionNode, IEnumerable<NamedValue> namedValues, IEdmModel model, IEdmEntityType collectionItemEntityType, out QueryNode keyLookupNode)
        {
            Dictionary<string, IEdmProperty> keys = new Dictionary<string, IEdmProperty>(StringComparer.Ordinal);
            foreach (IEdmStructuralProperty property in collectionItemEntityType.Key())
            {
                keys[property.Name] = property;
            }

            return TryBindToKeys(collectionNode, namedValues, model, collectionItemEntityType, keys, out keyLookupNode);
        }

        /// <summary>
        /// Binds key values to a key lookup on a collection.
        /// </summary>
        /// <param name="collectionNode">Already bound collection node.</param>
        /// <param name="namedValues">The named value tokens to bind.</param>
        /// <param name="model">The model to be used.</param>
        /// <param name="collectionItemEntityType">The type of a single item in a collection to apply the key value to.</param>
        /// <param name="keys">Dictionary of aliases to structural property names for the key.</param>
        /// <param name="keyLookupNode">The bound key lookup.</param>
        /// <returns>Returns true if binding succeeded.</returns>
        private bool TryBindToKeys(CollectionResourceNode collectionNode, IEnumerable<NamedValue> namedValues, IEdmModel model, IEdmEntityType collectionItemEntityType, IDictionary<string, IEdmProperty> keys, out QueryNode keyLookupNode)
        {
            List<KeyPropertyValue> keyPropertyValues = new List<KeyPropertyValue>();
            HashSet<string> keyPropertyNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (NamedValue namedValue in namedValues)
            {
                KeyPropertyValue keyPropertyValue;

                if (!this.TryBindKeyPropertyValue(namedValue, collectionItemEntityType, keys, out keyPropertyValue))
                {
                    keyLookupNode = null;
                    return false;
                }

                Debug.Assert(keyPropertyValue != null, "keyPropertyValue != null");
                Debug.Assert(keyPropertyValue.KeyProperty != null, "keyPropertyValue.KeyProperty != null");

                if (!keyPropertyNames.Add(keyPropertyValue.KeyProperty.Name))
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_DuplicitKeyPropertyInKeyValues(keyPropertyValue.KeyProperty.Name));
                }

                keyPropertyValues.Add(keyPropertyValue);
            }

            if (keyPropertyValues.Count == 0)
            {
                // No key values specified, for example '/Customers()', do not include the key lookup at all
                keyLookupNode = collectionNode;
                return true;
            }
            else if (keyPropertyValues.Count != collectionItemEntityType.Key().Count())
            {
                keyLookupNode = null;
                return false;
            }
            else
            {
                keyLookupNode = new KeyLookupNode(collectionNode, new ReadOnlyCollection<KeyPropertyValue>(keyPropertyValues));
                return true;
            }
        }

        /// <summary>
        /// Binds a key property value.
        /// </summary>
        /// <param name="namedValue">The named value to bind.</param>
        /// <param name="collectionItemEntityType">The type of a single item in a collection to apply the key value to.</param>
        /// <param name="keys">Dictionary of alias to keys.</param>
        /// <param name="keyPropertyValue">The bound key property value node.</param>
        /// <returns>The bound key property value node.</returns>
        private bool TryBindKeyPropertyValue(NamedValue namedValue, IEdmEntityType collectionItemEntityType, IDictionary<string, IEdmProperty> keys, out KeyPropertyValue keyPropertyValue)
        {
            // These are exception checks because the data comes directly from the potentially user specified tree.
            ExceptionUtils.CheckArgumentNotNull(namedValue, "namedValue");
            ExceptionUtils.CheckArgumentNotNull(namedValue.Value, "namedValue.Value");
            Debug.Assert(collectionItemEntityType != null, "collectionItemType != null");

            IEdmProperty keyProperty = null;
            if (namedValue.Name == null)
            {
                foreach (IEdmProperty p in keys.Values)
                {
                    if (keyProperty == null)
                    {
                        keyProperty = p;
                    }
                    else
                    {
                        throw new ODataException(ODataErrorStrings.MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties(collectionItemEntityType.FullTypeName()));
                    }
                }
            }
            else
            {
                keyProperty = keys.SingleOrDefault(k => string.CompareOrdinal(k.Key, namedValue.Name) == 0).Value;

                if (keyProperty == null)
                {
                    keyPropertyValue = null;
                    return false;
                }
            }

            IEdmTypeReference keyPropertyType = keyProperty.Type;

            SingleValueNode value = (SingleValueNode)this.keyValueBindMethod(namedValue.Value);

            // TODO: Check that the value is of primitive type
            Debug.Assert(keyPropertyType.IsODataPrimitiveTypeKind(), "The key's type must be primitive.");
            value = MetadataBindingUtils.ConvertToTypeIfNeeded(value, keyPropertyType);

            Debug.Assert(keyProperty != null, "keyProperty != null");
            keyPropertyValue = new KeyPropertyValue()
            {
                KeyProperty = keyProperty,
                KeyValue = value
            };

            return true;
        }
    }
}