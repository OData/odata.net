//---------------------------------------------------------------------
// <copyright file="ODataResourceSetBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using ODataErrorStrings = Microsoft.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Describes a collection of entities.
    /// </summary>
    public abstract class ODataResourceSetBase : ODataItem
    {
        /// <summary>
        /// URI representing the next page link.
        /// </summary>
        private Uri nextPageLink;

        /// <summary>
        /// URI representing the delta link.
        /// </summary>
        private Uri deltaLink;

        /// <summary>Gets or sets the number of items in the resource set.</summary>
        /// <returns>The number of items in the resource set.</returns>
        public long? Count
        {
            get;
            set;
        }

        /// <summary>Gets or sets the URI that identifies the entity set represented by the resource set.</summary>
        /// <returns>The URI that identifies the entity set represented by the resource set.</returns>
        public Uri Id
        {
            get;
            set;
        }

        /// <summary>Gets or sets the URI representing the next page link.</summary>
        /// <returns>The URI representing the next page link.</returns>
        public Uri NextPageLink
        {
            get
            {
                return this.nextPageLink;
            }

            set
            {
                if (this.DeltaLink != null && value != null)
                {
                    throw new ODataException(ODataErrorStrings.ODataResourceSet_MustNotContainBothNextPageLinkAndDeltaLink);
                }

                this.nextPageLink = value;
            }
        }

        /// <summary>
        /// URI representing the delta link.
        /// </summary>
        public Uri DeltaLink
        {
            get
            {
                return this.deltaLink;
            }

            set
            {
                if (this.NextPageLink != null && value != null)
                {
                    throw new ODataException(ODataErrorStrings.ODataResourceSet_MustNotContainBothNextPageLinkAndDeltaLink);
                }

                this.deltaLink = value;
            }
        }

        /// <summary>
        /// Collection of custom instance annotations.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
        public ICollection<ODataInstanceAnnotation> InstanceAnnotations
        {
            get { return this.GetInstanceAnnotations(); }
            set { this.SetInstanceAnnotations(value); }
        }
    }
}
