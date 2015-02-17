//---------------------------------------------------------------------
// <copyright file="IClientDataContextFormatApplier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for generating client code layers.
    /// </summary>
    [ImplementationSelector("ClientDataContextFormatApplier", DefaultImplementation = "Default")]
    public interface IClientDataContextFormatApplier
    {
        /// <summary>
        /// Determines whether [is using content type] [the specified content type].
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>
        ///   <c>true</c> if [is using content type] [the specified content type]; otherwise, <c>false</c>.
        /// </returns>
        bool IsUsingContentType(string contentType);
        
        /// <summary>
        /// Applies the specified data service client format.
        /// </summary>
        /// <param name="context">The data service client context to apply format to.</param>
        void Apply(DataServiceContext context);
    }
}
