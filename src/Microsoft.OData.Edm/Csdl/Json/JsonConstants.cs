//---------------------------------------------------------------------
// <copyright file="JsonConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Constants for the JSON format.
    /// </summary>
    internal static class JsonConstants
    {
        /// <summary>"actions" header for resource metadata.</summary>
        internal const string ODataActionsMetadataName = "actions";

        /// <summary>"functions" header for resource metadata.</summary>
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
        /// "target" header for the error message property
        /// </summary>
        internal const string ODataErrorTargetName = "target";

        /// <summary>
        /// "details" header for the inner error property
        /// </summary>
        internal const string ODataErrorDetailsName = "details";

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

    /// <summary>
    /// 
    /// </summary>
    public interface ICharArrayPool
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        char[] Rent(long size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        void Returns(char[] buffer);
    }

    /// <summary>
    /// Helpers to deal with buffers
    /// </summary>
    internal static class BufferUtils
    {
        /// <summary>
        /// Buffer length
        /// </summary>
        private const int BufferLength = 128;

        /// <summary>
        /// Checks if the buffer is not initialized and if initialized returns the same buffer or creates a new one.
        /// </summary>
        /// <param name="buffer">The buffer to verify</param>
        /// <returns>The initialized buffer</returns>
        public static char[] InitializeBufferIfRequired(char[] buffer)
        {
            return InitializeBufferIfRequired(null, buffer);
        }

        /// <summary>
        /// Checks if the buffer is not initialized and if initialized returns the same buffer or creates a new one.
        /// </summary>
        /// <param name="bufferPool">The character pool.</param>
        /// <param name="buffer">The buffer to verify</param>
        /// <returns>The initialized buffer.</returns>
        public static char[] InitializeBufferIfRequired(ICharArrayPool bufferPool, char[] buffer)
        {
            if (buffer != null)
            {
                return buffer;
            }

            return RentFromBuffer(bufferPool, BufferLength);
        }

        /// <summary>
        /// Rents a character array from the pool.
        /// </summary>
        /// <param name="bufferPool">The character pool.</param>
        /// <param name="minSize">The min required size of the character array.</param>
        /// <returns>The character array from the pool.</returns>
        public static char[] RentFromBuffer(ICharArrayPool bufferPool, int minSize)
        {
            if (bufferPool == null)
            {
                return new char[minSize];
            }

            char[] buffer = bufferPool.Rent(minSize);
            if (buffer == null || buffer.Length < minSize)
            {
                // throw new ODataException(Strings.BufferUtils_InvalidBufferOrSize(minSize));
                throw new System.Exception();
            }

            return buffer;
        }

        /// <summary>
        /// Returns a character array to the pool.
        /// </summary>
        /// <param name="bufferPool">The character pool.</param>
        /// <param name="buffer">The character array should be returned.</param>
        public static void ReturnToBuffer(ICharArrayPool bufferPool, char[] buffer)
        {
            if (bufferPool != null)
            {
                bufferPool.Returns(buffer);
            }
        }
    }
}
