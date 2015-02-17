//---------------------------------------------------------------------
// <copyright file="StreamDescriptorData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents data for the StreamDescriptor (from Microsoft.OData.Client namespace).
    /// </summary>
    public sealed class StreamDescriptorData : DescriptorData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDescriptorData"/> class.
        /// </summary>
        /// <param name="entityDescriptorData">The entity descriptor data containing the stream</param>
        internal StreamDescriptorData(EntityDescriptorData entityDescriptorData)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityDescriptorData, "entityDescriptorData");
            this.EntityDescriptor = entityDescriptorData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDescriptorData"/> class.
        /// </summary>
        /// <param name="entityDescriptorData">The entity descriptor data containing the stream</param>
        /// <param name="name">The name of the stream</param>
        internal StreamDescriptorData(EntityDescriptorData entityDescriptorData, string name)
            : this(entityDescriptorData)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the name of the stream descriptor
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the self link
        /// </summary>
        public Uri SelfLink { get; set; }

        /// <summary>
        /// Gets or sets the edit link
        /// </summary>
        public Uri EditLink { get; set; }

        /// <summary>
        /// Gets or sets the content type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets the entity descriptor data that contains this stream
        /// </summary>
        public EntityDescriptorData EntityDescriptor { get; private set; }

        /// <summary>
        /// Gets or sets the ETag
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the save stream data.
        /// </summary>
        internal SaveStreamData SaveStream { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{{ State = {0}, Name = '{1}', Edit = '{2}', Self = '{3}', ContentType = '{4}', ETag = '{5}' }}",
                this.State,
                this.Name,
                this.EditLink,
                this.SelfLink,
                this.ContentType,
                this.ETag);
        }

        /// <summary>
        /// Clones the current stream descriptor data
        /// </summary>
        /// <param name="clonedEntityDescriptor">The entity descriptor that will contain the result of this clone</param>
        /// <returns>A clone of the current stream descriptor data</returns>
        public StreamDescriptorData Clone(EntityDescriptorData clonedEntityDescriptor)
        {
            var clone = new StreamDescriptorData(clonedEntityDescriptor, this.Name);
            clone.State = this.State;
            clone.ChangeOrder = this.ChangeOrder;
            
            clone.ContentType = this.ContentType;
            clone.ETag = this.ETag;
            
            if (this.EditLink != null)
            {
                clone.EditLink = new Uri(this.EditLink.OriginalString, UriKind.RelativeOrAbsolute);
            }

            if (this.SelfLink != null)
            {
                clone.SelfLink = new Uri(this.SelfLink.OriginalString, UriKind.RelativeOrAbsolute);
            }

            if (this.SaveStream != null)
            {
                clone.SaveStream = this.SaveStream.Clone();
            }

            return clone;
        }
    }
}