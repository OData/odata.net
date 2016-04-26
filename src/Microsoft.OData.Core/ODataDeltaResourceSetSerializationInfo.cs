//---------------------------------------------------------------------
// <copyright file="ODataDeltaResourceSetSerializationInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Class to provide additional serialization information to the <see cref="ODataDeltaWriter"/> for an <see cref="ODataDeltaResourceSet"/>.
    /// </summary>
    public sealed class ODataDeltaResourceSetSerializationInfo
    {
        /// <summary>
        /// The entity set name of the resource/source resource to be written. Should be fully qualified if the entity set is not in the default container.
        /// </summary>
        private string entitySetName;

        /// <summary>
        /// The namespace qualified entity type name of the entity set.
        /// </summary>
        private string entityTypeName;

        /// <summary>
        /// The namespace qualified type name of the expected entity type.
        /// </summary>
        private string expectedEntityTypeName;

        /// <summary>
        /// The entity set name of the resource/source resource to be written. Should be fully qualified if the entity set is not in the default container.
        /// </summary>
        public string EntitySetName
        {
            get
            {
                return this.entitySetName;
            }

            set
            {
                ExceptionUtils.CheckArgumentStringNotNullOrEmpty(value, "EntitySetName");
                this.entitySetName = value;
            }
        }

        /// <summary>
        /// The namespace qualified element type name of the entity set.
        /// </summary>
        public string EntityTypeName
        {
            get
            {
                return this.entityTypeName;
            }

            set
            {
                ExceptionUtils.CheckArgumentStringNotNullOrEmpty(value, "EntityTypeName");
                this.entityTypeName = value;
            }
        }

        /// <summary>
        /// The namespace qualified type name of the expected entity type.
        /// </summary>
        public string ExpectedTypeName
        {
            get
            {
                return this.expectedEntityTypeName ?? this.EntityTypeName;
            }

            set
            {
                ExceptionUtils.CheckArgumentStringNotEmpty(value, "ExpectedTypeName");
                this.expectedEntityTypeName = value;
            }
        }

        /// <summary>
        /// Validates the <paramref name="serializationInfo"/> instance.
        /// </summary>
        /// <param name="serializationInfo">The serialization info instance to validate.</param>
        /// <returns>The <paramref name="serializationInfo"/> instance.</returns>
        internal static ODataDeltaResourceSetSerializationInfo Validate(ODataDeltaResourceSetSerializationInfo serializationInfo)
        {
            if (serializationInfo != null)
            {
                ExceptionUtils.CheckArgumentNotNull(serializationInfo.EntitySetName, "serializationInfo.EntitySetName");
                ExceptionUtils.CheckArgumentNotNull(serializationInfo.EntityTypeName, "serializationInfo.EntityTypeName");
            }

            return serializationInfo;
        }
    }
}
