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

namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// Representation of each node in the EpmSourceTree.
    /// </summary>
    internal sealed class EpmSourcePathSegment
    {
        #region Fields
        /// <summary>
        /// Name of the property under the parent type.
        /// </summary>
        /// <remarks>This fields is used to differentiate between some special node types as well.
        /// - null - this is the root node of the source tree.
        /// - "" (empty string) - this is a node representing a value of a primitive item in a collection property.
        /// - anything else - this is a node representing a property with the name of this field.</remarks>
        private readonly string propertyName;

        /// <summary>
        /// List of sub-properties if this segment corresponds to a complex type.
        /// </summary>
        private readonly List<EpmSourcePathSegment> subProperties;

        /// <summary>
        /// Corresponding EntityPropertyMappingInfo.
        /// </summary>
        private EntityPropertyMappingInfo epmInfo;
        #endregion

        /// <summary>
        /// Constructor creates a root source path segment
        /// </summary>
        internal EpmSourcePathSegment()
            : this(null)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Constructor creates a source path segment with the name set to <paramref name="propertyName"/>
        /// </summary>
        /// <param name="propertyName">StartPath property name</param>
        internal EpmSourcePathSegment(string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();

            this.propertyName = propertyName;
            this.subProperties = new List<EpmSourcePathSegment>();
        }

        #region Properties
        /// <summary>
        /// Name of the property under the parent type.
        /// </summary>
        /// <remarks>This property is used to differentiate between some special node types as well.
        /// - null - this is the root node of the source tree.
        /// - "" (empty string) - this is a node representing a value of a primitive item in a collection property.
        /// - anything else - this is a node representing a property with the name of this property.
        /// These values should not be compared directly, instead use the IsCollectionValueItemValue property to differentiate between the last two.
        /// The root node itself should never be accessed directly so far.</remarks>
        internal string PropertyName
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.propertyName;
            }
        }

        /// <summary>
        /// List of sub-properties if this segment corresponds to a complex type.
        /// </summary>
        internal List<EpmSourcePathSegment> SubProperties
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.subProperties;
            }
        }

        /// <summary>
        /// Corresponding EntityPropertyMappingInfo.
        /// </summary>
        internal EntityPropertyMappingInfo EpmInfo
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.epmInfo;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.epmInfo = value;
            }
        }

        #endregion
    }
}
