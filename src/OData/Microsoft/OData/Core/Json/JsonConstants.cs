//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#if SPATIAL
namespace Microsoft.Data.Spatial
#else
namespace Microsoft.OData.Core.Json
#endif
{
    #region Namespaces

    #endregion Namespaces

    /// <summary>
    /// Constants for the JSON format.
    /// </summary>
    internal static class JsonConstants
    {
        /// <summary>"actions" header for entry metadata.</summary>
        internal const string ODataActionsMetadataName = "actions";

        /// <summary>"functions" header for entry metadata.</summary>
        internal const string ODataFunctionsMetadataName = "functions";

        /// <summary>"title" header for "actions" and "functions" metadata.</summary>
        internal const string ODataOperationTitleName = "title";

        /// <summary>"metadata" header for "actions" and "functions" metadata.</summary>
        internal const string ODataOperationMetadataName = "metadata";

        /// <summary>"target" header for "actions" and "functions" metadata.</summary>
        internal const string ODataOperationTargetName = "target";

        /// <summary>
        /// "error" header for the error payload
        /// </summary>
        internal const string ODataErrorName = "error";

        /// <summary>
        /// "code" header for the error code property
        /// </summary>
        internal const string ODataErrorCodeName = "code";

        /// <summary>
        /// "message" header for the error message property
        /// </summary>
        internal const string ODataErrorMessageName = "message";

        /// <summary>
        /// "innererror" header for the inner error property
        /// </summary>
        internal const string ODataErrorInnerErrorName = "innererror";

        /// <summary>
        /// "message" header for an inner error (for Astoria compatibility)
        /// </summary>
        internal const string ODataErrorInnerErrorMessageName = "message";

        /// <summary>
        /// "typename" header for an inner error (for Astoria compatibility)
        /// </summary>
        internal const string ODataErrorInnerErrorTypeNameName = "type";

        /// <summary>
        /// "stacktrace" header for an inner error (for Astoria compatibility)
        /// </summary>
        internal const string ODataErrorInnerErrorStackTraceName = "stacktrace";

        /// <summary>
        /// "internalexception" header for an inner, inner error property (for Astoria compatibility)
        /// </summary>
        internal const string ODataErrorInnerErrorInnerErrorName = "internalexception";

        /// <summary>
        /// JSON datetime format.
        /// </summary>
        internal const string ODataDateTimeFormat = @"\/Date({0})\/";

        /// <summary>
        /// JSON datetime offset format.
        /// </summary>
        internal const string ODataDateTimeOffsetFormat = @"\/Date({0}{1}{2:D4})\/";

        /// <summary>
        /// A plus sign for the date time offset format.
        /// </summary>
        internal const string ODataDateTimeOffsetPlusSign = "+";

        /// <summary>
        /// The fixed property name for the entity sets array in a service document payload.
        /// </summary>
        internal const string ODataServiceDocumentEntitySetsName = "EntitySets";

        /// <summary>
        /// The true value literal.
        /// </summary>
        internal const string JsonTrueLiteral = "true";

        /// <summary>
        /// The false value literal.
        /// </summary>
        internal const string JsonFalseLiteral = "false";

        /// <summary>
        /// The null value literal.
        /// </summary>
        internal const string JsonNullLiteral = "null";

        /// <summary>
        /// Character which starts the object scope.
        /// </summary>
        internal const string StartObjectScope = "{";

        /// <summary>
        /// Character which ends the object scope.
        /// </summary>
        internal const string EndObjectScope = "}";

        /// <summary>
        /// Character which starts the array scope.
        /// </summary>
        internal const string StartArrayScope = "[";

        /// <summary>
        /// Character which ends the array scope.
        /// </summary>
        internal const string EndArrayScope = "]";

        /// <summary>
        /// "(" Json Padding Function scope open parens.
        /// </summary>
        internal const string StartPaddingFunctionScope = "(";

        /// <summary>
        /// ")" Json Padding Function scope close parens.
        /// </summary>
        internal const string EndPaddingFunctionScope = ")";

        /// <summary>
        /// The separator between object members.
        /// </summary>
        internal const string ObjectMemberSeparator = ",";

        /// <summary>
        /// The separator between array elements.
        /// </summary>
        internal const string ArrayElementSeparator = ",";

        /// <summary>
        /// The separator between the name and the value.
        /// </summary>
        internal const string NameValueSeparator = ":";

        /// <summary>
        /// The quote character.
        /// </summary>
        internal const char QuoteCharacter = '"';
    }
}
