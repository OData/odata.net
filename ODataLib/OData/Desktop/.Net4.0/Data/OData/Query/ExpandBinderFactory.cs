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
