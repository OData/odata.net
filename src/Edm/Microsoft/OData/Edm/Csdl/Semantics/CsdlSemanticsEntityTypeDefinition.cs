//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Library;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlEntityType.
    /// </summary>
    internal class CsdlSemanticsEntityTypeDefinition : CsdlSemanticsStructuredTypeDefinition, IEdmEntityType
    {
        private readonly CsdlEntityType entity;

        private readonly Cache<CsdlSemanticsEntityTypeDefinition, IEdmEntityType> baseTypeCache = new Cache<CsdlSemanticsEntityTypeDefinition, IEdmEntityType>();
        private static readonly Func<CsdlSemanticsEntityTypeDefinition, IEdmEntityType> ComputeBaseTypeFunc = (me) => me.ComputeBaseType();
        private static readonly Func<CsdlSemanticsEntityTypeDefinition, IEdmEntityType> OnCycleBaseTypeFunc = (me) => new CyclicEntityType(me.GetCyclicBaseTypeName(me.entity.BaseTypeName), me.Location);

        private readonly Cache<CsdlSemanticsEntityTypeDefinition, IEnumerable<IEdmStructuralProperty>> declaredKeyCache = new Cache<CsdlSemanticsEntityTypeDefinition, IEnumerable<IEdmStructuralProperty>>();
        private static readonly Func<CsdlSemanticsEntityTypeDefinition, IEnumerable<IEdmStructuralProperty>> ComputeDeclaredKeyFunc = (me) => me.ComputeDeclaredKey();

        public CsdlSemanticsEntityTypeDefinition(CsdlSemanticsSchema context, CsdlEntityType entity)
            : base(context, entity)
        {
            this.entity = entity;
        }

        public override IEdmStructuredType BaseType
        {
            get { return this.baseTypeCache.GetValue(this, ComputeBaseTypeFunc, OnCycleBaseTypeFunc); }
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Entity; }
        }

        public string Name
        {
            get { return this.entity.Name; }
        }

        public override bool IsAbstract
        {
            get { return this.entity.IsAbstract; }
        }

        public override bool IsOpen
        {
            get { return this.entity.IsOpen; }
        }

        public bool HasStream
        {
            get { return this.entity.HasStream; }
        }

        public IEnumerable<IEdmStructuralProperty> DeclaredKey
        {
            get
            {
                return this.declaredKeyCache.GetValue(this, ComputeDeclaredKeyFunc, null);
            }
        }

        public EdmTermKind TermKind
        {
            get { return EdmTermKind.Type; }
        }

        protected override CsdlStructuredType MyStructured
        {
            get { return this.entity; }
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

        private IEdmEntityType ComputeBaseType()
        {
            if (this.entity.BaseTypeName != null)
            {
                IEdmEntityType baseType = this.Context.FindType(this.entity.BaseTypeName) as IEdmEntityType;
                if (baseType != null)
                {
                    IEdmStructuredType junk = baseType.BaseType; // Evaluate the inductive step to detect cycles.
                }

                return baseType ?? new UnresolvedEntityType(this.Context.UnresolvedName(this.entity.BaseTypeName), this.Location);
            }

            return null;
        }

        private IEnumerable<IEdmStructuralProperty> ComputeDeclaredKey()
        {
            if (this.entity.Key != null)
            {
                List<IEdmStructuralProperty> key = new List<IEdmStructuralProperty>();
                foreach (CsdlPropertyReference keyProperty in this.entity.Key.Properties)
                {
                    IEdmStructuralProperty structuralProperty = this.FindProperty(keyProperty.PropertyName) as IEdmStructuralProperty;
                    if (structuralProperty != null)
                    {
                        key.Add(structuralProperty);
                    }
                    else
                    {
                        // If keyProperty is a duplicate, it will come back as non-structural from FindProperty, but it still might be structural
                        // inside the DeclaredProperties, so try it. If it is not in the DeclaredProperties or it is not structural there,
                        // then fall back to unresolved.
                        structuralProperty = this.DeclaredProperties.FirstOrDefault(p => p.Name == keyProperty.PropertyName) as IEdmStructuralProperty;
                        if (structuralProperty != null)
                        {
                            key.Add(structuralProperty);
                        }
                        else
                        {
                            key.Add(new UnresolvedProperty(this, keyProperty.PropertyName, this.Location));
                        }
                    }
                }

                return key;
            }

            return null;
        }
    }
}
