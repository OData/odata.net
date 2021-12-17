//---------------------------------------------------------------------
// <copyright file="CommandLineOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace ResultsComparer.Core
{
    public interface IResultsComparerProvider
    {
        IResultsComparer GetForFile(string filePath);
        IResultsComparer GetById(string comparerId);
    }
}
