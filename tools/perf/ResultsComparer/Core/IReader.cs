using System;
//---------------------------------------------------------------------
// <copyright file="CommandLineOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace ResultsComparer.Core
{
    interface IReader<T> : IDisposable, IEnumerable<T> where T : new()
    {
        bool ReadNext(out T value);
    }
}
