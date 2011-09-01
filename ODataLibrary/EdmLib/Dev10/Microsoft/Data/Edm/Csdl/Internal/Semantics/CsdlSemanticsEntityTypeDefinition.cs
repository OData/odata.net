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
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlEntityType.
    /// </summary>
    internal class CsdlSemanticsEntityTypeDefinition : CsdlSemanticsStructuredTypeDefinition, IEdmEntityType, IEdmCheckable
    {
        private readonly CsdlEntityType entity;

        private readonly Cache<CsdlSemanticsEntityTypeDefinition, IEdmEntityType> baseTypeCache = new Cache<CsdlSemanticsEntityTypeDefinition, IEdmEntityType>();
        private readonly static Func<CsdlSemanticsEntityTypeDefinition, IEdmEntityType> s_computeBaseType = (me) => me.ComputeBaseType();
        private readonly static Func<CsdlSemanticsEntityTypeDefinition, IEdmEntityType> s_onCycleBaseType = (me) => new CyclicEntityType(me.GetCyclicBaseTypeName(me.entity.BaseTypeName), me.Location);

        private readonly Cache<CsdlSemanticsEntityTypeDefinition, IEnumerable<IEdmStructuralProperty>> declaredKeyCache = new Cache<CsdlSemanticsEntityTypeDefinition, IEnumerable<IEdmStructuralProperty>>();
        private readonly static Func<CsdlSemanticsEntityTypeDefinition, IEnumerable<IEdmStructuralProperty>> s_computeDeclaredKey = (me) => me.ComputeDeclaredKey();

        public CsdlSemanticsEntityTypeDefinition(CsdlSemanticsSchema context, CsdlEntityType entity)
            : base(context)
        {
            this.entity = entity;
        }

        public override IEdmStructuredType BaseType
        {
            get { return this.baseTypeCache.GetValue(this, s_computeBaseType, s_onCycleBaseType); }
        }

        protected override CsdlStructuredType MyStructured
        {
            get { return this.entity; }
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Entity; }
        }

        public string Name
        {
            get { return this.entity.Name; }
        }

        string IEdmTerm.NamespaceUri
        {
            get { return this.context.NamespaceUri; }
        }

        public override bool IsAbstract
        {
            get { return this.entity.IsAbstract; }
        }

        public override bool IsOpen
        {
            get { return this.entity.IsOpen; }
        }

        private IEdmEntityType ComputeBaseType()
        {
            if (this.entity.BaseTypeName != null)
            {
                IEdmEntityType baseType = this.context.FindType(this.entity.BaseTypeName) as IEdmEntityType;
                if (baseType != null)
                {
                    IEdmStructuredType junk = baseType.BaseType; // Evaluate the inductive step to detect cycles.
                }

                return baseType ?? new UnresolvedEntityType(this.entity.BaseTypeName, this.Location);
            }

            return null;
        }

        public IEnumerable<IEdmStructuralProperty> DeclaredKey
        {
            get
            {
                return this.declaredKeyCache.GetValue(this, s_computeDeclaredKey, null);
            }
        }

        protected override List<IEdmProperty> ComputeDeclaredProperties()
        {
            List<IEdmProperty> properties = base.ComputeDeclaredProperties();
            foreach (CsdlNavigationProperty navigationProperty in this.entity.NavigationProperties)
            {
                properties.Add(new CsdlSemanticsNavigationProperty(this, navigationProperty));
            }

            return properties;
        }

        private IEnumerable<IEdmStructuralProperty> ComputeDeclaredKey()
        {
            if (this.entity.Key != null)
            {
                List<IEdmStructuralProperty> key = new List<IEdmStructuralProperty>();
                foreach (CsdlPropertyReference property in this.entity.Key.Properties)
                {
                    key.Add(FindProperty(property.PropertyName) as IEdmStructuralProperty ?? new UnresolvedProperty(this, property.PropertyName, this.Location));
                }

                return key;
            }

            return null;
        }

        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Type; }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                return ((this.BaseType is CyclicEntityType) || (this.BaseType is UnresolvedEntityType))
                           ? this.BaseType.Errors()
                           : Enumerable.Empty<EdmError>();
            }
        }
    }
}
