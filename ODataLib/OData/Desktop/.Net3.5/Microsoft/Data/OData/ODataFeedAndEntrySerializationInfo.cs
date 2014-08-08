//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
{
    /// <summary>
    /// Class to provide additional serialization information to the <see cref="ODataWriter"/> for an <see cref="ODataEntry"/>.
    /// </summary>
    public sealed class ODataFeedAndEntrySerializationInfo
    {
        /// <summary>
        /// The entity set name of the entry to be written. Should be fully qualified if the entity set is not in the default container.
        /// </summary>
        private string entitySetName;

        /// <summary>
        /// The namespace qualified element type name of the entity set.
        /// </summary>
        private string entitySetElementTypeName;

        /// <summary>
        /// The namespace qualified type name of the expected entity type.
        /// </summary>
        private string expectedTypeName;

        /// <summary>
        /// The entity set name of the entry to be written. Should be fully qualified if the entity set is not in the default container.
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
        public string EntitySetElementTypeName
        {
            get
            {
                return this.entitySetElementTypeName;
            }

            set
            {
                ExceptionUtils.CheckArgumentStringNotNullOrEmpty(value, "EntitySetElementTypeName");
                this.entitySetElementTypeName = value;
            }
        }

        /// <summary>
        /// The namespace qualified type name of the expected entity type.
        /// </summary>
        public string ExpectedTypeName
        {
            get
            {
                return this.expectedTypeName ?? this.EntitySetElementTypeName;
            }

            set
            {
                ExceptionUtils.CheckArgumentStringNotEmpty(value, "ExpectedTypeName");
                this.expectedTypeName = value;
            }
        }

        /// <summary>
        /// Validates the <paramref name="serializationInfo"/> instance.
        /// </summary>
        /// <param name="serializationInfo">The serialization info instance to validate.</param>
        /// <returns>The <paramref name="serializationInfo"/> instance.</returns>
        internal static ODataFeedAndEntrySerializationInfo Validate(ODataFeedAndEntrySerializationInfo serializationInfo)
        {
            DebugUtils.CheckNoExternalCallers();
            if (serializationInfo != null)
            {
                ExceptionUtils.CheckArgumentNotNull(serializationInfo.EntitySetName, "serializationInfo.EntitySetName");
                ExceptionUtils.CheckArgumentNotNull(serializationInfo.EntitySetElementTypeName, "serializationInfo.EntitySetElementTypeName");
            }

            return serializationInfo;
        }
    }
}
