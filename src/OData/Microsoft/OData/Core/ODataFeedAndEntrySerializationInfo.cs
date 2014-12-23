//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
