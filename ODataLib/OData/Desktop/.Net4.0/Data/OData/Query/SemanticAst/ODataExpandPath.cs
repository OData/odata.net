//   OData .NET Libraries ver. 5.6.3
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
