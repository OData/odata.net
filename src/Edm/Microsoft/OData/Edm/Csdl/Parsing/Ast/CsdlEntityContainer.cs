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
