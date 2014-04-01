//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Globalization;

namespace Microsoft.OData.Edm.Csdl
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
