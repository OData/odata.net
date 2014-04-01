//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    /// <summary>
    /// Class to provide additional serialization information to the <see cref="ODataCollectionWriter"/> for an <see cref="ODataCollectionStart"/>.
    /// </summary>
    public sealed class ODataCollectionStartSerializationInfo
    {
        /// <summary>
        /// The fully qualified type name of the collection to be written.
        /// </summary>
        private string collectionTypeName;

        /// <summary>
        /// The fully qualified type name of the collection to be written.
        /// </summary>
        public string CollectionTypeName
        {
            get
            {
                return this.collectionTypeName;
            }

            set
            {
                ExceptionUtils.CheckArgumentStringNotNullOrEmpty(value, "CollectionTypeName");
                ValidationUtils.ValidateCollectionTypeName(value);
                this.collectionTypeName = value;
            }
        }

        /// <summary>
        /// Validates the <paramref name="serializationInfo"/> instance.
        /// </summary>
        /// <param name="serializationInfo">The serialization info instance to validate.</param>
        /// <returns>The <paramref name="serializationInfo"/> instance.</returns>
        internal static ODataCollectionStartSerializationInfo Validate(ODataCollectionStartSerializationInfo serializationInfo)
        {
            if (serializationInfo != null)
            {
                ExceptionUtils.CheckArgumentNotNull(serializationInfo.CollectionTypeName, "serializationInfo.CollectionTypeName");
            }

            return serializationInfo;
        }
    }
}
