//---------------------------------------------------------------------
// <copyright file="ODataDeltaResourceSet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Describes a delta response.
    /// </summary>
    public sealed class ODataDeltaResourceSet : ODataResourceSetBase
    {
        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataDeltaWriter"/> for this <see cref="ODataDeltaResourceSet"/>.
        /// </summary>
        private ODataDeltaResourceSetSerializationInfo serializationInfo;

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataDeltaWriter"/> for this <see cref="ODataDeltaResourceSet"/>.
        /// </summary>
        internal ODataDeltaResourceSetSerializationInfo SerializationInfo
        {
            get
            {
                return this.serializationInfo;
            }

            set
            {
                this.serializationInfo = ODataDeltaResourceSetSerializationInfo.Validate(value);
            }
        }
    }
}
