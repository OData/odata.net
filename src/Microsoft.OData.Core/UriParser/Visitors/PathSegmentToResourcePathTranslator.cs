//---------------------------------------------------------------------
// <copyright file="PathSegmentToResourcePathTranslator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Metadata;
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Translator to translate query url path segments to strings.
    /// </summary>
    internal sealed class PathSegmentToResourcePathTranslator : PathSegmentTranslator<string>
    {
        /// <summary>
        /// Key Serializer for KeySegment
        /// </summary>
        private KeySerializer KeySerializer;

        /// <summary>
        /// Private constructor since the singleton instance is sufficient.
        /// </summary>
        /// <param name="odataUrlKeyDelimiter">Key delimiter used in url.</param>
        public PathSegmentToResourcePathTranslator(ODataUrlKeyDelimiter odataUrlKeyDelimiter)
        {
            Debug.Assert(odataUrlKeyDelimiter != null, "odataUrlKeyDelimiter != null");
            this.KeySerializer = KeySerializer.Create(odataUrlKeyDelimiter.EnableKeyAsSegment);
        }

        /// <summary>
        /// Translate a TypeSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer</returns>
        public override string Translate(TypeSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            IEdmType type = segment.EdmType;
            IEdmCollectionType collectionType = type as IEdmCollectionType;

            if (collectionType != null)
            {
                type = collectionType.ElementType.Definition;
            }

            return "/" + type.FullTypeName();
        }

        /// <summary>
        /// Translate a NavigationPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(NavigationPropertySegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return "/" + segment.NavigationProperty.Name;
        }

        /// <summary>
        /// Translate an EntitySetSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(EntitySetSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return "/" + segment.EntitySet.Name;
        }

        /// <summary>
        /// Translate an SingletonSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(SingletonSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return "/" + segment.Singleton.Name;
        }

        /// <summary>
        /// Translate a KeySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(KeySegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            List<KeyValuePair<string, object>> keys = segment.Keys.ToList();

            StringBuilder builder = new StringBuilder();
            this.KeySerializer.AppendKeyExpression(builder, new Collection<KeyValuePair<string, object>>(keys), p => p.Key, p => p.Value);

            return builder.ToString();
        }

        /// <summary>
        /// Translate a PropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(PropertySegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return "/" + segment.Property.Name;
        }

        /// <summary>
        /// Translate a OperationSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(OperationSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            string res = null;

            NodeToStringBuilder nodeToStringBuilder = new NodeToStringBuilder();
            foreach (OperationSegmentParameter operationSegmentParameter in segment.Parameters)
            {
                string tmp = nodeToStringBuilder.TranslateNode((QueryNode)operationSegmentParameter.Value);
                res = String.Concat(res, String.IsNullOrEmpty(res) ? null : ExpressionConstants.SymbolComma, operationSegmentParameter.Name, ExpressionConstants.SymbolEqual, tmp);
            }

            return String.IsNullOrEmpty(res) ? String.Concat("/", segment.Operations.OperationGroupFullName()) : String.Concat("/", segment.Operations.OperationGroupFullName(), ExpressionConstants.SymbolOpenParen, res, ExpressionConstants.SymbolClosedParen);
        }

        /// <summary>
        /// Translate a OperationImportSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(OperationImportSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            NodeToStringBuilder nodeToStringBuilder = new NodeToStringBuilder();
            string res = null;
            foreach (OperationSegmentParameter operationSegmentParameter in segment.Parameters)
            {
                string tmp = nodeToStringBuilder.TranslateNode((QueryNode)operationSegmentParameter.Value);
                res = String.Concat(res, String.IsNullOrEmpty(res) ? null : ExpressionConstants.SymbolComma, operationSegmentParameter.Name, ExpressionConstants.SymbolEqual, tmp);
            }

            return String.IsNullOrEmpty(res) ? String.Concat("/", segment.Identifier) : String.Concat("/", segment.Identifier, ExpressionConstants.SymbolOpenParen, res, ExpressionConstants.SymbolClosedParen);
        }

        /// <summary>
        /// Translate an OpenPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(DynamicPathSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return "/" + segment.Identifier;
        }

        /// <summary>
        /// Translate a CountSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(CountSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return "/" + segment.Identifier;
        }

        /// <summary>
        /// Visit a NavigationPropertyLinkSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(NavigationPropertyLinkSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return String.Concat("/", segment.NavigationProperty.Name, "/", UriQueryConstants.RefSegment);
        }

        /// <summary>
        /// Translate a ValueSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(ValueSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return "/" + segment.Identifier;
        }

        /// <summary>
        /// Translate a BatchSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(BatchSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return "/" + segment.Identifier;
        }

        /// <summary>
        /// Translate a BatchReferenceSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(BatchReferenceSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return "/" + segment.ContentId;
        }

        /// <summary>
        /// Translate a MetadataSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(MetadataSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return "/" + segment.Identifier;
        }
    }
}