//---------------------------------------------------------------------
// <copyright file="ODataDeltaFeed.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    /// <summary>
    /// Describes a delta response.
    /// </summary>
    public sealed class ODataDeltaFeed : ODataFeedBase
    {
        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataDeltaWriter"/> for this <see cref="ODataDeltaFeed"/>.
        /// </summary>
        private ODataDeltaFeedSerializationInfo serializationInfo;

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataDeltaWriter"/> for this <see cref="ODataDeltaFeed"/>.
        /// </summary>
        internal ODataDeltaFeedSerializationInfo SerializationInfo
        {
            get
            {
                return this.serializationInfo;
            }

            set
            {
                this.serializationInfo = ODataDeltaFeedSerializationInfo.Validate(value);
            }
        }
    }
}
