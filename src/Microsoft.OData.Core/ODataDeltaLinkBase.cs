//---------------------------------------------------------------------
// <copyright file="ODataDeltaLinkBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;

    /// <summary>
    /// Represents either an added link or a deleted link in delta response.
    /// </summary>
    public abstract class ODataDeltaLinkBase : ODataItem
    {
        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataDeltaWriter"/> for this <see cref="ODataDeltaLinkBase"/>.
        /// </summary>
        private ODataDeltaSerializationInfo serializationInfo;

        /// <summary>
        /// Initializes a new <see cref="ODataDeltaLinkBase"/>.
        /// </summary>
        /// <param name="source">The id of the entity from which the relationship is defined, which may be absolute or relative.</param>
        /// <param name="target">The id of the related entity, which may be absolute or relative.</param>
        /// <param name="relationship">The name of the relationship property on the parent object.</param>
        protected ODataDeltaLinkBase(Uri source, Uri target, string relationship)
        {
            this.Source = source;
            this.Target = target;
            this.Relationship = relationship;
        }

        /// <summary>
        /// The id of the entity from which the relationship is defined, which may be absolute or relative.
        /// </summary>
        public Uri Source { get; set; }

        /// <summary>
        /// The id of the related entity, which may be absolute or relative.
        /// </summary>
        public Uri Target { get; set; }

        /// <summary>
        /// The name of the relationship property on the parent object.
        /// </summary>
        public string Relationship { get; set; }

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataDeltaWriter"/> for this <see cref="ODataDeltaLinkBase"/>.
        /// </summary>
        internal ODataDeltaSerializationInfo SerializationInfo
        {
            get
            {
                return this.serializationInfo;
            }

            set
            {
                this.serializationInfo = ODataDeltaSerializationInfo.Validate(value);
            }
        }
    }
}
