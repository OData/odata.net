//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces
    using System;
    using System.IO;
    using Microsoft.Spatial;
    using Microsoft.Data.Spatial;
    #endregion Namespaces

    /// <summary>
    /// Helper methods for with literals.
    /// </summary>
    internal static class LiteralUtils
    {
        /// <summary>
        /// The formatter to create/format text to and from spatial.
        /// </summary>
        private static WellKnownTextSqlFormatter Formatter
        {
            get
            {
                return SpatialImplementation.CurrentImplementation.CreateWellKnownTextSqlFormatter(false /*allowOnlyTwoDimensions*/);
            }
        }

        /// <summary>
        /// Parse the given text as a Geography literal.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>The Geography object if succeeded, else a ParseErrorException is thrown.</returns>
        internal static Geography ParseGeography(string text)
        {
            using (StringReader reader = new StringReader(text))
            {
                return Formatter.Read<Geography>(reader);
            }
        }

        /// <summary>
        /// Parse the given text as a Geometry literal.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>The Geometry object if succeeded, else a ParseErrorException is thrown.</returns>
        internal static Geometry ParseGeometry(string text)
        {
            using (StringReader reader = new StringReader(text))
            {
                return Formatter.Read<Geometry>(reader);
            }
        }

        /// <summary>
        /// Convert to string the given Geography instance.
        /// </summary>
        /// <param name="instance">Instance to convert.</param>
        /// <returns>Well-known text representation.</returns>
        internal static string ToWellKnownText(Geography instance)
        {
            return Formatter.Write(instance);
        }

        /// <summary>
        /// Convert to string the given Geometry instance.
        /// </summary>
        /// <param name="instance">Instance to convert.</param>
        /// <returns>Well-known text representation.</returns>
        internal static string ToWellKnownText(Geometry instance)
        {
            return Formatter.Write(instance);
        }
    }
}
