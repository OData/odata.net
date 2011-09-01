//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Metadata
{
    /// <summary>
    /// Enumeration to represent criteria for conditional syndication mapping
    /// </summary>
    internal enum EpmSyndicationCriteria
    {
        /// <summary>
        /// No criteria
        /// </summary>
        None,

        /// <summary>
        /// category/@scheme
        /// </summary>
        CategoryScheme,

        /// <summary>
        /// link/@rel
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "LinkRel is not Hungarian notation")]
        LinkRel
    }
}
