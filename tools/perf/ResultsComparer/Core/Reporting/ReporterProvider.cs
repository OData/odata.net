//---------------------------------------------------------------------
// <copyright file="BdnComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace ResultsComparer.Core.Reporting
{
    public class ReporterProvider : IReporterProvider
    {
        private readonly IReporter _default = new MarkdownReporter();

        public IReporter GetDefault()
        {
            return _default;
        }
    }
}
