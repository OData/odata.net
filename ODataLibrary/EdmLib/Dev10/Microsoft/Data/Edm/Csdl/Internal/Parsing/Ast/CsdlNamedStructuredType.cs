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

using System.Collections.Generic;

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Common base class for CSDL entity and complex types.
    /// </summary>
    internal abstract class CsdlNamedStructuredType : CsdlStructuredType
    {
        protected string baseTypeName;
        protected bool isAbstract;
        protected string name;

        protected CsdlNamedStructuredType(string name, string baseTypeName, bool isAbstract, IEnumerable<CsdlProperty> properties, CsdlDocumentation documentation, CsdlLocation location)
            : base(properties, documentation, location)
        {
            this.isAbstract = isAbstract;
            this.name = name;
            this.baseTypeName = baseTypeName;
        }

        public string BaseTypeName
        {
            get { return this.baseTypeName; }
        }

        public bool IsAbstract
        {
            get { return this.isAbstract; }
        }

        public string Name
        {
            get { return this.name; }
        }
    }
}
