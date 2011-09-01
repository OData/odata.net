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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces

    /// <summary>
    /// Class for reading OData batch messages; also verifies the proper sequence of read calls on the reader.
    /// </summary>
    public sealed class ODataBatchReader
    {
        /// <summary>The input context to read the content from.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Not yet used.")]
        private readonly ODataRawInputContext inputContext;

        /// <summary>The boundary string for the batch structure itself.</summary>
        private readonly string batchBoundary;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input context to read the content from.</param>
        /// <param name="batchBoundary">The boundary string for the batch structure itself.</param>
        internal ODataBatchReader(ODataRawInputContext inputContext, string batchBoundary)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(inputContext != null, "inputContext != null");
            Debug.Assert(batchBoundary != null, "batchBoundary != null");

            this.inputContext = inputContext;
            this.batchBoundary = batchBoundary;

            // TODO: implement batch reading; we do not throw so we can create the reader for use in the content type tests
            // throw Error.NotImplemented();
        }
    }
}
