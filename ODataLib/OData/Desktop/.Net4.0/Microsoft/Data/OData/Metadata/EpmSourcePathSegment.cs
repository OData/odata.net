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
