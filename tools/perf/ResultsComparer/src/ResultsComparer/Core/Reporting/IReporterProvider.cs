//---------------------------------------------------------------------
// <copyright file="IReporterProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace ResultsComparer.Core.Reporting
{
    /// <summary>
    /// Retrieves registered <see cref="IReporter"/>s.
    /// </summary>
    public interface IReporterProvider
    {
        /// <summary>
        /// Gets the default <see cref="IReporter"/> used
        /// to display comparison reports.
        /// </summary>
        /// <returns>The default reporter.</returns>
        IReporter GetDefault();
    }
}
