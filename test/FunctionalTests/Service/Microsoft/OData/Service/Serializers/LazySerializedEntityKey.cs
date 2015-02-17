//---------------------------------------------------------------------
// <copyright file="LazySerializedEntityKey.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// Representation of the identity and edit-link of an entity that lazily builds them on demand.
    /// </summary>
    internal class LazySerializedEntityKey : SerializedEntityKey
    {
        /// <summary>Lazy storage for the edit link as an absolute URI without any type segments.</summary>
        private readonly SimpleLazy<Uri> lazyAbsoluteEditLinkWithoutSuffix;

        /// <summary>Lazy storage for the edit link as an absolute URI.</summary>
        private readonly SimpleLazy<Uri> lazyAbsoluteEditLink;

        /// <summary>Lazy storage for the edit link as a relative URI.</summary>
        private readonly SimpleLazy<Uri> lazyRelativeEditLink;

        /// <summary>Lazy storage for the identity.</summary>
        private readonly SimpleLazy<Uri> lazyIdentity;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazySerializedEntityKey"/> class which uses the same syntax for identity and edit link.
        /// </summary>
        /// <param name="lazyRelativeIdentity">The identity as a lazy string relative to the service URI.</param>
        /// <param name="absoluteServiceUri">The absolute service URI.</param>
        /// <param name="editLinkSuffix">The optional suffix to append to the edit link. Null means nothing will be appended.</param>
        internal LazySerializedEntityKey(SimpleLazy<string> lazyRelativeIdentity, Uri absoluteServiceUri, string editLinkSuffix)
        {
            Debug.Assert(absoluteServiceUri != null && absoluteServiceUri.IsAbsoluteUri, "absoluteServiceUri != null && absoluteServiceUri.IsAbsoluteUri");

            this.lazyAbsoluteEditLinkWithoutSuffix = new SimpleLazy<Uri>(() => RequestUriProcessor.AppendEscapedSegment(absoluteServiceUri, lazyRelativeIdentity.Value));
            this.lazyIdentity = new SimpleLazy<Uri>(() => this.lazyAbsoluteEditLinkWithoutSuffix.Value, false);
            this.lazyAbsoluteEditLink = AppendLazilyIfNeeded(this.lazyAbsoluteEditLinkWithoutSuffix, editLinkSuffix);
            
            SimpleLazy<Uri> relativeEdit = new SimpleLazy<Uri>(() => new Uri(lazyRelativeIdentity.Value, UriKind.Relative), false);
            this.lazyRelativeEditLink = AppendLazilyIfNeeded(relativeEdit, editLinkSuffix);
        }

        /// <summary>
        /// Gets the edit link of the entity relative to the service base.
        /// </summary>
        internal override Uri RelativeEditLink
        {
            get { return this.lazyRelativeEditLink.Value; }
        }

        /// <summary>
        /// Gets the identity of the entity.
        /// </summary>
        internal override Uri Identity
        {
            get { return this.lazyIdentity.Value; }
        }

        /// <summary>
        /// Gets the absolute edit link of the entity.
        /// </summary>
        internal override Uri AbsoluteEditLink
        {
            get { return this.lazyAbsoluteEditLink.Value; }
        }

        /// <summary>
        /// Gets the absolute edit link of the entity without a type segment or other suffix.
        /// </summary>
        internal override Uri AbsoluteEditLinkWithoutSuffix
        {
            get { return this.lazyAbsoluteEditLinkWithoutSuffix.Value; }
        }

        /// <summary>
        /// Creates an instance of <see cref="SerializedEntityKey"/> for the given properties and values.
        /// </summary>
        /// <param name="keySerializer">The key serializer to use.</param>
        /// <param name="absoluteServiceUri">The absolute service URI.</param>
        /// <param name="entitySetName">Name of the entity set.</param>
        /// <param name="keyProperties">The key properties.</param>
        /// <param name="getPropertyValue">The callback to get each property's value.</param>
        /// <param name="editLinkSuffix">The suffix to append to the edit-link, or null.</param>
        /// <returns>A serialized-key instance.</returns>
        internal static SerializedEntityKey Create(
            KeySerializer keySerializer, 
            Uri absoluteServiceUri,
            string entitySetName, 
            ICollection<ResourceProperty> keyProperties,
            Func<ResourceProperty, object> getPropertyValue, 
            string editLinkSuffix)
        {
            SimpleLazy<string> lazyRelativeIdentity = new SimpleLazy<string>(() =>
            {
                var builder = new StringBuilder();
                builder.Append(entitySetName);
                keySerializer.AppendKeyExpression(builder, keyProperties, p => p.Name, getPropertyValue);
                return builder.ToString();
            });
                
            return new LazySerializedEntityKey(lazyRelativeIdentity, absoluteServiceUri, editLinkSuffix);
        }

        /// <summary>
        /// Wraps a lazy URI with another that will have the given string appended if it is not null.
        /// </summary>
        /// <param name="lazyUri">The lazy URI to wrap.</param>
        /// <param name="suffix">The suffix for the URI.</param>
        /// <returns>A new lazy URI which will have the suffix, or the same instance if the suffix was null.</returns>
        private static SimpleLazy<Uri> AppendLazilyIfNeeded(SimpleLazy<Uri> lazyUri, string suffix)
        {
            return suffix == null ? lazyUri : new SimpleLazy<Uri>(() => RequestUriProcessor.AppendUnescapedSegment(lazyUri.Value, suffix), false);
        }
    }
}