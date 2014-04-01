//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    /// <summary>
    /// Create a SelectTreeNormalizer based on the values passed in from the user.
    /// </summary>
    //// TODO 1466134 We can delete this layer once V4 is the only thing used.
    internal sealed class SelectTreeNormalizerFactory
    {
        /// <summary>
        /// Build a SelectTreeNormalizer based on the values passed in from the user.
        /// </summary>
        /// <param name="configuration">The configuration values passed from the user</param>
        /// <returns>A V4SelectTreeNormalizer or a SelectTreeNormalizer based on the value from the user.</returns>
        public static ISelectTreeNormalizer Create(ODataUriParserConfiguration configuration)
        {
            if (configuration.Settings.UseV4ExpandSemantics)
            {
                return new V4SelectTreeNormalizer();
            }
            else
            {
                return new SelectTreeNormalizer();
            }
        }
    }
}
