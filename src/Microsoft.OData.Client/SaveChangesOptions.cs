//---------------------------------------------------------------------
// <copyright file="SaveChangesOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>
    /// options when saving changes
    /// </summary>
    [Flags]
    public enum SaveChangesOptions
    {
        /// <summary>default option, using multiple requests to the server stopping on the first failure</summary>
        None = 0,

        /// <summary>save the changes in a single changeset in a batch request.</summary>
        BatchWithSingleChangeset = 1,

        /// <summary>save all the changes using multiple requests</summary>
        ContinueOnError = 2,

        /// <summary>Use replace semantics when doing update.</summary>
        ReplaceOnUpdate = 4,

        /// <summary>save each change independently in a batch request.</summary>
        BatchWithIndependentOperations = 16,

        /// <summary>
        /// Use partial payload when doing post.
        /// Note it can only be used when using <see cref="T:Microsoft.OData.Client.DataServiceCollection`1" />
        /// </summary>
        PostOnlySetProperties = 8,

        /// <summary>
        /// Allow usage of Relative Uri.
        /// Note it can only be used in a batch request.
        /// </summary>
        UseRelativeUri = 32
    }
}
