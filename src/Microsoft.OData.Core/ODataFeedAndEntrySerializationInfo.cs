//---------------------------------------------------------------------
// <copyright file="ODataFeedAndEntrySerializationInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
                if (serializationInfo.NavigationSourceKind != EdmNavigationSourceKind.UnknownEntitySet)
                {
                    ExceptionUtils.CheckArgumentNotNull(serializationInfo.NavigationSourceName, "serializationInfo.NavigationSourceName");
                }

                ExceptionUtils.CheckArgumentNotNull(serializationInfo.NavigationSourceEntityTypeName, "serializationInfo.NavigationSourceEntityTypeName");
            }

            return serializationInfo;
        }
    }
}
