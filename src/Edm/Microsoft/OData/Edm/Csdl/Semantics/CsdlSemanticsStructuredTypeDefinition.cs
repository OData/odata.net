//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlStructuredType.
    /// </summary>
    internal abstract class CsdlSemanticsStructuredTypeDefinition : CsdlSemanticsTypeDefinition, IEdmStructuredType
    {
        private readonly CsdlSemanticsSchema context;

        private readonly Cache<CsdlSemanticsStructuredTypeDefinition, List<IEdmProperty>> declaredPropertiesCache = new Cache<CsdlSemanticsStructuredTypeDefinition, List<IEdmProperty>>();
        private static readonly Func<CsdlSemanticsStructuredTypeDefinition, List<IEdmProperty>> ComputeDeclaredPropertiesFunc = (me) => me.ComputeDeclaredProperties();

        private readonly Cache<CsdlSemanticsStructuredTypeDefinition, IDictionary<string, IEdmProperty>> propertiesDictionaryCache = new Cache<CsdlSemanticsStructuredTypeDefinition, IDictionary<string, IEdmProperty>>();
        private static readonly Func<CsdlSemanticsStructuredTypeDefinition, IDictionary<string, IEdmProperty>> ComputePropertiesDictionaryFunc = (me) => me.ComputePropertiesDictionary();

        protected CsdlSemanticsStructuredTypeDefinition(CsdlSemanticsSchema context, CsdlStructuredType type)
            : base(type)
        {
            this.context = context;
        }

        public virtual bool IsAbstract
        {
            get { return false; }
        }

        public virtual bool IsOpen
        {
            get { return false; }
        }

        public abstract IEdmStructuredType BaseType
        {
            get;
        }

        public override CsdlElement Element
        {
            get { return this.MyStructured; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        public string Namespace
        {
            get { return this.context.Namespace; }
        }

        public CsdlSemanticsSchema Context
        {
            get { return this.context; }
        }

        public IEnumerable<IEdmProperty> DeclaredProperties
        {
            get { return this.declaredPropertiesCache.GetValue(this, ComputeDeclaredPropertiesFunc, null); }
        }

        protected abstract CsdlStructuredType MyStructured
        {
            get;
        }

        private IDictionary<string, IEdmProperty> PropertiesDictionary
        {
            get { return this.propertiesDictionaryCache.GetValue(this, ComputePropertiesDictionaryFunc, null); }
        }

        public IEdmProperty FindProperty(string name)
        {
            IEdmProperty result;
            this.PropertiesDictionary.TryGetValue(name, out result);
            return result;
        }

        protected virtual List<IEdmProperty> ComputeDeclaredProperties()
        {
            List<IEdmProperty> properties = new List<IEdmProperty>();
            foreach (CsdlProperty property in this.MyStructured.Properties)
            {
                properties.Add(new CsdlSemanticsProperty(this, property));
            }

            return properties;
        }

        // Resolves the real name of the base type, in case it was using an alias before.
        protected string GetCyclicBaseTypeName(string baseTypeName)
        {
            IEdmSchemaType schemaBaseType = this.context.FindType(baseTypeName);
            return (schemaBaseType != null) ? schemaBaseType.FullName() : baseTypeName;
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.context);
        }

        private IDictionary<string, IEdmProperty> ComputePropertiesDictionary()
        {
            Dictionary<string, IEdmProperty> properties = new Dictionary<string, IEdmProperty>();
            foreach (IEdmProperty property in this.Properties())
            {
                RegistrationHelper.RegisterProperty(property, property.Name, properties);
            }

            return properties;
        }
    }
}
