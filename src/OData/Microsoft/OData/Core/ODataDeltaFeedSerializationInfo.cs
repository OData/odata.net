//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    /// <summary>
    /// Class to provide additional serialization information to the <see cref="ODataDeltaWriter"/> for an <see cref="ODataDeltaFeed"/>.
    /// </summary>
    public sealed class ODataDeltaFeedSerializationInfo
    {
        /// <summary>
        /// The entity set name of the entry/source entry to be written. Should be fully qualified if the entity set is not in the default container.
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
        /// The entity set name of the entry/source entry to be written. Should be fully qualified if the entity set is not in the default container.
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
        internal static ODataDeltaFeedSerializationInfo Validate(ODataDeltaFeedSerializationInfo serializationInfo)
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
