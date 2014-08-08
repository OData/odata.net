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

namespace Microsoft.Data.OData.Query
{
    /// <summary>
    /// Class to represent a null value with or without type information for URI paremeters.
    /// </summary>
    /// <remarks>This class is only intended for use as a sentinal for null values in URI parameters.  It cannot be used elsewhere.</remarks>
    public sealed class ODataUriNullValue : ODataAnnotatable
    {
        /// <summary>
        /// String representation of the type of this null value. 'null' indicates that no type information was provided.
        /// </summary>
        public string TypeName
        {
            get;
            set;
        }
    }
}
