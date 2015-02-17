//---------------------------------------------------------------------
// <copyright file="OperationDescriptorData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents data for the OperationDescriptor (from Microsoft.OData.Client namespace).
    /// </summary>
    public class OperationDescriptorData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationDescriptorData"/> class.
        /// </summary>
        /// <param name="metadata">The metadata of OperationDescriptor</param>
        /// <param name="target">The target of OperationDescriptor</param>
        /// <param name="title">The title of OperationDescriptor</param>
        /// <param name="isAction">Whether the operation is action</param>
        internal OperationDescriptorData(Uri metadata, Uri target, string title, bool isAction)
        {
            this.Metadata = metadata;
            this.Target = target;
            this.Title = title;
            this.IsAction = isAction;
        }

        /// <summary>
        /// Gets or sets the Metadata
        /// </summary>
        public Uri Metadata { get; set; }

        /// <summary>
        /// Gets or sets the Target link
        /// </summary>
        public Uri Target { get; set; }

        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation is an action
        /// </summary>
        internal bool IsAction { get; set; }

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
                "{{ OperationDescriptor {{ Metadata = {0}, Target = {1}, Title = {2}, IsAction = {3} }}}}",
                this.Metadata,
                this.Target,
                this.Title,
                this.IsAction);
        }

        /// <summary>
        /// Returns another OperationDescriptorData with equivalent values but no references to the current instance
        /// </summary>
        /// <returns>A cloned OperationDescriptorData</returns>
        public OperationDescriptorData Clone()
        {
            var clone = new OperationDescriptorData(this.Metadata, this.Target, this.Title, this.IsAction);
            return clone;
        }
    }
}