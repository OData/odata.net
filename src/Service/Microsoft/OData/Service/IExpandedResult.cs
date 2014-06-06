//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
{
    /// <summary>
    /// This interface declares the methods required to support enumerators for results and for
    /// associated segments on a WCF Data Service $expand query option.
    /// </summary>
    public interface IExpandedResult
    {
        /// <summary>Gets the element with expanded properties.</summary>
        /// <returns>The object in a property expanded by <see cref="T:Microsoft.OData.Service.IExpandedResult" />.</returns>
        object ExpandedElement
        { 
            get;
        }

        /// <summary>Gets the value for a named property of the result.</summary>
        /// <returns>The value of the property.</returns>
        /// <param name="name">The name of the property for which to get enumerable results.</param>
        /// <remarks>
        /// If the element returned in turn has properties which are expanded out-of-band
        /// of the object model, then the result will also be of type <see cref="IExpandedResult"/>,
        /// and the value will be available through <see cref="ExpandedElement"/>.
        /// </remarks>
        object GetExpandedPropertyValue(string name);
    }
}
