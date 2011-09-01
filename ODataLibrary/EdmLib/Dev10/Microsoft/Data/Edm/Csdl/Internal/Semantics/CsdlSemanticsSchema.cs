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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlSchema.
    /// </summary>
    internal class CsdlSemanticsSchema : CsdlSemanticsElement
    {
        private readonly CsdlSemanticsModel model;
        private readonly CsdlSchema schema;

        private readonly Cache<CsdlSemanticsSchema, IEnumerable<IEdmSchemaType>> typesCache = new Cache<CsdlSemanticsSchema, IEnumerable<IEdmSchemaType>>();
        private readonly static Func<CsdlSemanticsSchema, IEnumerable<IEdmSchemaType>> s_computeTypes = (me) => me.ComputeTypes();

        private readonly Cache<CsdlSemanticsSchema, IEnumerable<IEdmAssociation>> associationsCache = new Cache<CsdlSemanticsSchema, IEnumerable<IEdmAssociation>>();
        private readonly static Func<CsdlSemanticsSchema, IEnumerable<IEdmAssociation>> s_computeAssociations = (me) => me.ComputeAssociations();

        private readonly Cache<CsdlSemanticsSchema, IEnumerable<IEdmFunction>> functionsCache = new Cache<CsdlSemanticsSchema, IEnumerable<IEdmFunction>>();
        private readonly static Func<CsdlSemanticsSchema, IEnumerable<IEdmFunction>> s_computeFunctions = (me) => me.ComputeFunctions();

        private readonly Cache<CsdlSemanticsSchema, IEnumerable<IEdmEntityContainer>> entityContainersCache = new Cache<CsdlSemanticsSchema, IEnumerable<IEdmEntityContainer>>();
        private readonly static Func<CsdlSemanticsSchema, IEnumerable<IEdmEntityContainer>> s_computeEntityContainers = (me) => me.ComputeEntityContainers();

        private readonly Cache<CsdlSemanticsSchema, IEnumerable<IEdmValueTerm>> valueTermsCache = new Cache<CsdlSemanticsSchema, IEnumerable<IEdmValueTerm>>();
        private readonly static Func<CsdlSemanticsSchema, IEnumerable<IEdmValueTerm>> s_computeValueTerms = (me) => me.ComputeValueTerms();

        public CsdlSemanticsSchema(CsdlSemanticsModel model, CsdlSchema schema)
        {
            this.model = model;
            this.schema = schema;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.model; }
        }

        public override CsdlElement Element
        {
            get { return this.schema; }
        }

        public IEnumerable<IEdmSchemaType> Types
        {
            get { return this.typesCache.GetValue(this, s_computeTypes, null); }
        }

        private IEnumerable<IEdmSchemaType> ComputeTypes()
        {
            List<IEdmSchemaType> types = new List<IEdmSchemaType>();
            foreach (var structuredType in this.schema.StructuredTypes)
            {
                CsdlEntityType entity = structuredType as CsdlEntityType;
                if (entity != null)
                {
                    types.Add(new CsdlSemanticsEntityTypeDefinition(this, entity));
                }
                else
                {
                    CsdlComplexType complex = structuredType as CsdlComplexType;
                    if (complex != null)
                    {
                        types.Add(new CsdlSemanticsComplexTypeDefinition(this, complex));
                    }
                }
            }

            foreach (var enumType in this.schema.EnumTypes)
            {
                types.Add(new CsdlSemanticsEnumTypeDefinition(this, enumType));
            }

            return types;
        }

        public IEnumerable<IEdmAssociation> Associations
        {
            get { return this.associationsCache.GetValue(this, s_computeAssociations, null); }
        }

        public IEnumerable<IEdmFunction> Functions
        {
            get { return this.functionsCache.GetValue(this, s_computeFunctions, null); }
        }

        private IEnumerable<IEdmAssociation> ComputeAssociations()
        {
            List<IEdmAssociation> associations = new List<IEdmAssociation>();
            foreach (CsdlAssociation association in this.schema.Associations)
            {
                associations.Add(new CsdlSemanticsAssociation(this, association));
            }

            return associations;
        }

        private IEnumerable<IEdmFunction> ComputeFunctions()
        {
            List<IEdmFunction> functions = new List<IEdmFunction>();
            foreach (CsdlFunction function in this.schema.Functions)
            {
                functions.Add(new CsdlSemanticsFunction(this, function));
            }

            return functions;
        }

        public IEnumerable<IEdmValueTerm> ValueTerms
        {
            get { return this.valueTermsCache.GetValue(this, s_computeValueTerms, null); }
        }

        private IEnumerable<IEdmValueTerm> ComputeValueTerms()
        {
            List<IEdmValueTerm> valueTerms = new List<IEdmValueTerm>();
            foreach (CsdlValueTerm valueTerm in this.schema.ValueTerms)
            {
                valueTerms.Add(new CsdlSemanticsValueTerm(this, valueTerm));
            }

            return valueTerms;
        }

        public IEnumerable<IEdmEntityContainer> EntityContainers
        {
            get { return this.entityContainersCache.GetValue(this, s_computeEntityContainers, null); }
        }

        private IEnumerable<IEdmEntityContainer> ComputeEntityContainers()
        {
            List<IEdmEntityContainer> entityContainers = new List<IEdmEntityContainer>();
            foreach (CsdlEntityContainer entityContainer in this.schema.EntityContainers)
            {
                entityContainers.Add(new CsdlSemanticsEntityContainer(this, entityContainer));
            }

            return entityContainers;
        }

        public string Namespace
        {
            get { return this.schema.Namespace; }
        }

        public string NamespaceUri
        {
            get { return this.schema.NamespaceUri; }
        }

        public string GetNamespaceUri(string namespaceName)
        {
            if (namespaceName == this.Namespace)
            {
                return this.NamespaceUri ?? string.Empty;
            }

            foreach (CsdlUsing myUsing in this.schema.Usings)
            {
                if (namespaceName == myUsing.Alias)
                {
                    return myUsing.NamespaceUri ?? string.Empty;
                }
            }

            return string.Empty;
        }

        private static IEdmAssociation FindAssociation(IEdmModel model, string name)
        {
            return model.FindAssociation(name);
        }

        private static IEdmSchemaType FindType(IEdmModel model, string name)
        {
            return model.FindType(name);
        }

        public IEdmAssociation FindAssociation(string name)
        {
            return FindSchemaElement<IEdmAssociation>(name, FindAssociation);
        }

        public IEdmSchemaType FindType(string name)
        {
            return FindSchemaElement<IEdmSchemaType>(name, FindType);
        }

        public IEdmNamedElement FindElement(string name)
        {
            IEdmSchemaType targetType = FindType(name);
            if (targetType != null)
            {
                return targetType;
            }

            string containerName;
            string elementName;
            if (EdmUtil.TryGetNamespaceNameFromQualifiedName(name, out containerName, out elementName))
            {
                IEdmEntityContainer container = this.Model.FindEntityContainer(containerName);
                if (container != null)
                {
                    IEdmEntitySet entitySet = container.FindEntitySet(elementName);
                    if (entitySet != null)
                    {
                        return entitySet;
                    }
                }
            }

            return null;
        }

        private static IEdmValueTerm FindValueTerm(IEdmModel model, string name)
        {
            return model.FindValueTerm(name);
        }

        public IEdmValueTerm FindValueTerm(string name)
        {
            return FindSchemaElement<IEdmValueTerm>(name, FindValueTerm);
        }

        public T FindSchemaElement<T>(string name, Func<IEdmModel, string, T> modelFinder) where T : class, IEdmSchemaElement
        {
            T candidate = modelFinder(this.model, name);

            if (name.Contains('.'))
            {
                string aliasReplaced = ReplaceAlias(name);
                if (aliasReplaced != null)
                {
                    candidate = modelFinder(this.model, aliasReplaced);
                }
            }
            else
            {
                if (candidate == null)
                {
                    candidate = modelFinder(this.model, (this.Namespace ?? string.Empty) + "." + name);
                }
            }

            return candidate;
        }

        public string ReplaceAlias(string name)
        {
            string replaced = ReplaceAlias(this.Namespace, this.schema.Alias, name);
            if (replaced == null)
            {
                foreach (CsdlUsing myUsing in this.schema.Usings)
                {
                    replaced = ReplaceAlias(myUsing.Namespace, myUsing.Alias, name);
                    if (replaced != null)
                    {
                        break;
                    }
                }
            }

            return replaced;
        }

        private static string ReplaceAlias(string namespaceName, string namespaceAlias, string name)
        {
            if (namespaceAlias != null)
            {
                if (name.Length > namespaceAlias.Length && name.StartsWith(namespaceAlias, StringComparison.Ordinal) && name[namespaceAlias.Length] == '.')
                {
                    return (namespaceName ?? string.Empty) + name.Substring(namespaceAlias.Length);
                }
            }

            return null;
        }
    }
}
