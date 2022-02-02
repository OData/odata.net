//---------------------------------------------------------------------
// <copyright file="IReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace ResultsComparer.Core
{
    /// <summary>
    /// Represents a sequential forward reader for objects of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of values returned by the reader.</typeparam>
    interface IReader<T> : IDisposable, IEnumerable<T> where T : new()
    {
        /// <summary>
        /// Read the next object and return true if successful.
        /// </summary>
        /// <param name="value">The object that was read.</param>
        /// <returns>True if the reading was successful, false if the end of data source was reached.</returns>
        bool ReadNext(out T value);
    }
}
