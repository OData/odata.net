//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
