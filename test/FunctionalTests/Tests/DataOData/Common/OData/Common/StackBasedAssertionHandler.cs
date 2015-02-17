//---------------------------------------------------------------------
// <copyright file="StackBasedAssertionHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    #endregion

    /// <summary>
    /// Assertion handler implementation that allows a stack of messages to be set up for more useful errors when comparing trees
    /// </summary>
    /// <remarks>This is a copy from the Taupo.Astoria ODataPayloadElementComparer</remarks>
    internal class StackBasedAssertionHandler : AssertionHandler
    {
        //// TODO: Ideally this would allow for wrapping another assertion handler, so that we don't just rely on exception.

        /// <summary>
        /// Stack of messages to report.
        /// </summary>
        private Stack<string> messageStack = new Stack<string>();

        /// <summary>
        /// Adds a new message to the assertion handler's stack
        /// </summary>
        /// <param name="message">The message to add</param>
        /// <param name="args">The arguments to the message</param>
        /// <returns>A disposable object that will remove the message when disposed</returns>
        public IDisposable WithMessage(string message, params object[] args)
        {
            this.messageStack.Push(args.Length == 0 ? message : string.Format(CultureInfo.InvariantCulture, message, args));
            return new DelegateBasedDisposable(() => this.messageStack.Pop());
        }

        /// <summary>
        /// Invoked whenever there's an assertion failure.
        /// </summary>
        /// <param name="text">Assertion failure text.</param>
        protected override void OnAssertionFailure(string text)
        {
            using (this.WithMessage(text))
            {
                throw new AssertionFailedException(this.GetErrorMessage());
            }
        }

        /// <summary>
        /// Invoked whenever there's data comparison failure.
        /// </summary>
        /// <param name="expected">Expected value</param>
        /// <param name="actual">Actual value</param>
        /// <param name="text">Error text</param>
        protected override void OnDataComparisonFailure(object expected, object actual, string text)
        {
            using (this.WithMessage(text))
            {
                throw new DataComparisonException(this.GetErrorMessage())
                {
                    ExpectedValue = expected,
                    ActualValue = actual,
                };
            }
        }

        /// <summary>
        /// Invoked whenever <see cref="Skip"/> method is called.
        /// </summary>
        /// <param name="text">Skip message</param>
        protected override void OnSkipped(string text)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Invoked whenever <see cref="Warn"/> method is called.
        /// </summary>
        /// <param name="text">Warning text.</param>
        protected override void OnWarning(string text)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Returns the textual representation of all the messages on the stack.
        /// </summary>
        /// <returns>The cummulative error message.</returns>
        private string GetErrorMessage()
        {
            return string.Join(Environment.NewLine, this.messageStack.Reverse());
        }
    }
}
