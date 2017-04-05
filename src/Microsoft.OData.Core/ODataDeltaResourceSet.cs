//---------------------------------------------------------------------
// <copyright file="ODataDeltaResourceSet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Metadata;

namespace Microsoft.OData
{
    /// <summary>
    /// Describes a delta response.
    /// </summary>
    public sealed class ODataDeltaResourceSet : ODataResourceSetBase
    {
        /// <summary>
        /// The type name of the resource set.
        /// </summary>
        private string typeName;

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataDeltaWriter"/> for this <see cref="ODataDeltaResourceSet"/>.
        /// </summary>
        private ODataDeltaResourceSetSerializationInfo serializationInfo;

        /// <summary>Gets the resource set type name.</summary>
        /// <returns>The resource set type name.</returns>
        public string TypeName
        {
            get
            {
                if (typeName == null && this.SerializationInfo != null && this.SerializationInfo.ExpectedTypeName != null)
                {
                    typeName = EdmLibraryExtensions.GetCollectionTypeName(this.serializationInfo.ExpectedTypeName);
                }

                return typeName;
            }

            set
            {
                this.typeName = value;
            }
        }

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
