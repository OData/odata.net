//---------------------------------------------------------------------
// <copyright file="InstanceAnnotationWriteTracker.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Helper class to track if an annotation has been written.
    /// </summary>
    internal sealed class InstanceAnnotationWriteTracker
    {
        /// <summary>
        /// Maintains the write status for each annotation using its key.
        /// If a key exists in the list then it is considered written.
        /// </summary>
        private readonly Lazy<HashSet<string>> writeStatus = new Lazy<HashSet<string>>(() => new HashSet<string>(StringComparer.Ordinal));

        /// <summary>
        /// Check if an annotation is already written.
        /// </summary>
        /// <returns>true if the annotation is written; otherwise false.</returns>
        /// <param name="key">The key of the element to check if its written.</param>
        public bool IsAnnotationWritten(string key)
        {
            return this.writeStatus.Value.Contains(key);
        }

        /// <summary>
        /// Mark an annotation as written.
        /// </summary>
        /// <returns>true if the annotation was unmarked before; otherwise false.</returns>
        /// <param name="key">The key of the element to mark as written.</param>
        public bool MarkAnnotationWritten(string key)
        {
            return this.writeStatus.Value.Add(key);
        }
    }
}
