//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
