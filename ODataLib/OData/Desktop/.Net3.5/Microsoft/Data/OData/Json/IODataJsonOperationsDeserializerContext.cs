//   OData .NET Libraries ver. 5.6.3
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
