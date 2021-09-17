//---------------------------------------------------------------------
// <copyright file="ODataExpandPath.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// A specific type of <see cref="ODataPath"/> which can only contain instances of <see cref="TypeSegment"/> or <see cref="NavigationPropertySegment"/> or <see cref="PropertySegment"/> of complex.
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
                if (segment is PropertySegment)
                {
                    if (index == this.Count - 1)
                    {
                        throw new ODataException(ODataErrorStrings.ODataExpandPath_LastSegmentMustBeNavigationPropertyOrTypeSegment);
                    }
                }
                else if (segment is NavigationPropertySegment)
                {
                    if (foundNavProp)
                    {
                        throw new ODataException(ODataErrorStrings.ODataExpandPath_OnlyLastSegmentCanBeNavigationProperty);
                    }

                    foundNavProp = true;
                }
                else if (!(segment is TypeSegment))
                {
                    throw new ODataException(ODataErrorStrings.ODataExpandPath_InvalidExpandPathSegment(segment.GetType().Name));
                }

                index++;
            }
        }
    }
}