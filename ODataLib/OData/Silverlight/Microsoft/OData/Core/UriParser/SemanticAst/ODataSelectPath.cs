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
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// A specific type of <see cref="ODataPath"/> which can only contain instances of <see cref="TypeSegment"/>, <see cref="NavigationPropertySegment"/>,
    /// <see cref="PropertySegment"/>, <see cref="OperationSegment"/>, or <see cref="OpenPropertySegment"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "ODataSelectPathCollection just doesn't sound right")]
    public class ODataSelectPath : ODataPath
    {
        /// <summary>
        /// Create an ODataSelectPath
        /// </summary>
        /// <param name="segments">The list of segments that makes up this path.</param>
        /// <exception cref="ODataException">Throws if the list of segments doesn't match the requirements for a path in $select</exception>
        public ODataSelectPath(IEnumerable<ODataPathSegment> segments)
            : base(segments)
        {
            this.ValidatePath();
        }

        /// <summary>
        /// Create an ODataPath object based on a single segment
        /// </summary>
        /// <param name="segments">The list of segments that makes up this path.</param>
        /// <exception cref="ODataException">Throws if the list of segments doesn't match the requirements for a path in $select</exception>
        public ODataSelectPath(params ODataPathSegment[] segments)
            : base(segments)
        {
            this.ValidatePath();
        }

        /// <summary>
        /// Ensure that the segments given to us are valid select segments.
        /// </summary>
        /// <exception cref="ODataException">Throws if the list of segments doesn't match the requirements for a path in $select</exception>
        private void ValidatePath()
        {
            int index = 0;
            foreach (ODataPathSegment segment in this)
            {
                if (segment is NavigationPropertySegment)
                {
                    if (index != this.Count - 1)
                    {
                        throw new ODataException(ODataErrorStrings.ODataSelectPath_NavPropSegmentCanOnlyBeLastSegment);
                    }
                }
                else if (segment is OperationSegment)
                {
                    if (index != this.Count - 1)
                    {
                        throw new ODataException(ODataErrorStrings.ODataSelectPath_OperationSegmentCanOnlyBeLastSegment);
                    }
                }
                else if (segment is TypeSegment)
                {
                    if (index == this.Count - 1)
                    {
                        throw new ODataException(ODataErrorStrings.ODataSelectPath_CannotEndInTypeSegment);
                    }
                }
                else if (segment is OpenPropertySegment || segment is PropertySegment)
                {
                    continue;
                }
                else
                {
                    throw new ODataException(ODataErrorStrings.ODataSelectPath_InvalidSelectPathSegmentType(segment.GetType().Name));
                }

                index++;
            }
        }
    }
}
