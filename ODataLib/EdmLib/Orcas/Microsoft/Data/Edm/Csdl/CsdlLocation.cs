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

using System;
using System.Globalization;

namespace Microsoft.Data.Edm.Csdl
{
    /// <summary>
    /// Defines a location in a XML file.
    /// </summary>
    public class CsdlLocation : EdmLocation
    {
        internal CsdlLocation(int number, int position)
        {
            this.LineNumber = number;
            this.LinePosition = position;
        }

        /// <summary>
        /// Gets the line number in the file.
        /// </summary>
        public int LineNumber { get; private set; }

        /// <summary>
        /// Gets the position in the line.
        /// </summary>
        public int LinePosition { get; private set; }

        /// <summary>
        /// Gets a string representation of the location.
        /// </summary>
        /// <returns>A string representation of the location.</returns>
        public override string ToString()
        {
            return "(" + Convert.ToString(this.LineNumber, CultureInfo.InvariantCulture) + ", " + Convert.ToString(this.LinePosition, CultureInfo.InvariantCulture) + ")";
        }
    }
}
