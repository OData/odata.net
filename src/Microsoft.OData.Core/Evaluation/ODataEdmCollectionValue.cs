//---------------------------------------------------------------------
// <copyright file="ODataEdmCollectionValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Evaluation
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.OData.Edm.Values;

    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmCollectionValue"/> implementation of an OData collection value.
    /// </summary>
    internal sealed class ODataEdmCollectionValue : EdmValue, IEdmCollectionValue
    {
        /// <summary>
        /// The <see cref="ODataCollectionValue"/> to convert into an <see cref="IEdmCollectionValue"/>.
        /// </summary>
        private readonly ODataCollectionValue collectionValue;

        /// <summary>
        /// Creates a new instance of an <see cref="ODataEdmCollectionValue"/>.
        /// </summary>
        /// <param name="collectionValue">The <see cref="ODataCollectionValue"/> to create the collection value for.</param>
        internal ODataEdmCollectionValue(ODataCollectionValue collectionValue)
            : base(collectionValue.GetEdmType())
        {
            Debug.Assert(collectionValue != null, "collectionValue != null");

            this.collectionValue = collectionValue;
        }

        /// <summary>
        /// Gets the values stored in this collection.
        /// </summary>
        public IEnumerable<IEdmDelayedValue> Elements
        {
            get 
            {
                if (this.collectionValue != null)
                {
                    IEdmTypeReference itemType = this.Type == null ? null : (this.Type.Definition as IEdmCollectionType).ElementType;
                    foreach (object collectionItem in this.collectionValue.Items)
                    {
                        yield return ODataEdmValueUtils.ConvertValue(collectionItem, itemType);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get 
            {
                return EdmValueKind.Collection;
            }
        }
    }
}
