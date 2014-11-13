//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Core
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
        private readonly HashSet<string> writeStatus;

        /// <summary>
        /// Creates a new <see cref="InstanceAnnotationWriteTracker"/> to hold write status for instance annotations.
        /// </summary>
        public InstanceAnnotationWriteTracker()
        {
            this.writeStatus = new HashSet<string>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Check if an annotation is already written.
        /// </summary>
        /// <returns>true if the annotation is written; otherwise false.</returns>
        /// <param name="key">The key of the element to check if its written.</param>
        public bool IsAnnotationWritten(string key)
        {
            return this.writeStatus.Contains(key);
        }

        /// <summary>
        /// Mark an annotation as written.
        /// </summary>
        /// <returns>true if the annotation was unmarked before; otherwise false.</returns>
        /// <param name="key">The key of the element to mark as written.</param>
        public bool MarkAnnotationWritten(string key)
        {
            return this.writeStatus.Add(key);
        }
    }
}
