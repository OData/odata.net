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

namespace System.Data.OData
{
    /// <summary>
    /// Constant values used in the OData library.
    /// </summary>
    internal static class ODataConstants
    {
        #region OData constants ------------------------------------------------------------------------------

        /// <summary>Default language for error messages if not specified.</summary>
        internal const string ODataErrorMessageDefaultLanguage = "en-US";

        // TODO: Bug 129884: What should be the default version of ODataLib?

        /// <summary>The default protocol version to use in ODataLib if none is specified.</summary>
        internal const ODataVersion ODataDefaultProtocolVersion = ODataVersion.V2;

        /// <summary>The template used when computing a batch request boundary.</summary>
        internal const string BatchRequestBoundaryTemplate = "batch_{0}";

        /// <summary>The template used when computing a batch response boundary.</summary>
        internal const string BatchResponseBoundaryTemplate = "batchresponse_{0}";

        /// <summary>The template used when computing a request changeset boundary.</summary>
        internal const string RequestChangeSetBoundaryTemplate = "changeset_{0}";

        /// <summary>The template used when computing a response changeset boundary.</summary>
        internal const string ResponseChangeSetBoundaryTemplate = "changeset_{0}";
        #endregion OData constants
    }
}
