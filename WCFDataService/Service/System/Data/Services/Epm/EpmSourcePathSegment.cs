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

#if ASTORIA_SERVER
namespace System.Data.Services.Serializers
#else
namespace System.Data.Services.Client.Serializers
#endif
{
    using System.Collections.Generic;

    /// <summary>
    /// Representation of each node in the <see cref="EpmSourceTree"/>
    /// </summary>
    internal class EpmSourcePathSegment
    {
        #region Fields

        /// <summary>Name of the property under the parent resource type</summary>
        /// <remarks>This fields is used to diferentiate between some special node types as well.
        /// - null - this is the root node of the source tree.
        /// - "" (empty string) - this is a node representing a value of a primitive item in a collection property.
        /// - anything else - this is a node representing a property with the name of this field.</remarks>
        private readonly String propertyName;

        /// <summary>List of sub-properties if this segment corresponds to a complex type</summary>
        private readonly List<EpmSourcePathSegment> subProperties;

        #endregion

        /// <summary>
        /// Constructor creates a root source path segment
        /// </summary>
        internal EpmSourcePathSegment()
        {
            this.propertyName = null;
            this.subProperties = new List<EpmSourcePathSegment>();
        }

        /// <summary>
        /// Constructor creates a source path segment with the name set to <paramref name="propertyName"/>
        /// </summary>
        /// <param name="propertyName">Segment property name</param>
        internal EpmSourcePathSegment(String propertyName)
        {
            this.propertyName = propertyName;
            this.subProperties = new List<EpmSourcePathSegment>();
        }

        #region Properties

        /// <summary>Name of the property under the parent resource type</summary>
        /// <remarks>This property is used to diferentiate between some special node types as well.
        /// - null - this is the root node of the source tree.
        /// - "" (empty string) - this is a node representing a value of a primitive item in a collection property.
        /// - anything else - this is a node representing a property with the name of this property.
        /// These values should not be compared directly, instead use the IsCollectionItemValue property to differentiate between the last two.
        /// The root node itself should never be accessed directly so far.</remarks>
        internal String PropertyName
        {
            get
            {
                return this.propertyName;
            }
        }

        /// <summary>List of sub-properties if this segment corresponds to a complex type</summary>
        internal List<EpmSourcePathSegment> SubProperties
        {
            get
            {
                return this.subProperties;
            }
        }

        /// <summary>Corresponding EntityPropertyMappingInfo</summary>
        internal EntityPropertyMappingInfo EpmInfo
        {
            get;
            set;
        }

        #endregion
    }
}
