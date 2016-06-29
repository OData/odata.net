//---------------------------------------------------------------------
// <copyright file="PathSegmentToContextUrlPathTranslator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Evaluation;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Translator to translate context url path segments to strings.
    /// </summary>
    internal sealed class PathSegmentToContextUrlPathTranslator : PathSegmentTranslator<String>
    {
        /// <summary>
        /// Default instance of the translator.
        /// </summary>
        internal static readonly PathSegmentToContextUrlPathTranslator DefaultInstance = new PathSegmentToContextUrlPathTranslator(false);

        /// <summary>
        /// Instance of the translator that use key as segment in path.
        /// </summary>
        internal static readonly PathSegmentToContextUrlPathTranslator KeyAsSegmentInstance = new PathSegmentToContextUrlPathTranslator(true);

        /// <summary>
        /// Key Serializer for KeySegment
        /// </summary>
        private KeySerializer KeySerializer;

        /// <summary>
        /// Private constructor since the singleton instance is sufficient.
        /// </summary>
        /// <param name="keyAsSegment">Whether use key as segment</param>
        private PathSegmentToContextUrlPathTranslator(bool keyAsSegment)
        {
            this.KeySerializer = KeySerializer.Create(keyAsSegment);
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
            KeySerializer.AppendKeyExpression(builder, new Collection<KeyValuePair<string, object>>(keys), p => p.Key, p => p.Value);

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
            return "/" + segment.Operations.OperationGroupFullName();
        }

        /// <summary>
        /// Translate a OperationSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(OperationImportSegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return string.Empty;
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
            return string.Empty;
        }

        /// <summary>
        /// Visit a NavigationPropertyLinkSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(NavigationPropertyLinkSegment segment)
        {
            return string.Empty;
        }

        /// <summary>
        /// Translate a ValueSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(ValueSegment segment)
        {
            return string.Empty;
        }

        /// <summary>
        /// Translate a BatchSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(BatchSegment segment)
        {
            return string.Empty;
        }

        /// <summary>
        /// Translate a BatchReferenceSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(BatchReferenceSegment segment)
        {
            return string.Empty;
        }

        /// <summary>
        /// Translate a MetadataSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(MetadataSegment segment)
        {
            return string.Empty;
        }
    }
}