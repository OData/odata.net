//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Build a SelectBinder based on whether we're using V4 or not
    /// </summary>
    /// TODO 1466134 We don't need this layer once V4 is working and always used.
    internal static class SelectBinderFactory
    {
        /// <summary>
        /// Create a SelectBinder based on whether we're using V4 or not.
        /// </summary>
        /// <param name="model">The edm model provided by the user</param>
        /// <param name="entityType">the top level entity type</param>
        /// <param name="settings">the list of settings provided by the user</param>
        /// <param name="subExpand">the already built expand to decorate with this select clause.</param>
        /// <returns>Either a SelectBinder or a V4SelectBinder based on whether we're using V4 or not.</returns>
        public static ISelectBinder Create(IEdmModel model, IEdmStructuredType entityType, ODataUriParserSettings settings, SelectExpandClause subExpand)
        {
            if (settings.UseV4ExpandSemantics)
            {
                return new V4SelectBinder(model, entityType, settings.SelectExpandLimit, subExpand);
            }
            else
            {
                return new SelectBinder(model, entityType, settings.SelectExpandLimit, subExpand);
            }
        }
    }
}
