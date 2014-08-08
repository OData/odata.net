//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.OData.Query
{
    using System;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Expressions;
    using Microsoft.Data.OData;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Class that knows how to resolve an IEdmExpression to find its associated EntitySet.
    /// This functionality is needed to determine what a EntitySets a FunctionImport applies to.
    /// </summary>
    internal static class EntitySetExpressionResolver
    {
        /// <summary>
        /// Resolves an IEdmExpression to an IEdmEntitySet.
        /// </summary>
        /// <param name="expression">Expression to resolve.</param>
        /// <returns>The resolved EntitySet.</returns>
        internal static IEdmEntitySet ResolveEntitySetFromExpression(IEdmExpression expression)
        {
            DebugUtils.CheckNoExternalCallers();
            if (expression == null)
            {
                return null;
            }

            switch (expression.ExpressionKind)
            {
                case EdmExpressionKind.EntitySetReference:
                    return ((IEdmEntitySetReferenceExpression)expression).ReferencedEntitySet;
                default:
                    // TODO: we should support all the other options
                    throw new NotSupportedException(
                        ODataErrorStrings.Nodes_NonStaticEntitySetExpressionsAreNotSupportedInThisRelease);
            }
        }
    }
}
