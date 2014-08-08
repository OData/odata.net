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

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL documentation.
    /// </summary>
    internal class CsdlDocumentation : CsdlElement
    {
        private readonly string summary;
        private readonly string longDescription;

        public CsdlDocumentation(string summary, string longDescription, CsdlLocation location)
            : base(location)
        {
            this.summary = summary;
            this.longDescription = longDescription;
        }

        public string Summary
        {
            get { return this.summary; }
        }

        public string LongDescription
        {
            get { return this.longDescription; }
        }
    }
}
