//---------------------------------------------------------------------
// <copyright file="DefaultStackBasedAssertionHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Assertion handler implementation that allows a stack of messages to be set up for more useful errors when comparing trees
    /// </summary>
    [ImplementationName(typeof(StackBasedAssertionHandler), "Default")]
    public class DefaultStackBasedAssertionHandler : StackBasedAssertionHandler
    {
        private Stack<string> messageStack = new Stack<string>();

        /// <summary>
        /// Adds a new message to the assertion handler's stack
        /// </summary>
        /// <param name="message">The message to add</param>
        /// <param name="args">The arguments to the message</param>
        /// <returns>A disposable object that will remove the message when disposed</returns>
        public override IDisposable WithMessage(string message, params object[] args)
        {
            this.messageStack.Push(string.Format(CultureInfo.InvariantCulture, message, args));
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

        private string GetErrorMessage()
        {
            return string.Join(Environment.NewLine, this.messageStack.Reverse());
        }
    }
}