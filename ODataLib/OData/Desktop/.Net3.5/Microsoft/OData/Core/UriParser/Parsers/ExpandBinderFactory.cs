//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Build an ExpandBinder based on global settings
    /// </summary>
    //// TODO 1466134 We don't need this layer once V4 is working and always used.
    internal static class ExpandBinderFactory
    {
        /// <summary>
        /// Build an ExpandBinder based on global settings
        /// </summary>
        /// <param name="elementType">The type of the top level expand item.</param>
        /// <param name="entitySet">The entity set of the top level expand item.</param>
        /// <param name="configuration">The configuration to use for binding.</param>
        /// <returns>An ExpandBinder strategy based on the global settings</returns>
        public static ExpandBinder Create(IEdmStructuredType elementType, IEdmEntitySet entitySet, ODataUriParserConfiguration configuration)
        {
            Debug.Assert(configuration != null, "configuration != null");
            Debug.Assert(configuration.Settings != null, "configuration.Settings != null");
            if (configuration.Settings.UseV4ExpandSemantics)
            {
                return new V4ExpandBinder(configuration, elementType, entitySet);
            }
            else if (configuration.Settings.SupportExpandOptions)
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
