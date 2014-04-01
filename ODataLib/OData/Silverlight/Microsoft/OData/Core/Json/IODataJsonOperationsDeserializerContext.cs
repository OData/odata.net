//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Json
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>
    /// Interface representing a context necessary for reading JSON operations values.
    /// </summary>
    internal interface IODataJsonOperationsDeserializerContext
    {
        /// <summary>
        /// The JSON reader to read the operations value from.
        /// </summary>
        JsonReader JsonReader { get; }

        /// <summary>
        /// Given a URI from the payload, this method will try to make it absolute, or fail otherwise.
        /// </summary>
        /// <param name="uriFromPayload">The URI string from the payload to process.</param>
        /// <returns>An absolute URI to report.</returns>
        Uri ProcessUriFromPayload(string uriFromPayload);

        /// <summary>
        /// Adds the specified action to the current entry.
        /// </summary>
        /// <param name="action">The action whcih is fully populated with the data from the payload.</param>
        void AddActionToEntry(ODataAction action);

        /// <summary>
        /// Adds the specified function to the current entry.
        /// </summary>
        /// <param name="function">The function whcih is fully populated with the data from the payload.</param>
        void AddFunctionToEntry(ODataFunction function);
    }
}
