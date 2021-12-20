//---------------------------------------------------------------------
// <copyright file="BdnComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace ResultsComparer.Core.Reporting
{
    public interface IReporterProvider
    {
        IReporter GetDefault();
    }
}
