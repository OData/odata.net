//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.OData.Json
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
