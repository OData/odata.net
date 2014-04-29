//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Visitors
{
    using System;
    using Microsoft.OData.Core.UriParser.Semantic;

    /// <summary>
    /// Handler interface for walking a select item tree.
    /// </summary>
    public abstract class SelectItemHandler
    {
        /// <summary>
        /// Handle a WildcardSelectItem
        /// </summary>
        /// <param name="item">the item to Handle</param>
        public virtual void Handle(WildcardSelectItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle a PathSelectItem
        /// </summary>
        /// <param name="item">the item to Handle</param>
        public virtual void Handle(PathSelectItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle a ContainerQualifiedWildcardSelectItem
        /// </summary>
        /// <param name="item">the item to Handle</param>
        public virtual void Handle(NamespaceQualifiedWildcardSelectItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle an ExpandedNavigationSelectItem
        /// </summary>
        /// <param name="item">the item to Handle</param>
        public virtual void Handle(ExpandedNavigationSelectItem item)
        {
            throw new NotImplementedException();
        }
    }
}
