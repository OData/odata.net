//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Class to provide additional serialization information to the <see cref="ODataWriter"/> for an <see cref="ODataEntry"/>.
    /// </summary>
    public sealed class ODataFeedAndEntrySerializationInfo
    {
        /// <summary>
        /// The navigation source name of the entry to be written. Should be fully qualified if the navigation source is not in the default container.
        /// </summary>
        private string navigationSourceName;

        /// <summary>
        /// The namespace qualified entity type name of the navigation source.
        /// </summary>
        private string navigationSourceEntityTypeName;

        /// <summary>
        /// The namespace qualified type name of the expected entity type.
        /// </summary>
        private string expectedTypeName;

        /// <summary>
        /// The navigation source name of the entry to be written. Should be fully qualified if the navigation source is not in the default container.
        /// </summary>
        public string NavigationSourceName
        {
            get
            {
                return this.navigationSourceName;
            }

            set
            {
                ExceptionUtils.CheckArgumentStringNotNullOrEmpty(value, "NavigationSourceName");
                this.navigationSourceName = value;
            }
        }

        /// <summary>
        /// The namespace qualified element type name of the navigation source.
        /// </summary>
        public string NavigationSourceEntityTypeName
        {
            get
            {
                return this.navigationSourceEntityTypeName;
            }

            set
            {
                ExceptionUtils.CheckArgumentStringNotNullOrEmpty(value, "NavigationSourceEntityTypeName");
                this.navigationSourceEntityTypeName = value;
            }
        }

        /// <summary>
        /// The kind of the navigation source.
        /// </summary>
        public EdmNavigationSourceKind NavigationSourceKind { get; set; }

        /// <summary>
        /// The flag we use to identify if the current entry is from a navigation property with collection type or not.
        /// </summary>
        public bool IsFromCollection { get; set; }

        /// <summary>
        /// The namespace qualified type name of the expected entity type.
        /// </summary>
        public string ExpectedTypeName
        {
            get
            {
                return this.expectedTypeName ?? this.NavigationSourceEntityTypeName;
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
            if (serializationInfo != null)
            {
                ExceptionUtils.CheckArgumentNotNull(serializationInfo.NavigationSourceName, "serializationInfo.NavigationSourceName");
                ExceptionUtils.CheckArgumentNotNull(serializationInfo.NavigationSourceEntityTypeName, "serializationInfo.NavigationSourceEntityTypeName");
            }

            return serializationInfo;
        }
    }
}
