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
