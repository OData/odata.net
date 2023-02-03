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
    internal class CsdlSemanticsEntityTypeDefinition : CsdlSemanticsStructuredTypeDefinition, IEdmEntityType, IEdmFullNamedElement, IEdmKeyPropertyRef
    {
        private readonly CsdlEntityType entity;
        private readonly string fullName;

        private readonly Cache<CsdlSemanticsEntityTypeDefinition, IEdmEntityType> baseTypeCache = new Cache<CsdlSemanticsEntityTypeDefinition, IEdmEntityType>();
        private static readonly Func<CsdlSemanticsEntityTypeDefinition, IEdmEntityType> ComputeBaseTypeFunc = (me) => me.ComputeBaseType();
        private static readonly Func<CsdlSemanticsEntityTypeDefinition, IEdmEntityType> OnCycleBaseTypeFunc = (me) => new CyclicEntityType(me.GetCyclicBaseTypeName(me.entity.BaseTypeName), me.Location);

        private readonly Cache<CsdlSemanticsEntityTypeDefinition, IEnumerable<IEdmPropertyRef>> declaredKeyCache = new Cache<CsdlSemanticsEntityTypeDefinition, IEnumerable<IEdmPropertyRef>>();
        private static readonly Func<CsdlSemanticsEntityTypeDefinition, IEnumerable<IEdmPropertyRef>> ComputeDeclaredKeyFunc = (me) => me.ComputeDeclaredKey();

        public CsdlSemanticsEntityTypeDefinition(CsdlSemanticsSchema context, CsdlEntityType entity)
            : base(context, entity)
        {
            this.entity = entity;
            this.fullName = EdmUtil.GetFullNameForSchemaElement(context?.Namespace, this.entity?.Name);
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

        /// <summary>
        /// Gets the full name of this schema element.
        /// </summary>
        public string FullName
        {
            get { return this.fullName; }
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
                return this.DeclaredKeyRef?.Select(x => x.ReferencedProperty);
            }
        }

        public IEnumerable<IEdmPropertyRef> DeclaredKeyRef
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

        private IEnumerable<IEdmPropertyRef> ComputeDeclaredKey()
        {
            if (this.entity.Key != null)
            {
                List<IEdmPropertyRef> key = new List<IEdmPropertyRef>();
                foreach (CsdlPropertyReference keyProperty in this.entity.Key.Properties)
                {
                    IEdmStructuralProperty structuralProperty = FindKeyProperty(keyProperty.PropertyName);
                    if (structuralProperty != null)
                    {
                        key.Add(new EdmPropertyRef(structuralProperty, keyProperty.PropertyName, keyProperty.PropertyAlias));
                    }
                    else
                    {
                        key.Add(new UnresolvedPropertyRef(this, keyProperty.PropertyName, keyProperty.PropertyAlias, this.Location));
                    }
                }

                return key;
            }

            return null;
        }

        private IEdmStructuralProperty FindKeyProperty(string nameOrPath)
        {
            if (string.IsNullOrWhiteSpace(nameOrPath))
            {
                return null;
            }

            string[] segments = nameOrPath.Split('/');
            if (segments.Length == 1)
            {
                return this.FindProperty(nameOrPath) as IEdmStructuralProperty;
            }
            else
            {
                // OData spec says "The value of Name is a path expression leading to a primitive property."
                // The segment in a path expression could be single value property name, collection value property name, type cast, ...
                // However, for the key property reference path expression:
                // 1) Collection value property name segment is invalid, right?
                // 2) Type cast? reference a key on the sub type? it's valid but....
                // So far, let's skip those.
                IEdmStructuredType edmStructuredType = this;
                for (int i = 0; i < segments.Length; ++i)
                {
                    if (edmStructuredType == null)
                    {
                        return null;
                    }

                    string segment = segments[i];

                    if (segment.Contains("."))
                    {
                        // a type cast, let's skip it.
                        continue;
                    }

                    IEdmProperty edmProperty = FindPropertyOnType(edmStructuredType, segment);
                    if (i == segment.Length - 1)
                    {
                        return edmProperty as IEdmStructuralProperty;
                    }
                    else if (edmProperty != null)
                    {
                        // If the property is a collection value, let's move on using the element type of this collection
                        edmStructuredType = edmProperty.Type.ToStructuredType();
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        private static IEdmProperty FindPropertyOnType(IEdmStructuredType structuredType, string name)
        {
            IEdmProperty property = structuredType.FindProperty(name);

            if (property == null)
            {
                property = structuredType.DeclaredProperties.FirstOrDefault(p => p.Name == name);
            }

            return property;
        }
    }
}
