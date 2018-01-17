//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsEntityTypeDefinition.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

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

        protected override CsdlStructuredType MyStructured
        {
            get { return this.entity; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "baseType2",
            Justification = "Value assignment is required by compiler.")]
        private IEdmEntityType ComputeBaseType()
        {
            if (this.entity.BaseTypeName != null)
            {
                IEdmEntityType baseType = this.Context.FindType(this.entity.BaseTypeName) as IEdmEntityType;
                if (baseType != null)
                {
                    // Evaluate the inductive step to detect cycles.
                    // Overriding BaseType getter from concrete type implementing IEdmComplexType will be invoked to
                    // detect cycles. The object assignment is required by compiler only.
                    IEdmStructuredType baseType2 = baseType.BaseType;
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
