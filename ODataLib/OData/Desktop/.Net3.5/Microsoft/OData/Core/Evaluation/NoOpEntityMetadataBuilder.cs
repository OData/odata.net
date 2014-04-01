//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Implementation of the metadata builder which only returns values which were explicitly set (never computing or modifying them).
    /// </summary>
    internal sealed class NoOpEntityMetadataBuilder : ODataEntityMetadataBuilder
    {
        /// <summary>
        /// The entry whose payload metadata is being queried.
        /// </summary>
        private readonly ODataEntry entry;

        /// <summary>
        /// Creates a new no-op metadata builder.
        /// </summary>
        /// <param name="entry">The entry whose payload metadata is being queried.</param>
        internal NoOpEntityMetadataBuilder(ODataEntry entry)
        {
            Debug.Assert(entry != null, "entry != null");

            this.entry = entry;
        }

        /// <summary>
        /// Gets the edit link of the entity.
        /// </summary>
        /// <returns>
        /// The absolute URI of the edit link for the entity.
        /// </returns>
        internal override Uri GetEditLink()
        {
            return this.entry.NonComputedEditLink;
        }

        /// <summary>
        /// Gets the read link of the entity.
        /// </summary>
        /// <returns>
        /// The absolute URI of the read link for the entity.
        /// </returns>
        internal override Uri GetReadLink()
        {
            return this.entry.NonComputedReadLink;
        }

        /// <summary>
        /// Gets the ID of the entity.
        /// </summary>
        /// <returns>
        /// The ID for the entity.
        /// </returns>
        internal override Uri GetId()
        {
            return this.entry.IsTransient ? null : this.entry.NonComputedId;
        }

        /// <summary>
        /// Gets the ETag of the entity.
        /// </summary>
        /// <returns>
        /// The ETag for the entity.
        /// </returns>
        internal override string GetETag()
        {
            return this.entry.NonComputedETag;
        }

        /// <summary>
        /// Gets the default media resource of the entity.
        /// </summary>
        /// <returns>
        /// The the default media resource of the entity.
        /// Or null if the entity is not an MLE.
        /// </returns>
        internal override ODataStreamReferenceValue GetMediaResource()
        {
            return this.entry.NonComputedMediaResource;
        }

        /// <summary>
        /// Gets the entity properties.
        /// </summary>
        /// <param name="nonComputedProperties">Non-computed properties from the entity.</param>
        /// <returns>The the computed and non-computed entity properties.</returns>
        internal override IEnumerable<ODataProperty> GetProperties(IEnumerable<ODataProperty> nonComputedProperties)
        {
            return nonComputedProperties;
        }


        /// <summary>
        /// Gets the list of computed and non-computed actions for the entity.
        /// </summary>
        /// <returns>The list of computed and non-computed actions for the entity.</returns>
        internal override IEnumerable<ODataAction> GetActions()
        {
            return this.entry.NonComputedActions;
        }

        /// <summary>
        /// Gets the list of computed and non-computed functions for the entity.
        /// </summary>
        /// <returns>The list of computed and non-computed functions for the entity.</returns>
        internal override IEnumerable<ODataFunction> GetFunctions()
        {
            return this.entry.NonComputedFunctions;
        }

        /// <summary>
        /// Gets the navigation link URI for the specified navigation property.
        /// </summary>
        /// <param name="navigationPropertyName">The name of the navigation property to get the navigation link URI for.</param>
        /// <param name="navigationLinkUrl">The value of the link URI as seen on the wire or provided explicitly by the user or previously returned by the metadata builder, which may be null.</param>
        /// <param name="hasNavigationLinkUrl">true if the value of the <paramref name="navigationLinkUrl"/> was seen on the wire or provided explicitly by the user or previously returned by
        /// the metadata builder, false otherwise. This flag allows the metadata builder to determine whether a null navigation link url is an uninitialized value or a value that was set explicitly.</param>
        /// <returns>
        /// The navigation link URI for the navigation property.
        /// null if its not possible to determine the navigation link for the specified navigation property.
        /// </returns>
        internal override Uri GetNavigationLinkUri(string navigationPropertyName, Uri navigationLinkUrl, bool hasNavigationLinkUrl)
        {
            return navigationLinkUrl;
        }

        /// <summary>
        /// Gets the association link URI for the specified navigation property.
        /// </summary>
        /// <param name="navigationPropertyName">The name of the navigation property to get the association link URI for.</param>
        /// <param name="associationLinkUrl">The value of the link URI as seen on the wire or provided explicitly by the user or previously returned by the metadata builder, which may be null.</param>
        /// <param name="hasAssociationLinkUrl">true if the value of the <paramref name="associationLinkUrl"/> was seen on the wire or provided explicitly by the user or previously returned by
        /// the metadata builder, false otherwise. This flag allows the metadata builder to determine whether a null association link url is an uninitialized value or a value that was set explicitly.</param>
        /// <returns>
        /// The association link URI for the navigation property.
        /// null if its not possible to determine the association link for the specified navigation property.
        /// </returns>
        internal override Uri GetAssociationLinkUri(string navigationPropertyName, Uri associationLinkUrl, bool hasAssociationLinkUrl)
        {
            return associationLinkUrl;
        }

        /// <summary>
        /// Get the id that need to be written into wire
        /// </summary>
        /// <param name="id">The id return to the caller</param>
        /// <returns>
        /// If writer should write odata.id property into wire
        /// </returns>
        internal override bool TryGetIdForSerialization(out Uri id)
        {
            if (this.entry.IsTransient)
            {
                id = null;
                return true;
            }
            else
            {
                id = this.GetId();
                if (id == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
