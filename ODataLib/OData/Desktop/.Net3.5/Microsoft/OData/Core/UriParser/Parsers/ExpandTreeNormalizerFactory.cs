//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    /// <summary>
    /// Build an ExpandTreeNormalizer based on the configuration value passed in from the server
    /// </summary>
    //// TODO 1466134 We don't need this layer once V4 is working and always used.
    internal static class ExpandTreeNormalizerFactory
    {
        /// <summary>
        /// Create an <see cref="IExpandTreeNormalizer"/> based on the value passed in as configuration.
        /// </summary>
        /// <param name="configuration">configuration values passed in from the user.</param>
        /// <returns>Either a V4ExpandTreeNormalizer or an ExpandTreeNormalizer based on the value of UseV4Semantics and SupportExpandOptions</returns>
        public static IExpandTreeNormalizer Create(ODataUriParserConfiguration configuration)
        {
            if (configuration.Settings.UseV4ExpandSemantics || configuration.Settings.SupportExpandOptions)
            {
                return new V4ExpandTreeNormalizer();
            }
            else
            {
                return new ExpandTreeNormalizer();
            }
        }
    }
}
