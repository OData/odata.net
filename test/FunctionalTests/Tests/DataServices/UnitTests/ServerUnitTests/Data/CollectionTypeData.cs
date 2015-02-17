//---------------------------------------------------------------------
// <copyright file="CollectionTypeData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Data
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    #endregion Namespaces

    /// <summary>Provides information about interesting collection types.</summary>
    /// <remarks>
    /// Bit collections and dictionary-style collections aren't covered by
    /// this type.
    /// </remarks>
    public sealed class CollectionTypeData
    {
        #region Private fields.

        /// <summary>Interesting collection types.</summary>
        public static CollectionTypeData[] values;

        /// <summary>Collection type (possibly generic definition).</summary>
        private readonly Type type;

        #endregion Private fields.

        #region Constructors.

        /// <summary>Initializes a new <see cref="CollectionTypeData"/> with the specified values.</summary>
        /// <param name="type">Collection type (possibly generic definition).</param>
        private CollectionTypeData(Type type)
        {
            Debug.Assert(type != null, "type != null");

            this.type = type;
        }

        #endregion Constructors.

        #region Public properties.

        /// <summary>Gets interesting collection types.</summary>
        public static CollectionTypeData[] Values
        {
            get
            {
                if (values == null)
                {
                    values = new CollectionTypeData[]
                    {
                        // System.Collections
                        new CollectionTypeData(typeof(ArrayList)),
                        new CollectionTypeData(typeof(Hashtable)),
                        new CollectionTypeData(typeof(Queue)),
                        new CollectionTypeData(typeof(SortedList)),
                        new CollectionTypeData(typeof(Stack)),
                        new CollectionTypeData(typeof(ICollection)),
                        new CollectionTypeData(typeof(IEnumerable)),
                        new CollectionTypeData(typeof(IEnumerator)),
                        new CollectionTypeData(typeof(IList)),

                        // System.Collections.Generic
                        new CollectionTypeData(typeof(HashSet<>)),
                        new CollectionTypeData(typeof(LinkedList<>)),
                        new CollectionTypeData(typeof(List<>)),
                        new CollectionTypeData(typeof(Queue<>)),
                        new CollectionTypeData(typeof(Stack<>)),
                        new CollectionTypeData(typeof(SynchronizedCollection<>)),                        
                        new CollectionTypeData(typeof(ICollection<>)),
                        new CollectionTypeData(typeof(IEnumerable<>)),
                        new CollectionTypeData(typeof(IEnumerator<>)),
                        new CollectionTypeData(typeof(IList<>)),

                        // System.Collections.ObjectModel
                        new CollectionTypeData(typeof(System.Collections.ObjectModel.Collection<>)),
                        new CollectionTypeData(typeof(System.Collections.ObjectModel.ReadOnlyCollection<>)),

                        // System.Collections.Specialized
                        new CollectionTypeData(typeof(System.Collections.Specialized.StringCollection)),
                    };
                }

                return values;
            }
        }

        /// <summary>Whether this type is generic.</summary>
        public bool IsGeneric
        {
            get { return this.type.GetGenericArguments().Length > 0; }
        }

        /// <summary>Collection type (possibly generic definition).</summary>
        public Type Type
        {
            get { return this.type; }
        }

        #endregion Public properties.

        #region Public methods.

        /// <summary>Gets the element type for the collection type.</summary>
        /// <returns>The element type for the collection type; null if this is a generic collection.</returns>
        public Type GetElementType()
        {
            if (this.IsGeneric)
            {
                return null;
            }

            if (this.type == typeof(System.Collections.Specialized.StringCollection))
            {
                return typeof(string);
            }
            else
            {
                return typeof(object);
            }
        }

        /// <summary>Gets a type that can hold instances of <paramref name="elementType"/>.</summary>
        /// <param name="elementType">Type of element.</param>
        /// <returns>A new type.</returns>
        public Type GetTypeForElements(Type elementType)
        {
            if (this.IsGeneric)
            {
                return type.MakeGenericType(elementType);
            }
            else
            {
                return this.type;
            }
        }

        #endregion Public methods.
    }
}
