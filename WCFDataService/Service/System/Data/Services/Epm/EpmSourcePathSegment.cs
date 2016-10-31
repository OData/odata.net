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
