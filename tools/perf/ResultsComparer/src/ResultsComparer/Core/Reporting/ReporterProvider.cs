//---------------------------------------------------------------------
// <copyright file="ReporterProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace ResultsComparer.Core.Reporting
{
    /// <summary>
    /// The default <see cref="IReporterProvider"/>.
    /// </summary>
    public class ReporterProvider : IReporterProvider
    {
        private readonly IReporter _default = new MarkdownReporter();

        /// <inheritdoc/>
        public IReporter GetDefault()
        {
            return _default;
        }
    }
}
