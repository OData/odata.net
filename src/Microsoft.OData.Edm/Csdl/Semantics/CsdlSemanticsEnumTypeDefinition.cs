//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsEnumTypeDefinition.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlEnumType.
    /// </summary>
    internal class CsdlSemanticsEnumTypeDefinition : CsdlSemanticsTypeDefinition, IEdmEnumType
    {
        private readonly CsdlEnumType enumeration;

        private readonly Cache<CsdlSemanticsEnumTypeDefinition, IEdmPrimitiveType> underlyingTypeCache = new Cache<CsdlSemanticsEnumTypeDefinition, IEdmPrimitiveType>();
        private static readonly Func<CsdlSemanticsEnumTypeDefinition, IEdmPrimitiveType> ComputeUnderlyingTypeFunc = (me) => me.ComputeUnderlyingType();

        private readonly Cache<CsdlSemanticsEnumTypeDefinition, IEnumerable<IEdmEnumMember>> membersCache = new Cache<CsdlSemanticsEnumTypeDefinition, IEnumerable<IEdmEnumMember>>();
        private static readonly Func<CsdlSemanticsEnumTypeDefinition, IEnumerable<IEdmEnumMember>> ComputeMembersFunc = (me) => me.ComputeMembers();

        public CsdlSemanticsEnumTypeDefinition(CsdlSemanticsSchema context, CsdlEnumType enumeration)
            : base(enumeration)
        {
            this.Context = context;
            this.enumeration = enumeration;
        }

        IEdmPrimitiveType IEdmEnumType.UnderlyingType
        {
            get { return this.underlyingTypeCache.GetValue(this, ComputeUnderlyingTypeFunc, null); }
        }

        public IEnumerable<IEdmEnumMember> Members
        {
            get { return this.membersCache.GetValue(this, ComputeMembersFunc, null); }
        }

        bool IEdmEnumType.IsFlags
        {
            get { return this.enumeration.IsFlags; }
        }

        EdmSchemaElementKind IEdmSchemaElement.SchemaElementKind
        {
            get { return EdmSchemaElementKind.TypeDefinition; }
        }

        public string Namespace
        {
            get { return this.Context.Namespace; }
        }

        string IEdmNamedElement.Name
        {
            get { return this.enumeration.Name; }
        }

        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Enum; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.Context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.enumeration; }
        }

        public CsdlSemanticsSchema Context
        {
            get;
            private set;
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.Context);
        }

        private IEdmPrimitiveType ComputeUnderlyingType()
        {
            if (this.enumeration.UnderlyingTypeName != null)
            {
                var underlyingTypeKind = EdmCoreModel.Instance.GetPrimitiveTypeKind(this.enumeration.UnderlyingTypeName);
                return underlyingTypeKind != EdmPrimitiveTypeKind.None ?
                    EdmCoreModel.Instance.GetPrimitiveType(underlyingTypeKind) :
                    new UnresolvedPrimitiveType(this.enumeration.UnderlyingTypeName, this.Location);
            }

            return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
        }

        private IEnumerable<IEdmEnumMember> ComputeMembers()
        {
            var members = new List<IEdmEnumMember>();

            // Walk the members and assign implicit values where needed.
            long previousValue = -1;
            foreach (CsdlEnumMember member in this.enumeration.Members)
            {
                IEdmEnumMember semanticsMember;
                long? implicitValue = null;
                if (!member.Value.HasValue)
                {
                    if (previousValue < long.MaxValue)
                    {
                        implicitValue = previousValue + 1;
                        previousValue = implicitValue.Value;
                        member.Value = implicitValue;
                        semanticsMember = new CsdlSemanticsEnumMember(this, member);
                    }
                    else
                    {
                        semanticsMember = new CsdlSemanticsEnumMember(this, member);
                    }

                    semanticsMember.SetIsValueExplicit(this.Model, false);
                }
                else
                {
                    previousValue = member.Value.Value;
                    semanticsMember = new CsdlSemanticsEnumMember(this, member);
                    semanticsMember.SetIsValueExplicit(this.Model, true);
                }

                members.Add(semanticsMember);
            }

            return members;
        }
    }
}
