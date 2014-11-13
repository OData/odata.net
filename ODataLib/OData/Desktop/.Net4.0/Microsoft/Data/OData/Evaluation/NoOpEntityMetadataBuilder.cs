//   OData .NET Libraries ver. 5.6.3
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

namespace Microsoft.Data.OData.Evaluation
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
            return this.entry.NonComputedReadLink;
        }

        /// <summary>
        /// Gets the ID of the entity.
        /// </summary>
        /// <returns>
        /// The ID for the entity.
        /// </returns>
        internal override string GetId()
        {
            DebugUtils.CheckNoExternalCallers();
            return this.entry.NonComputedId;
        }

        /// <summary>
        /// Gets the ETag of the entity.
        /// </summary>
        /// <returns>
        /// The ETag for the entity.
        /// </returns>
        internal override string GetETag()
        {
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
            return this.entry.NonComputedMediaResource;
        }

        /// <summary>
        /// Gets the entity properties.
        /// </summary>
        /// <param name="nonComputedProperties">Non-computed properties from the entity.</param>
        /// <returns>The the computed and non-computed entity properties.</returns>
        internal override IEnumerable<ODataProperty> GetProperties(IEnumerable<ODataProperty> nonComputedProperties)
        {
            DebugUtils.CheckNoExternalCallers();
            return nonComputedProperties;
        }


        /// <summary>
        /// Gets the list of computed and non-computed actions for the entity.
        /// </summary>
        /// <returns>The list of computed and non-computed actions for the entity.</returns>
        internal override IEnumerable<ODataAction> GetActions()
        {
            DebugUtils.CheckNoExternalCallers();
            return this.entry.NonComputedActions;
        }

        /// <summary>
        /// Gets the list of computed and non-computed functions for the entity.
        /// </summary>
        /// <returns>The list of computed and non-computed functions for the entity.</returns>
        internal override IEnumerable<ODataFunction> GetFunctions()
        {
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
            return associationLinkUrl;
        }
    }
}
