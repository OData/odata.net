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

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Semantic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Translate a select tree into the right format to be used with an expand tree.
    /// </summary>
    //// TODO: Rename this to SelectTreeNormalizer when we're only using V4
    internal sealed class SelectTreeNormalizer
    {
        /// <summary>
        /// Normalize a SelectToken into something that can be used to trim an expand tree.
        /// </summary>
        /// <param name="treeToNormalize">The select token to normalize</param>
        /// <returns>Normalized SelectToken</returns>
        public SelectToken NormalizeSelectTree(SelectToken treeToNormalize)
        {
            PathReverser pathReverser = new PathReverser();
            List<PathSegmentToken> invertedPaths = (from property in treeToNormalize.Properties 
                                                    select property.Accept(pathReverser)).ToList();

            // to normalize a select token we just need to invert its paths, so that 
            // we match the ordering on an ExpandToken.
            return new SelectToken(invertedPaths);
        }
    }
}
