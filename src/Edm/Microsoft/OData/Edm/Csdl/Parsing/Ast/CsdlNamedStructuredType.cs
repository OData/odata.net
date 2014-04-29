//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Common base class for CSDL entity and complex types.
    /// </summary>
    internal abstract class CsdlNamedStructuredType : CsdlStructuredType
    {
        protected string baseTypeName;
        protected bool isAbstract;
        protected bool isOpen;
        protected string name;

        protected CsdlNamedStructuredType(string name, string baseTypeName, bool isAbstract, bool isOpen, IEnumerable<CsdlProperty> properties, CsdlDocumentation documentation, CsdlLocation location)
            : base(properties, documentation, location)
        {
            this.isAbstract = isAbstract;
            this.isOpen = isOpen;
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

        public bool IsOpen
        {
            get { return this.isOpen; }
        }

        public string Name
        {
            get { return this.name; }
        }
    }
}
