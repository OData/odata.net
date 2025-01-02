namespace Microsoft.OData.TestCommon
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;

    /// <summary>
    /// A <see cref="TraceListener"/> that handles failures in <see cref="Debug.Assert(bool)"/> and its overloads by throwing an exception rather than by displaying a UI prompt
    /// </summary>
    public sealed class DebugAssertTraceListener : TraceListener
    {
        /// <inheritdoc/>
        public override void Write(string message)
        {
        }

        /// <inheritdoc/>
        public override void WriteLine(string message)
        {
        }

        /// <inheritdoc/>
        public override void Fail(string message)
        {
            throw new DebugAssertException(message);
        }

        /// <inheritdoc/>
        public override void Fail(string message, string detailMessage)
        {
            Fail($"Message: {message}. Details: {detailMessage}.");
        }

        /// <summary>
        /// A private, nested exception type that will be thrown when failures occur in <see cref="Debug.Assert(bool)"/> and its overloads. It is nested and private so that
        /// it cannot be used in <see langword="catch"/> blocks, preventing debug assertions from being used (intentionally or unintentionally) for control flow logic
        /// </summary>
        private sealed class DebugAssertException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DebugAssertException"/> class.
            /// </summary>
            public DebugAssertException()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DebugAssertException"/> class.
            /// </summary>
            /// <param name="message">The message that describes the error</param>
            public DebugAssertException(string message) : base(message)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DebugAssertException"/> class.
            /// </summary>
            /// <param name="message">The error message that explains the reason for the exception</param>
            /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified</param>
            public DebugAssertException(string message, Exception innerException) : base(message, innerException)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DebugAssertException"/> class.
            /// </summary>
            /// <param name="info">
            /// The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown
            /// </param>
            /// <param name="context">
            /// The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination
            /// </param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="info"/> is <see langword="null"/></exception>
            /// <exception cref="System.Runtime.Serialization.SerializationException">
            /// Thrown if class name is <see langword="null"/> or <see cref="System.Exception.HResult"/> is zero
            /// </exception>
            public DebugAssertException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}
