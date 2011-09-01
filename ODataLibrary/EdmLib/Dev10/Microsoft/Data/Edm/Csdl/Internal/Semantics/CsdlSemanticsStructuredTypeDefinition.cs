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
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlStructuredType.
    /// </summary>
    internal abstract class CsdlSemanticsStructuredTypeDefinition : CsdlSemanticsTypeDefinition, IEdmStructuredType
    {
        protected readonly CsdlSemanticsSchema context;

        private readonly Cache<CsdlSemanticsStructuredTypeDefinition, List<IEdmProperty>> declaredPropertiesCache = new Cache<CsdlSemanticsStructuredTypeDefinition, List<IEdmProperty>>();
        private readonly static Func<CsdlSemanticsStructuredTypeDefinition, List<IEdmProperty>> s_computeDeclaredProperties = (me) => me.ComputeDeclaredProperties();

        private readonly Cache<CsdlSemanticsStructuredTypeDefinition, IDictionary<string, IEdmProperty>> propertiesDictionaryCache = new Cache<CsdlSemanticsStructuredTypeDefinition, IDictionary<string, IEdmProperty>>();
        private readonly static Func<CsdlSemanticsStructuredTypeDefinition, IDictionary<string, IEdmProperty>> s_computePropertiesDictionary = (me) => me.ComputePropertiesDictionary();

        public CsdlSemanticsStructuredTypeDefinition(CsdlSemanticsSchema context)
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

        protected abstract CsdlStructuredType MyStructured
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
            get { return this.declaredPropertiesCache.GetValue(this, s_computeDeclaredProperties, null); }
        }

        private IDictionary<string, IEdmProperty> PropertiesDictionary
        {
            get { return this.propertiesDictionaryCache.GetValue(this, s_computePropertiesDictionary, null); }
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

        private IDictionary<string, IEdmProperty> ComputePropertiesDictionary()
        {
            Dictionary<string, IEdmProperty> properties = new Dictionary<string, IEdmProperty>();
            foreach (IEdmProperty property in this.Properties())
            {
                RegistrationHelper.RegisterProperty(property, property.Name, properties);
            }

            return properties;
        }

        // Resolves the real name of the base type, in case it was using an alias before.
        protected string GetCyclicBaseTypeName(string baseTypeName)
        {
            IEdmSchemaType schemaBaseType = this.context.FindType(baseTypeName);
            return (schemaBaseType != null) ? schemaBaseType.FullName() : baseTypeName;
        }

        protected override IEnumerable<IEdmAnnotation> ComputeImmutableAnnotations()
        {
            return this.Model.WrapAnnotations(this, this.context);
        }
    }
}
