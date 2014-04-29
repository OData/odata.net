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
    /// Represents a CSDL entity container.
    /// </summary>
    internal class CsdlEntityContainer : CsdlNamedElement
    {
        private readonly string extends;
        private readonly List<CsdlEntitySet> entitySets;
        private readonly List<CsdlSingleton> singletons;
        private readonly List<CsdlOperationImport> operationImports;

        public CsdlEntityContainer(string name, string extends, IEnumerable<CsdlEntitySet> entitySets, IEnumerable<CsdlSingleton> singletons, IEnumerable<CsdlOperationImport> operationImports, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.extends = extends;
            this.entitySets = new List<CsdlEntitySet>(entitySets);
            this.singletons = new List<CsdlSingleton>(singletons);
            this.operationImports = new List<CsdlOperationImport>(operationImports);
        }

        public string Extends
        {
            get { return this.extends; }
        }

        public IEnumerable<CsdlEntitySet> EntitySets
        {
            get { return this.entitySets; }
        }

        public IEnumerable<CsdlSingleton> Singletons
        {
            get { return this.singletons; }
        }

        public IEnumerable<CsdlOperationImport> OperationImports
        {
            get { return this.operationImports; }
        }
    }
}
