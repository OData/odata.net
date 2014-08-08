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

namespace System.Data.Services
{
    /// <summary>
    /// This interface declares the methods required to support enumerators for results and for
    /// associated segments on a WCF Data Service $expand query option.
    /// </summary>
    public interface IExpandedResult
    {
        /// <summary>Gets the element with expanded properties.</summary>
        /// <returns>The object in a property expanded by <see cref="T:System.Data.Services.IExpandedResult" />.</returns>
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
