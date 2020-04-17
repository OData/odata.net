//---------------------------------------------------------------------
// <copyright file="JsonConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Constants for the JSON writer.
    /// </summary>
    internal static class JsonConstants
    {
        /// <summary>
        /// The indentation string to prepand to each line for each indentation level.
        /// </summary>
        internal const string IndentationString = "  ";

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
        /// The quote character.
        /// </summary>
        internal const char QuoteCharacter = '"';

        /// <summary>
        /// The colon character.
        /// </summary>
        internal const char ColonCharacter = ':';

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
        internal const string NameValueSeparator = ": ";

        /// <summary>
        /// The white space for empty object
        /// </summary>
        internal const string WhiteSpaceForEmptyObject = " ";

        /// <summary>
        /// The white space for empty array
        /// </summary>
        internal const string WhiteSpaceForEmptyArray = " ";

        /// <summary>
        /// Empty object
        /// </summary>
        /// <remarks>To indicate empty object in JSON.</remarks>
        internal const string EmptyObject = "{ }";

        /// <summary>
        /// Empty array
        /// </summary>
        /// <remarks>To indicate empty array in JSON.</remarks>
        internal const string EmptyArray = "[ ]";

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
    }
}