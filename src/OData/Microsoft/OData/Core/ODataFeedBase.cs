//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion Namespaces

    /// <summary>
    /// Describes a collection of entities.
    /// </summary>
    public abstract class ODataFeedBase : ODataItem
    {
        /// <summary>
        /// URI representing the next page link.
        /// </summary>
        private Uri nextPageLink;

        /// <summary>
        /// URI representing the delta link.
        /// </summary>
        private Uri deltaLink;

        /// <summary>Gets or sets the number of items in the feed.</summary>
        /// <returns>The number of items in the feed.</returns>
        public long? Count 
        { 
            get; 
            set; 
        }

        /// <summary>Gets or sets the URI that identifies the entity set represented by the feed.</summary>
        /// <returns>The URI that identifies the entity set represented by the feed.</returns>
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
                    throw new ODataException(ODataErrorStrings.ODataFeed_MustNotContainBothNextPageLinkAndDeltaLink);
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
                    throw new ODataException(ODataErrorStrings.ODataFeed_MustNotContainBothNextPageLinkAndDeltaLink);
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
