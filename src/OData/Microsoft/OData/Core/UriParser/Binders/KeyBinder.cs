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

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

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
        /// <returns>The bound key lookup.</returns>
        internal QueryNode BindKeyValues(EntityCollectionNode collectionNode, IEnumerable<NamedValue> namedValues)
        {
            Debug.Assert(namedValues != null, "namedValues != null");
            Debug.Assert(collectionNode != null, "CollectionNode != null");

            IEdmEntityTypeReference collectionItemType = collectionNode.EntityItemType;
            List<KeyPropertyValue> keyPropertyValues = new List<KeyPropertyValue>();

            IEdmEntityType collectionItemEntityType = collectionItemType.EntityDefinition();

            HashSet<string> keyPropertyNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (NamedValue namedValue in namedValues)
            {
                KeyPropertyValue keyPropertyValue = this.BindKeyPropertyValue(namedValue, collectionItemEntityType);
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
                return collectionNode;
            }
            else if (keyPropertyValues.Count != collectionItemEntityType.Key().Count())
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues(collectionNode.ItemType.ODataFullName()));
            }
            else
            {
                return new KeyLookupNode(collectionNode, new ReadOnlyCollection<KeyPropertyValue>(keyPropertyValues));
            }
        }

        /// <summary>
        /// Binds a key property value.
        /// </summary>
        /// <param name="namedValue">The named value to bind.</param>
        /// <param name="collectionItemEntityType">The type of a single item in a collection to apply the key value to.</param>
        /// <returns>The bound key property value node.</returns>
        private KeyPropertyValue BindKeyPropertyValue(NamedValue namedValue, IEdmEntityType collectionItemEntityType)
        {
            // These are exception checks because the data comes directly from the potentially user specified tree.
            ExceptionUtils.CheckArgumentNotNull(namedValue, "namedValue");
            ExceptionUtils.CheckArgumentNotNull(namedValue.Value, "namedValue.Value");
            Debug.Assert(collectionItemEntityType != null, "collectionItemType != null");

            IEdmProperty keyProperty = null;
            if (namedValue.Name == null)
            {
                foreach (IEdmProperty p in collectionItemEntityType.Key())
                {
                    if (keyProperty == null)
                    {
                        keyProperty = p;
                    }
                    else
                    {
                        throw new ODataException(ODataErrorStrings.MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties(collectionItemEntityType.ODataFullName()));
                    }
                }
            }
            else
            {
                keyProperty = collectionItemEntityType.Key().Where(k => string.CompareOrdinal(k.Name, namedValue.Name) == 0).SingleOrDefault();

                if (keyProperty == null)
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_PropertyNotDeclaredOrNotKeyInKeyValue(namedValue.Name, collectionItemEntityType.ODataFullName()));
                }
            }

            IEdmTypeReference keyPropertyType = keyProperty.Type;

            SingleValueNode value = (SingleValueNode)this.keyValueBindMethod(namedValue.Value);

            // TODO: Check that the value is of primitive type
            Debug.Assert(keyPropertyType.IsODataPrimitiveTypeKind(), "The key's type must be primitive.");
            value = MetadataBindingUtils.ConvertToTypeIfNeeded(value, keyPropertyType);

            Debug.Assert(keyProperty != null, "keyProperty != null");
            return new KeyPropertyValue()
            {
                KeyProperty = keyProperty,
                KeyValue = value
            };
        }
    }
}
