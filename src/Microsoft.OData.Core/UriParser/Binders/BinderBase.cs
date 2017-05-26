//---------------------------------------------------------------------
// <copyright file="BinderBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
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
