//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;

    /// <summary>
    /// Build an ISelectExpandBinder based on whether we're using V4 or not
    /// </summary>
    //// TODO 1466134 We don't need this factory once V4 is working and always used.
    internal static class SelectExpandSemanticBinderFactory
    {
        /// <summary>
        /// Create an ISelectExpandSemanticBinder, using the settings the user passed in.
        /// </summary>
        /// <param name="configuration">the settings that the user passed in.</param>
        /// <returns>Either a V4SelectExpandBinder or a SelectExpandSemanticBinder, based user configuration.</returns>
        public static ISelectExpandSemanticBinder Create(ODataUriParserConfiguration configuration)
        {
            if (configuration.Settings.UseV4ExpandSemantics)
            {
                return new V4SelectExpandSemanticBinder();
            }
            else
            {
                return new SelectExpandSemanticBinder();
            }
        }
    }
}
