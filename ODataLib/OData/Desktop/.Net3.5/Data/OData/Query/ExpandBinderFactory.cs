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
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Build an ExpandBinder based on global settings
    /// </summary>
    internal static class ExpandBinderFactory
    {
        /// <summary>
        /// Build an ExpandBinder based on global settings
        /// </summary>
        /// <param name="elementType">The entity type of the top level expand item.</param>
        /// <param name="entitySet">The entity set of the top level expand item.</param>
        /// <param name="configuration">The configuration to use for binding.</param>
        /// <returns>An ExpandBinder strategy based on the global settings</returns>
        public static ExpandBinder Create(IEdmEntityType elementType, IEdmEntitySet entitySet, ODataUriParserConfiguration configuration)
        {
            Debug.Assert(configuration != null, "configuration != null");
            Debug.Assert(configuration.Settings != null, "configuration.Settings != null");
            if (configuration.Settings.SupportExpandOptions)
            {
                return new ExpandOptionExpandBinder(configuration, elementType, entitySet);
            }
            else
            {
                return new NonOptionExpandBinder(configuration, elementType, entitySet);
            }
        }
    }
}
