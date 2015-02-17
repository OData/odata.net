//---------------------------------------------------------------------
// <copyright file="LinkDescriptorData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System.Globalization;

    /// <summary>
    /// Represents data for the LinkDescriptor (from Microsoft.OData.Client namespace).
    /// </summary>
    public sealed class LinkDescriptorData : DescriptorData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkDescriptorData"/> class.
        /// </summary>
        /// <param name="sourceDescriptor">The entity descriptor for the source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="targetDescriptor">The entity descriptor for the target.</param>
        internal LinkDescriptorData(EntityDescriptorData sourceDescriptor, string sourcePropertyName, EntityDescriptorData targetDescriptor)
            : base()
        {
            this.SourceDescriptor = sourceDescriptor;
            this.TargetDescriptor = targetDescriptor;
            this.SourcePropertyName = sourcePropertyName;
        }

        /// <summary>
        /// Gets the name of the source property.
        /// </summary>
        public string SourcePropertyName { get; private set; }

        /// <summary>
        /// Gets entity descriptor for the source.
        /// </summary>
        /// <value>The source descriptor.</value>
        public EntityDescriptorData SourceDescriptor { get; private set; }

        /// <summary>
        /// Gets or sets entity descriptor for the target.
        /// </summary>
        /// <value>The target descriptor.</value>
        public EntityDescriptorData TargetDescriptor { get; set; }

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
                 "{{ State = {0}, Source = {1}, SourceProperty = '{2}', Target = {3} }}",
                 this.State,
                 this.SourceDescriptor,
                 this.SourcePropertyName,
                 this.TargetDescriptor == null ? "null" : this.TargetDescriptor.ToString());
        }

        /// <summary>
        /// Clones the current link descriptor data
        /// </summary>
        /// <param name="clonedSource">The already-cloned source for the link</param>
        /// <param name="clonedTarget">The already-cloded target for the link</param>
        /// <returns>A clone of this link descriptor data</returns>
        public LinkDescriptorData Clone(EntityDescriptorData clonedSource, EntityDescriptorData clonedTarget)
        {
            var clone = new LinkDescriptorData(clonedSource, this.SourcePropertyName, clonedTarget);
            clone.ChangeOrder = this.ChangeOrder;
            clone.State = this.State;
            return clone;
        }
    }
}