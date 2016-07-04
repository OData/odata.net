//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsTypeDefinitionReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides the semantics of a reference to an EDM type definition.
    /// </summary>
    internal class CsdlSemanticsTypeDefinitionReference : CsdlSemanticsNamedTypeReference, IEdmTypeDefinitionReference
    {
        private static readonly Func<CsdlSemanticsTypeDefinitionReference, bool> ComputeIsUnboundedFunc = me => me.ComputeIsUnbounded();
        private static readonly Func<CsdlSemanticsTypeDefinitionReference, int?> ComputeMaxLengthFunc = me => me.ComputeMaxLength();
        private static readonly Func<CsdlSemanticsTypeDefinitionReference, bool?> ComputeIsUnicodeFunc = me => me.ComputeIsUnicode();
        private static readonly Func<CsdlSemanticsTypeDefinitionReference, int?> ComputePrecisionFunc = me => me.ComputePrecision();
        private static readonly Func<CsdlSemanticsTypeDefinitionReference, int?> ComputeScaleFunc = me => me.ComputeScale();
        private static readonly Func<CsdlSemanticsTypeDefinitionReference, int?> ComputeSridFunc = me => me.ComputeSrid();

        private readonly Cache<CsdlSemanticsTypeDefinitionReference, bool> isUnboundedCache = new Cache<CsdlSemanticsTypeDefinitionReference, bool>();
        private readonly Cache<CsdlSemanticsTypeDefinitionReference, int?> maxLengthCache = new Cache<CsdlSemanticsTypeDefinitionReference, int?>();
        private readonly Cache<CsdlSemanticsTypeDefinitionReference, bool?> isUnicodeCache = new Cache<CsdlSemanticsTypeDefinitionReference, bool?>();
        private readonly Cache<CsdlSemanticsTypeDefinitionReference, int?> precisionCache = new Cache<CsdlSemanticsTypeDefinitionReference, int?>();
        private readonly Cache<CsdlSemanticsTypeDefinitionReference, int?> scaleCache = new Cache<CsdlSemanticsTypeDefinitionReference, int?>();
        private readonly Cache<CsdlSemanticsTypeDefinitionReference, int?> sridCache = new Cache<CsdlSemanticsTypeDefinitionReference, int?>();

        public CsdlSemanticsTypeDefinitionReference(CsdlSemanticsSchema schema, CsdlNamedTypeReference reference)
            : base(schema, reference)
        {
        }

        public bool IsUnbounded
        {
            get { return this.isUnboundedCache.GetValue(this, ComputeIsUnboundedFunc, null); }
        }

        public int? MaxLength
        {
            get { return this.maxLengthCache.GetValue(this, ComputeMaxLengthFunc, null); }
        }

        public bool? IsUnicode
        {
            get { return this.isUnicodeCache.GetValue(this, ComputeIsUnicodeFunc, null); }
        }

        public int? Precision
        {
            get { return this.precisionCache.GetValue(this, ComputePrecisionFunc, null); }
        }

        public int? Scale
        {
            get { return this.scaleCache.GetValue(this, ComputeScaleFunc, null); }
        }

        public int? SpatialReferenceIdentifier
        {
            get { return this.sridCache.GetValue(this, ComputeSridFunc, null); }
        }

        private CsdlNamedTypeReference Reference
        {
            get { return (CsdlNamedTypeReference)this.Element; }
        }

        private bool ComputeIsUnbounded()
        {
            return this.UnderlyingType().CanSpecifyMaxLength() && this.Reference.IsUnbounded;
        }

        private int? ComputeMaxLength()
        {
            return this.UnderlyingType().CanSpecifyMaxLength() ? this.Reference.MaxLength : null;
        }

        private bool? ComputeIsUnicode()
        {
            return this.UnderlyingType().IsString() ? this.Reference.IsUnicode : null;
        }

        private int? ComputePrecision()
        {
            if (this.UnderlyingType().IsDecimal())
            {
                return this.Reference.Precision;
            }

            if (this.UnderlyingType().IsTemporal())
            {
                return this.Reference.Precision ?? CsdlConstants.Default_TemporalPrecision;
            }

            return null;
        }

        private int? ComputeScale()
        {
            return this.UnderlyingType().IsDecimal() ? this.Reference.Scale : null;
        }

        private int? ComputeSrid()
        {
            if (this.UnderlyingType().IsGeography())
            {
                return DefaultSridIfUnspecified(CsdlConstants.Default_SpatialGeographySrid);
            }

            if (this.UnderlyingType().IsGeometry())
            {
                return DefaultSridIfUnspecified(CsdlConstants.Default_SpatialGeometrySrid);
            }

            return null;
        }

        private int? DefaultSridIfUnspecified(int defaultSrid)
        {
            if (this.Reference.SpatialReferenceIdentifier != CsdlConstants.Default_UnspecifiedSrid)
            {
                // The SRID is specified.
                return this.Reference.SpatialReferenceIdentifier;
            }

            return defaultSrid;
        }
    }
}
