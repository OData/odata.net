//---------------------------------------------------------------------
// <copyright file="CsdlLocation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Globalization;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Defines a location in a CSDL file.
    /// </summary>
    public class CsdlLocation : EdmLocation
    {
        internal CsdlLocation(int number, int position)
            : this(null, number, position)
        {
        }

        internal CsdlLocation(string source, int number, int position)
        {
            this.Source = source;
            this.LineNumber = number;
            this.LinePosition = position;
        }

        internal CsdlLocation(string jsonPath)
        {
            Path = jsonPath;
        }

        /// <summary>
        /// Gets the source of the file.
        /// </summary>
        /// <remarks>
        /// Value 'null' means the source is unknown.
        /// Empty value means there is no source (e.g., constructed from in-memory string).
        /// Non-empty value indicates there is a certain source.
        /// </remarks>
        public string Source { get; private set; }

        /// <summary>
        /// Gets the line number in the file.
        /// </summary>
        public int LineNumber { get; private set; }

        /// <summary>
        /// Gets the position in the line.
        /// </summary>
        public int LinePosition { get; private set; }

        /// <summary>
        /// Gets the path string.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets a string representation of the location.
        /// </summary>
        /// <returns>A string representation of the location.</returns>
        public override string ToString()
        {
            if (Path != null)
            {
                return Path;
            }

            return "(" + Convert.ToString(this.LineNumber, CultureInfo.InvariantCulture) + ", " + Convert.ToString(this.LinePosition, CultureInfo.InvariantCulture) + ")";
        }
    }
}
