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
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// A segment representing an entity id represented by $id query option
    /// The query option $id would always produce a result that is an EntitySetSegment followed by the KeySegment
    /// </summary>
    public sealed class EntityIdSegment
    {
        /// <summary>
        /// The path segment representing the entity set segment
        /// </summary>
        private ODataPathSegment odataPathSegment;

        /// <summary>
        /// The key segment
        /// </summary>
        private KeySegment keySegment;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityIdSegment"/> class.
        /// </summary>
        /// <param name="pathSegment">ODataPathSegment representing the entity set segment.</param>
        /// <param name="keySegment">The key segment.</param>
        /// <exception cref="ODataException">If ODataPath is not an EntitySetPath</exception>
        /// <exception cref="ODataException">If the KeySegment provided is either null, having no keys, or does not target a single resource.</exception>
        public EntityIdSegment(ODataPathSegment pathSegment, KeySegment keySegment)
        {
            EntitySetSegment firstSegment = pathSegment as EntitySetSegment;

            // We expect to have an EntityIdSegment as EntitySetSetment followed by KeySegment
            // This assumption might change later (in case of containment or in case $id is an opaque value.
            if (firstSegment == null)
            {
                throw new ODataException(ODataErrorStrings.RequestUriProcessor_InvalidValueForEntitySegment(pathSegment.Identifier));
            }

            // The KeySegment must be targeting a single resource
            if (keySegment == null 
                || !keySegment.Keys.Any() 
                || keySegment.TargetKind != RequestTargetKind.Resource 
                || !keySegment.SingleResult)
            {
                throw new ODataException(ODataErrorStrings.RequestUriProcessor_InvalidValueForKeySegment(keySegment != null ? keySegment.Identifier : string.Empty));
            }

            this.odataPathSegment = firstSegment;
            this.keySegment = keySegment;
        }

        /// <summary>
        /// Gets the PathSegement representing entity set segment.
        /// </summary>
        public ODataPathSegment PathSegment
        {
            get
            {
                return this.odataPathSegment;
            }
        }

        /// <summary>
        /// Gets the KeySegment that follows EntitySetSegment
        /// </summary>
        public KeySegment KeySegment
        {
            get
            {
                return this.keySegment;
            }
        }
    }
}
