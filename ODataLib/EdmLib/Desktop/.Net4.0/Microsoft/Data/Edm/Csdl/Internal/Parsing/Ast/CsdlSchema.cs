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

using System;
using System.Collections.Generic;

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL schema.
    /// </summary>
    internal class CsdlSchema : CsdlElementWithDocumentation
    {
        private readonly List<CsdlUsing> usings;
        private readonly List<CsdlAssociation> associations;
        private readonly List<CsdlStructuredType> structuredTypes;
        private readonly List<CsdlEnumType> enumTypes;
        private readonly List<CsdlFunction> functions;
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
            IEnumerable<CsdlUsing> usings,
            IEnumerable<CsdlAssociation> associations,
            IEnumerable<CsdlStructuredType> structuredTypes,
            IEnumerable<CsdlEnumType> enumTypes,
            IEnumerable<CsdlFunction> functions,
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
            this.usings = new List<CsdlUsing>(usings);
            this.associations = new List<CsdlAssociation>(associations);
            this.structuredTypes = new List<CsdlStructuredType>(structuredTypes);
            this.enumTypes = new List<CsdlEnumType>(enumTypes);
            this.functions = new List<CsdlFunction>(functions);
            this.valueTerms = new List<CsdlValueTerm>(valueTerms);
            this.entityContainers = new List<CsdlEntityContainer>(entityContainers);
            this.outOfLineAnnotations = new List<CsdlAnnotations>(outOfLineAnnotations);
        }

        public IEnumerable<CsdlUsing> Usings
        {
            get { return this.usings; }
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

        public IEnumerable<CsdlFunction> Functions
        {
            get { return this.functions; }
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
