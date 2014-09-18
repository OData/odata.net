//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL Navigation Property Binding.
    /// </summary>
    internal class CsdlNavigationPropertyBinding : CsdlElementWithDocumentation
    {
        private readonly string path;
        private readonly string target;

        public CsdlNavigationPropertyBinding(string path, string target, CsdlDocumentation documentation, CsdlLocation location)
            : base(documentation, location)
        {
            this.path = path;
            this.target = target;
        }

        public string Path
        {
            get { return this.path; }
        }

        public string Target
        {
            get { return this.target; }
        }
    }
}
