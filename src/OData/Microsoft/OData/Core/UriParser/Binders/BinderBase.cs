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

namespace Microsoft.OData.Core.UriParser.Binders
{
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Parsers;

    /// <summary>
    /// Base class for binders
    /// </summary>
    internal abstract class BinderBase
    {
        /// <summary>
        /// Method to use for binding the parent node, if needed.
        /// </summary>
        protected MetadataBinder.QueryTokenVisitor bindMethod;

        /// <summary>
        /// State of metadata binding.
        /// </summary>
        protected BindingState state;

        /// <summary>
        /// Constructor for binderbase.
        /// </summary>
        /// <param name="bindMethod">Method to use for binding the parent token, if needed.</param>
        /// <param name="state">State of the metadata binding.</param>
        protected BinderBase(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
        {
            ExceptionUtils.CheckArgumentNotNull(bindMethod, "bindMethod");
            ExceptionUtils.CheckArgumentNotNull(state, "state");
            this.bindMethod = bindMethod;
            this.state = state;
        }

        /// <summary>
        /// Resolver for uri parser.
        /// </summary>
        protected ODataUriResolver Resolver
        {
            get { return state.Configuration.Resolver; }
        }
    }
}
