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

namespace System.Data.OData
{
    /// <summary>
    /// Class representing a resource collection in a workspace of a data service.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Temporary name only.")]
#if INTERNAL_DROP
    internal sealed class ODataResourceCollectionInfo : ODataAnnotatable
#else
    public sealed class ODataResourceCollectionInfo : ODataAnnotatable
#endif
    {
        /// <summary>
        /// The name of the collection; this is the entity set name in JSON and the Href in ATOM
        /// </summary>
        public string Name
        {
            get;
            set;
        }
    }
}
