//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL schema.
    /// </summary>
    internal class CsdlSchema : CsdlElementWithDocumentation
    {
        private readonly List<CsdlAssociation> associations;
        private readonly List<CsdlStructuredType> structuredTypes;
        private readonly List<CsdlEnumType> enumTypes;
        private readonly List<CsdlOperation> operations;
        private readonly List<CsdlValueTerm> valueTerms;
        private readonly List<CsdlEntityContainer> entityContainers;
        private readonly List<CsdlAnnotations> outOfLineAnnotations;

        private readonly string alias;
        private readonly string namespaceName;
        private readonly Version version;

        public CsdlSchema(
            string namespaceName,
            string alias,
            Version version,
            IEnumerable<CsdlAssociation> associations,
            IEnumerable<CsdlStructuredType> structuredTypes,
            IEnumerable<CsdlEnumType> enumTypes,
            IEnumerable<CsdlOperation> functions,
            IEnumerable<CsdlValueTerm> valueTerms,
            IEnumerable<CsdlEntityContainer> entityContainers,
            IEnumerable<CsdlAnnotations> outOfLineAnnotations,
            CsdlDocumentation documentation,
            CsdlLocation location)
            : base(documentation, location)
        {
            this.alias = alias;
            this.namespaceName = namespaceName;
            this.version = version;
            this.associations = new List<CsdlAssociation>(associations);
            this.structuredTypes = new List<CsdlStructuredType>(structuredTypes);
            this.enumTypes = new List<CsdlEnumType>(enumTypes);
            this.operations = new List<CsdlOperation>(functions);
            this.valueTerms = new List<CsdlValueTerm>(valueTerms);
            this.entityContainers = new List<CsdlEntityContainer>(entityContainers);
            this.outOfLineAnnotations = new List<CsdlAnnotations>(outOfLineAnnotations);
        }

        public IEnumerable<CsdlAssociation> Associations
        {
            get { return this.associations; }
        }

        public IEnumerable<CsdlStructuredType> StructuredTypes
        {
            get { return this.structuredTypes; }
        }

        public IEnumerable<CsdlEnumType> EnumTypes
        {
            get { return this.enumTypes; }
        }

        public IEnumerable<CsdlOperation> Operations
        {
            get { return this.operations; }
        }

        public IEnumerable<CsdlValueTerm> ValueTerms
        {
            get { return this.valueTerms; }
        }

        public IEnumerable<CsdlEntityContainer> EntityContainers
        {
            get { return this.entityContainers; }
        }

        public IEnumerable<CsdlAnnotations> OutOfLineAnnotations
        {
            get { return this.outOfLineAnnotations; }
        }

        public string Alias
        {
            get { return this.alias; }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public Version Version
        {
            get { return this.version; }
        }
    }
}
