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
    using System.Diagnostics;

    /// <summary>
    /// Representation of each node in the <see cref="EpmTargetTree"/>
    /// </summary>
    [DebuggerDisplay("EpmTargetPathSegment {SegmentName} HasContent={HasContent}")]
    internal class EpmTargetPathSegment
    {
        #region Private fields

        /// <summary>Name of the xml element/attribute</summary>
        /// <remarks>This field is used to differentiate between special nodes as well.
        /// - null - this is the root of the target tree.
        /// - anything else (doesn't start with @) - this node represents an element with the specified name.
        /// - anything else (starts with @) - this node represents an attribute with the specified name.</remarks>
        private readonly String segmentName;

        /// <summary>URI of the namespace to which the <see cref="segmentName"/> belongs</summary>
        private readonly String segmentNamespaceUri;

        /// <summary>If this is a non-leaf element, the child elements/attributes collection</summary>
        private readonly List<EpmTargetPathSegment> subSegments;

        /// <summary>Parent element of this element/attribute</summary>
        private readonly EpmTargetPathSegment parentSegment;

        #endregion Private fields

        /// <summary>
        /// Constructor initializes the list of sub-nodes to be empty, used for creating root nodes
        /// in the <see cref="EpmTargetTree"/>
        /// </summary>
        internal EpmTargetPathSegment()
        {
            this.subSegments = new List<EpmTargetPathSegment>();
        }

        /// <summary>Used for creating non-root nodes in the syndication/custom trees</summary>
        /// <param name="segmentName">Name of xml element/attribute</param>
        /// <param name="segmentNamespaceUri">URI of the namespace for <paramref name="segmentName"/></param>
        /// <param name="parentSegment">Reference to the parent node if this is a sub-node, useful for traversals in visitors</param>
        internal EpmTargetPathSegment(String segmentName, String segmentNamespaceUri, EpmTargetPathSegment parentSegment)
            : this()
        {
            Debug.Assert(segmentName == null || segmentName.Length > 0, "Empty segment name is not allowed.");

            this.segmentName = segmentName;
            this.segmentNamespaceUri = segmentNamespaceUri;
            this.parentSegment = parentSegment;
        }

        /// <summary>Name of the xml element/attribute</summary>
        /// <remarks>This property is used to differentiate between special nodes as well.
        /// - null - this is the root of the target tree.
        /// - anything else (doesn't start with @) - this node represents an element with the specified name.
        /// - anything else (starts with @) - this node represents an attribute with the specified name.
        /// The value of the proeprty should not be compared directly to differentiate between these cases, instead
        /// properties IsAttribute and IsElementContentSegment should be used. The root not should not be accessed directly
        /// from anywhere so far.</remarks>
        internal String SegmentName
        {
            get
            {
                return this.segmentName;
            }
        }

#if !ASTORIA_CLIENT
        /// <summary>
        /// Retruns name of the attribute the property is mapped to. Must not be called if a property is mapped to an element.
        /// </summary>
        internal String AttributeName
        {
            get
            {
                Debug.Assert(this.IsAttribute, "This property should be called only for attributes"); 
                return this.SegmentName.Substring(1);
            }
        }
#endif

        /// <summary>URI of the namespace to which the <see cref="segmentName"/> belongs</summary>
        internal String SegmentNamespaceUri
        {
            get
            {
                return this.segmentNamespaceUri;
            }
        }

        /// <summary>EntityPropertyMappingInfo corresponding to current segement</summary>
        internal EntityPropertyMappingInfo EpmInfo
        {
            get;
            set;
        }

        /// <summary>Whether this node corresponds to ResourceType or ClientType property values</summary>
        internal bool HasContent
        {
            get
            {
                return this.EpmInfo != null;
            }
        }

        /// <summary>Does this node correspond to xml attribute</summary>
        internal bool IsAttribute
        {
            get
            {
                return !string.IsNullOrEmpty(this.SegmentName) && this.SegmentName[0] == '@';
            }
        }

        /// <summary>Parent node in the tree (always an element if present)</summary>
        internal EpmTargetPathSegment ParentSegment
        {
            get
            {
                return this.parentSegment;
            }
        }

        /// <summary>Sub-nodes of this node. Only exist if current node is an element node</summary>
        internal List<EpmTargetPathSegment> SubSegments
        {
            get
            {
                return this.subSegments;
            }
        }
    }
}
