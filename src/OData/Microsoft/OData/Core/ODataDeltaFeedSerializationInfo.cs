//   OData .NET Libraries ver. 6.8.1
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
