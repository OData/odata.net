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

namespace System.Data.OData.Atom
{
    #region Namespaces.
    using System.Collections.Generic;
    #endregion Namespaces.

    /// <summary>
    /// Atom metadata description of a content generator.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class AtomGeneratorMetadata
#else
    public sealed class AtomGeneratorMetadata
#endif
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AtomGeneratorMetadata()
        {
        }

        /// <summary>
        /// The human readable name of the generator of the content.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// The (optional) IRI describing the generator of the content.
        /// </summary>
        public Uri Uri
        {
            get;
            set;
        }

        /// <summary>
        /// The (optional) version of the generator.
        /// </summary>
        public string Version
        {
            get;
            set;
        }
    }
}
