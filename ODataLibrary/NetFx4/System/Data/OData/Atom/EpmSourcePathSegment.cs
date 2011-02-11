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
    /// Representation of each node in the EpmSourceTree.
    /// </summary>
    internal class EpmSourcePathSegment
    {
        #region Fields
        /// <summary>
        /// Name of the property under the parent resource type.
        /// </summary>
        /// <remarks>This fields is used to differentiate between some special node types as well.
        /// - null - this is the root node of the source tree.
        /// - "" (empty string) - this is a node representing a value of a primitive item in a MultiValue property.
        /// - anything else - this is a node representing a property with the name of this field.</remarks>
        private String propertyName;

        /// <summary>
        /// List of sub-properties if this segment corresponds to a complex type.
        /// </summary>
        private List<EpmSourcePathSegment> subProperties;

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
        /// <param name="propertyName">Segment property name</param>
        internal EpmSourcePathSegment(String propertyName)
        {
            DebugUtils.CheckNoExternalCallers();

            this.propertyName = propertyName;
            this.subProperties = new List<EpmSourcePathSegment>();
        }

        #region Properties
        /// <summary>
        /// Name of the property under the parent resource type.
        /// </summary>
        /// <remarks>This property is used to diferentiate between some special node types as well.
        /// - null - this is the root node of the source tree.
        /// - "" (empty string) - this is a node representing a value of a primitive item in a MultiValue property.
        /// - anything else - this is a node representing a property with the name of this property.
        /// These values should not be compared directly, instead use the IsMultiValueItemValue property to differentiate between the last two.
        /// The root node itself should never be accessed directly so far.</remarks>
        internal String PropertyName
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

        /// <summary>
        /// true if this segment represents the direct value of a MultiValue item (a special node).
        /// </summary>
        internal bool IsMultiValueItemValue
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.propertyName != null && this.propertyName.Length == 0;
            }
        }
        #endregion

        /// <summary>
        /// Creates a new special segment which represents the value of a MultiValue item.
        /// </summary>
        /// <returns>The newly creates source path segment</returns>
        internal static EpmSourcePathSegment CreateMultiValueItemValueSegment()
        {
            DebugUtils.CheckNoExternalCallers();
            return new EpmSourcePathSegment(string.Empty);
        }
    }
}
