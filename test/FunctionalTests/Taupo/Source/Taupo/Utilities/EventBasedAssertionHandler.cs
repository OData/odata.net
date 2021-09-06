//---------------------------------------------------------------------
// <copyright file="EventBasedAssertionHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.IO;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Assertion handler that notifies about failures via events.
    /// </summary>
    /// <remarks>
    /// Used primarily for self-testing.
    /// </remarks>
    public class EventBasedAssertionHandler : AssertionHandler
    {
        /// <summary>
        /// Event to be raise on assertion failure.
        /// </summary>
        public event ErrorEventHandler AssertionFailure;

        /// <summary>
        /// Event to be raised whenever test is skipped.
        /// </summary>
        public event ErrorEventHandler TestSkipped;

        /// <summary>
        /// Event to be raised on data miscompare.
        /// </summary>
        public event ErrorEventHandler DataComparisonFailure;

        /// <summary>
        /// Event to be raised on test warning.
        /// </summary>
        public event ErrorEventHandler TestWarning;

        /// <summary>
        /// Raises <see cref="AssertionFailure"/> event.
        /// </summary>
        /// <param name="text">Exception message text to be passed in <see cref="ErrorEventArgs"/> exception.</param>
        protected override void OnAssertionFailure(string text)
        {
            if (this.AssertionFailure != null)
            {
                this.AssertionFailure(this, new ErrorEventArgs(new AssertionFailedException(text)));
            }
        }

        /// <summary>
        /// Raises <see cref="DataComparisonFailure"/> event.
        /// </summary>
        /// <param name="expected">Expected value</param>
        /// <param name="actual">Actual value</param>
        /// <param name="text">Exception message text to be passed in <see cref="ErrorEventArgs"/> exception.</param>
        protected override void OnDataComparisonFailure(object expected, object actual, string text)
        {
            if (this.DataComparisonFailure != null)
            {
                this.DataComparisonFailure(this, new ErrorEventArgs(new DataComparisonException(text) { ActualValue = actual, ExpectedValue = expected }));
            }
        }

        /// <summary>
        /// Raises <see cref="TestSkipped"/> event.
        /// </summary>
        /// <param name="text">Exception message text to be passed in <see cref="ErrorEventArgs"/> exception.</param>
        protected override void OnSkipped(string text)
        {
            if (this.TestSkipped != null)
            {
                this.TestSkipped(this, new ErrorEventArgs(new TestSkippedException(text)));
            }
        }

        /// <summary>
        /// Raises <see cref="TestWarning"/> event.
        /// </summary>
        /// <param name="text">Exception message text to be passed in <see cref="ErrorEventArgs"/> exception.</param>
        protected override void OnWarning(string text)
        {
            if (this.TestWarning != null)
            {
                this.TestWarning(this, new ErrorEventArgs(new TaupoInvalidOperationException(text)));
            }
        }
    }
}
