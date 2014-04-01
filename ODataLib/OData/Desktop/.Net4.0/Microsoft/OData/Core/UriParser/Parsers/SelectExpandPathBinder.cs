//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Collections.Generic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

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
        /// <param name="currentLevelType">the top level type, will be overwritten with the last entity type in the chain</param>
        /// <param name="firstNonTypeToken">the first non type token in the path</param>
        /// <returns>A path with type segments added to it.</returns>
        public static IEnumerable<ODataPathSegment> FollowTypeSegments(PathSegmentToken firstTypeToken, IEdmModel model, int maxDepth, ref IEdmStructuredType currentLevelType, out PathSegmentToken firstNonTypeToken)
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
                currentLevelType = UriEdmHelpers.FindTypeFromModel(model, currentToken.Identifier) as IEdmStructuredType;
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
