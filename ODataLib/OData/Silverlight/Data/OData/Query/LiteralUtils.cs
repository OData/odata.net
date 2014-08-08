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
    #region Namespaces
    using System;
    using System.IO;
    using System.Spatial;
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
            DebugUtils.CheckNoExternalCallers();

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
            DebugUtils.CheckNoExternalCallers();

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
            DebugUtils.CheckNoExternalCallers();
            return Formatter.Write(instance);
        }

        /// <summary>
        /// Convert to string the given Geometry instance.
        /// </summary>
        /// <param name="instance">Instance to convert.</param>
        /// <returns>Well-known text representation.</returns>
        internal static string ToWellKnownText(Geometry instance)
        {
            DebugUtils.CheckNoExternalCallers();
            return Formatter.Write(instance);
        }
    }
}
