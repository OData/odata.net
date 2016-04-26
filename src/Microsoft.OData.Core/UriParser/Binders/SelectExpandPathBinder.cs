//---------------------------------------------------------------------
// <copyright file="SelectExpandPathBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Collections.Generic;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Walk down a chain of type segments, checking that we find the correct type at each level.
    /// </summary>
    internal static class SelectExpandPathBinder
    {
        /// <summary>
        /// Follow any type segments on the path, stopping at the first segment that isn't a type token.
        /// </summary>
        /// <param name="firstTypeToken">the first type segment</param>
        /// <param name="model">the model these types are contained in.</param>
        /// <param name="maxDepth">the maximum recursive depth</param>
        /// <param name="resolver">Resolver for uri parser.</param>
        /// <param name="currentLevelType">the top level type, will be overwritten with the last entity type in the chain</param>
        /// <param name="firstNonTypeToken">the first non type token in the path</param>
        /// <returns>A path with type segments added to it.</returns>
        public static IEnumerable<ODataPathSegment> FollowTypeSegments(PathSegmentToken firstTypeToken, IEdmModel model, int maxDepth, ODataUriResolver resolver, ref IEdmStructuredType currentLevelType, out PathSegmentToken firstNonTypeToken)
        {
            ExceptionUtils.CheckArgumentNotNull(firstTypeToken, "firstTypeToken");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            if (!firstTypeToken.IsNamespaceOrContainerQualified())
            {
                throw new ODataException(ODataErrorStrings.SelectExpandPathBinder_FollowNonTypeSegment(firstTypeToken.Identifier));
            }

            int index = 0;
            List<ODataPathSegment> pathToReturn = new List<ODataPathSegment>();
            PathSegmentToken currentToken = firstTypeToken;
            while (currentToken.IsNamespaceOrContainerQualified() && currentToken.NextToken != null)
            {
                IEdmType previousLevelEntityType = currentLevelType;
                currentLevelType = UriEdmHelpers.FindTypeFromModel(model, currentToken.Identifier, resolver) as IEdmStructuredType;
                if (currentLevelType == null)
                {
                    // TODO: fix this error message?
                    throw new ODataException(ODataErrorStrings.ExpandItemBinder_CannotFindType(currentToken.Identifier));
                }

                UriEdmHelpers.CheckRelatedTo(previousLevelEntityType, currentLevelType);
                pathToReturn.Add(new TypeSegment(currentLevelType, /*entitySet*/null));

                index++;
                currentToken = currentToken.NextToken;

                if (index >= maxDepth)
                {
                    throw new ODataException(ODataErrorStrings.ExpandItemBinder_PathTooDeep);
                }
            }

            firstNonTypeToken = currentToken;

            return pathToReturn;
        }
    }
}