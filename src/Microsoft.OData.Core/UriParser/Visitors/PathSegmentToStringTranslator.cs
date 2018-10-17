//---------------------------------------------------------------------
// <copyright file="PathSegmentToStringTranslator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Translator to translate segments to strings.
    /// </summary>
    internal sealed class PathSegmentToStringTranslator : PathSegmentTranslator<String>
    {
        /// <summary>
        /// Singleton instance of the translator.
        /// </summary>
        internal static readonly PathSegmentToStringTranslator Instance = new PathSegmentToStringTranslator();

        /// <summary>
        /// Private constructor since the singleton instance is sufficient.
        /// </summary>
        private PathSegmentToStringTranslator()
        {
        }

        /// <summary>
        /// Translate a TypeSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer</returns>
        public override string Translate(TypeSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return segment.EdmType.FullTypeName();
        }

        /// <summary>
        /// Translate a NavigationPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(NavigationPropertySegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return segment.NavigationProperty.Name;
        }

        /// <summary>
        /// Translate a PropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(PropertySegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return segment.Property.Name;
        }

        /// <summary>
        /// Translate an AnnotationSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(AnnotationSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return segment.Term.FullName();
        }

        /// <summary>
        /// Translate an OperationSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(OperationSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return segment.Operations.OperationGroupFullName();
        }

        /// <summary>
        /// Translate an OperationImportSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(OperationImportSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return segment.OperationImports.OperationImportGroupFullName();
        }

        /// <summary>
        /// Translate an OpenPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(DynamicPathSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return segment.Identifier;
        }

        /// <summary>
        /// Translate a FilterSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(FilterSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return segment.LiteralText;
        }

        /// <summary>
        /// Translate a ReferenceSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(ReferenceSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return segment.Identifier;
        }

        /// <summary>
        /// Translate an EachSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(EachSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return segment.Identifier;
        }

        /// <summary>
        /// Translate a PathTemplateSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(PathTemplateSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return segment.LiteralText;
        }
    }
}