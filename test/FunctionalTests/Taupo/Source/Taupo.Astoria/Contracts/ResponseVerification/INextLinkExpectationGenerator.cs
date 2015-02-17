//---------------------------------------------------------------------
// <copyright file="INextLinkExpectationGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contract for generating expected next links
    /// </summary>
    [ImplementationSelector("NextLinkExpectationGenerator", DefaultImplementation = "QueryBased", HelpText = "")]
    public interface INextLinkExpectationGenerator
    {
        /// <summary>
        /// Generates the expected next link for a top-level feed
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="lastEntityValue">The last entity value.</param>
        /// <returns>
        /// The expected next link
        /// </returns>
        string GenerateNextLink(ODataUri requestUri, int pageSize, QueryStructuralValue lastEntityValue);

       /// <summary>
        /// Generates the next link for an expanded feed.
        /// </summary>
        /// <param name="containingEntity">The containing entity.</param>
        /// <param name="navigation">The expanded navigation property.</param>
        /// <param name="lastEntityValue">The last entity value.</param>
        /// <returns>
        /// The expected next link
        /// </returns>
        string GenerateExpandedNextLink(EntityInstance containingEntity, NavigationPropertyInstance navigation, QueryStructuralValue lastEntityValue);
    }
}
