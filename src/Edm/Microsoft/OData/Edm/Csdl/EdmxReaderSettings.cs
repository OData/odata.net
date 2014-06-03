//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl
{
    using System;
    using System.Xml;

    /// <summary>
    /// Settings used when parsing Edmx document.
    /// </summary>
    public sealed class EdmxReaderSettings
    {
        /// <summary>
        /// Default constructor for EdmxReaderSettings
        /// </summary>
        public EdmxReaderSettings()
        {
            this.IgnoreUnexpectedAttributesAndElements = false;
        }

        /// <summary>
        /// The function to load referenced model xml. If null, will stop loading the referenced models. Normally it should throw no exception.
        /// </summary>
        public Func<Uri, XmlReader> GetReferencedModelReaderFunc { get; set; }

        /// <summary>
        /// Ignore the unexpected attributes and elements in schema.
        /// </summary>
        public bool IgnoreUnexpectedAttributesAndElements { get; set; }
    }
}
