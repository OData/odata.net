//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    using System.Collections.Generic;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Query.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

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
        /// <param name="currentLevelEntityType">the top level entity type, will be overwritten with the last entity type in the chain</param>
        /// <param name="firstNonTypeToken">the first non type token in the path</param>
        /// <returns>A path with type segments added to it.</returns>
        public static IEnumerable<ODataPathSegment> FollowTypeSegments(PathSegmentToken firstTypeToken, IEdmModel model, int maxDepth, ref IEdmEntityType currentLevelEntityType, out PathSegmentToken firstNonTypeToken)
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
                IEdmEntityType previousLevelEntityType = currentLevelEntityType;
                currentLevelEntityType = UriEdmHelpers.FindTypeFromModel(model, currentToken.Identifier) as IEdmEntityType;
                if (currentLevelEntityType == null)
                {
                    // TODO: fix this error message?
                    throw new ODataException(ODataErrorStrings.ExpandItemBinder_CannotFindType(currentToken.Identifier));
                }

                UriEdmHelpers.CheckRelatedTo(previousLevelEntityType, currentLevelEntityType);
                pathToReturn.Add(new TypeSegment(currentLevelEntityType, /*entitySet*/null));

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
