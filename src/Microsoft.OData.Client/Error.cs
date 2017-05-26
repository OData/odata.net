//---------------------------------------------------------------------
// <copyright file="Error.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>
    ///    Strongly-typed and parameterized exception factory.
    /// </summary>
    internal static partial class Error
    {
        /// <summary>
        /// create and trace new ArgumentException
        /// </summary>
        /// <param name="message">exception message</param>
        /// <param name="parameterName">parameter name in error</param>
        /// <returns>ArgumentException</returns>
        internal static ArgumentException Argument(string message, string parameterName)
        {
            return Trace(new ArgumentException(message, parameterName));
        }

        /// <summary>
        /// create and trace new InvalidOperationException
        /// </summary>
        /// <param name="message">exception message</param>
        /// <returns>InvalidOperationException</returns>
        internal static InvalidOperationException InvalidOperation(string message)
        {
            return Trace(new InvalidOperationException(message));
        }

        /// <summary>
        /// create and trace new InvalidOperationException
        /// </summary>
        /// <param name="message">exception message</param>
        /// <param name="innerException">innerException</param>
        /// <returns>InvalidOperationException</returns>
        internal static InvalidOperationException InvalidOperation(string message, Exception innerException)
        {
            return Trace(new InvalidOperationException(message, innerException));
        }

        /// <summary>
        /// Create and trace a NotSupportedException with a message
        /// </summary>
        /// <param name="message">Message for the exception</param>
        /// <returns>NotSupportedException</returns>
        internal static NotSupportedException NotSupported(string message)
        {
            return Trace(new NotSupportedException(message));
        }

        /// <summary>
        /// create and throw a ThrowObjectDisposed with a type name
        /// </summary>
        /// <param name="type">type being thrown on</param>
        internal static void ThrowObjectDisposed(Type type)
        {
            throw Trace(new ObjectDisposedException(type.ToString()));
        }

        /// <summary>
        /// create and trace a
        /// </summary>
        /// <param name="errorCode">errorCode</param>
        /// <param name="message">message</param>
        /// <returns>InvalidOperationException</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801", Justification = "errorCode ignored for code sharing")]
        internal static InvalidOperationException HttpHeaderFailure(int errorCode, string message)
        {
            return Trace(new InvalidOperationException(message));
        }

        /// <summary>method not supported</summary>
        /// <param name="m">method</param>
        /// <returns>exception to throw</returns>
        internal static NotSupportedException MethodNotSupported(System.Linq.Expressions.MethodCallExpression m)
        {
            return Error.NotSupported(Strings.ALinq_MethodNotSupported(m.Method.Name));
        }

        /// <summary>throw an exception because unexpected batch content was encounted</summary>
        /// <param name="value">internal error</param>
        internal static void ThrowBatchUnexpectedContent(InternalError value)
        {
            throw InvalidOperation(Strings.Batch_UnexpectedContent((int)value));
        }

        /// <summary>throw an exception because expected batch content was not encountered</summary>
        /// <param name="value">internal error</param>
        internal static void ThrowBatchExpectedResponse(InternalError value)
        {
            throw InvalidOperation(Strings.Batch_ExpectedResponse((int)value));
        }

        /// <summary>unexpected xml when reading web responses</summary>
        /// <param name="value">internal error</param>
        /// <returns>exception to throw</returns>
        internal static InvalidOperationException InternalError(InternalError value)
        {
            return InvalidOperation(Strings.Context_InternalError((int)value));
        }

        /// <summary>throw exception for unexpected xml when reading web responses</summary>
        /// <param name="value">internal error</param>
        internal static void ThrowInternalError(InternalError value)
        {
            throw InternalError(value);
        }

        /// <summary>
        /// Trace the exception
        /// </summary>
        /// <typeparam name="T">type of the exception</typeparam>
        /// <param name="exception">exception object to trace</param>
        /// <returns>the exception parameter</returns>
        private static T Trace<T>(T exception) where T : Exception
        {
            return exception;
        }
    }

    /// <summary>unique numbers for repeated error messages for unlikely, unactionable exceptions</summary>
    internal enum InternalError
    {
        UnexpectedReadState = 4,
        UnvalidatedEntityState = 6,
        NullResponseStream = 7,
        EntityNotDeleted = 8,
        EntityNotAddedState = 9,
        LinkNotAddedState = 10,
        EntryNotModified = 11,
        LinkBadState = 12,
        UnexpectedBeginChangeSet = 13,
        UnexpectedBatchState = 14,
        ChangeResponseMissingContentID = 15,
        ChangeResponseUnknownContentID = 16,
        InvalidHandleOperationResponse = 18,

        InvalidEndGetRequestStream = 20,
        InvalidEndGetRequestCompleted = 21,
        InvalidEndGetRequestStreamRequest = 22,
        InvalidEndGetRequestStreamStream = 23,
        InvalidEndGetRequestStreamContent = 24,
        InvalidEndGetRequestStreamContentLength = 25,

        InvalidEndWrite = 30,
        InvalidEndWriteCompleted = 31,
        InvalidEndWriteRequest = 32,
        InvalidEndWriteStream = 33,

        InvalidEndGetResponse = 40,
        InvalidEndGetResponseCompleted = 41,
        InvalidEndGetResponseRequest = 42,
        InvalidEndGetResponseResponse = 43,
        InvalidAsyncResponseStreamCopy = 44,
        InvalidAsyncResponseStreamCopyBuffer = 45,

        InvalidEndRead = 50,
        InvalidEndReadCompleted = 51,
        InvalidEndReadStream = 52,
        InvalidEndReadCopy = 53,
        InvalidEndReadBuffer = 54,

        InvalidSaveNextChange = 60,
        InvalidBeginNextChange = 61,
        SaveNextChangeIncomplete = 62,
        MaterializerReturningMoreThanOneEntity = 63,

        InvalidGetResponse = 71,
        InvalidHandleCompleted = 72,

        InvalidMethodCallWhenNotReadingJsonLight = 73,
    }
}
