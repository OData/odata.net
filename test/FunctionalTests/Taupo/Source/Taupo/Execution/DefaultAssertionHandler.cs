//---------------------------------------------------------------------
// <copyright file="DefaultAssertionHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Default implementation of <see cref="AssertionHandler"/> which uses common exception types:
    /// <see cref="AssertionFailedException"/>, 
    /// <see cref="DataComparisonException"/>, 
    /// <see cref="TestSkippedException" />
    /// </summary>
    [ImplementationName(typeof(AssertionHandler), "Default")]
    public class DefaultAssertionHandler : AssertionHandler
    {
        private Logger logger;

        /// <summary>
        /// Initializes a new instance of the DefaultAssertionHandler class.
        /// </summary>
        /// <param name="logger">Logger to use.</param>
        public DefaultAssertionHandler(Logger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Throws <see cref="AssertionFailedException"/> with the specified text.
        /// </summary>
        /// <param name="text">Exception text.</param>
        protected override void OnAssertionFailure(string text)
        {
            throw new AssertionFailedException(text);
        }

        /// <summary>
        /// Throws <see cref="DataComparisonException"/> with the specified text, 
        /// <see cref="DataComparisonException.ActualValue"/> and <see cref="DataComparisonException.ExpectedValue"/>
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value</param>
        /// <param name="text">Exception text.</param>
        protected override void OnDataComparisonFailure(object expected, object actual, string text)
        {
            throw new DataComparisonException(text)
            {
                ActualValue = actual,
                ExpectedValue = expected
            };
        }

        /// <summary>
        /// Throws <see cref="TestSkippedException"/> with a given text.
        /// </summary>
        /// <param name="text">Exception text.</param>
        protected override void OnSkipped(string text)
        {
            throw new TestSkippedException(text);
        }

        /// <summary>
        /// Writes the specified warning message to the logger.
        /// </summary>
        /// <param name="text">Warning message.</param>
        protected override void OnWarning(string text)
        {
            this.logger.WriteLine(LogLevel.Warning, text);
        }
    }
}
