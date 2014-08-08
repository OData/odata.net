//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.OData.Query.SemanticAst
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.Edm;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// A specific type of <see cref="ODataPath"/> which can only contain instances of <see cref="TypeSegment"/> or <see cref="NavigationPropertySegment"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "ODataExpandPathCollection just doesn't sound right")]
    public class ODataExpandPath : ODataPath
    {
        /// <summary>
        /// Create an ODataPath object to represent a path semantically
        /// </summary>
        /// <param name="segments">The list of segments in the path.</param>
        /// <exception cref="ODataException">Throws if this list of segments doesn't match the requirements for a $expand</exception>
        public ODataExpandPath(IEnumerable<ODataPathSegment> segments)
            : base(segments)
        {
            this.ValidatePath();
        }

        /// <summary>
        /// Create an ODataPath object based on a single segment
        /// </summary>
        /// <param name="segments">A list of segments in the path.</param>
        /// <exception cref="ODataException">Throws if this list of segments doesn't match the requirements for a $expand</exception>
        public ODataExpandPath(params ODataPathSegment[] segments)
            : base(segments)
        {
            this.ValidatePath();
        }

        /// <summary>
        /// Gets the navigation property for this expand path.
        /// </summary>
        /// <returns>the navigation property for this expand path.</returns>
        internal IEdmNavigationProperty GetNavigationProperty()
        {
            DebugUtils.CheckNoExternalCallers();
            return ((NavigationPropertySegment)this.LastSegment).NavigationProperty;
        }

        /// <summary>
        /// Ensure that this expand path contains only valid segment types.
        /// </summary>
        /// <exception cref="ODataException">Throws if this list of segments doesn't match the requirements for a $expand</exception>
        private void ValidatePath()
        {
            int index = 0;
            bool foundNavProp = false;
            foreach (ODataPathSegment segment in this)
            {
                if (segment is TypeSegment) 
                {
                    if (index == this.Count - 1)
                    {
                        throw new ODataException(ODataErrorStrings.ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty);
                    }
                }
                else if (segment is NavigationPropertySegment)
                {
                    if (index < this.Count - 1 || foundNavProp)
                    {
                        throw new ODataException(ODataErrorStrings.ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty);
                    }

                    foundNavProp = true;
                }
                else
                {
                    throw new ODataException(ODataErrorStrings.ODataExpandPath_InvalidExpandPathSegment(segment.GetType().Name));
                }

                index++;
            }
        }
    }
}
