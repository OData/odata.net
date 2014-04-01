//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
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
