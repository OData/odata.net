//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
