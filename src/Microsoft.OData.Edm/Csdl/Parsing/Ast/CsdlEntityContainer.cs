//---------------------------------------------------------------------
// <copyright file="CsdlEntityContainer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
