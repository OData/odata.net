//---------------------------------------------------------------------
// <copyright file="BdnComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;

namespace ResultsComparer.Core.Reporting
{
    public interface IReporter
    {
        void GenerateReport(ComparerResults results, Stream destination, ComparerOptions options);
    }
}
