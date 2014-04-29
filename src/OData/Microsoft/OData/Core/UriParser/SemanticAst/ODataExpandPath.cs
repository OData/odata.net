//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

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
