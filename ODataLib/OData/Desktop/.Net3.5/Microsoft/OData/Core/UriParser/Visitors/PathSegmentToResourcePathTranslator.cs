//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if !INTERNAL_DROP || ODATALIB

namespace Microsoft.OData.Core.UriParser.Visitors
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Translator to translate segments to strings.
    /// </summary>
    internal sealed class PathSegmentToResourcePathTranslator : PathSegmentTranslator<String>
    {
        /// <summary>
        /// Default instance of the translator.
        /// </summary>
        internal static readonly PathSegmentToResourcePathTranslator DefaultInstance = new PathSegmentToResourcePathTranslator(false);

        /// <summary>
        /// Instance of the translator that use key as segment in path.
        /// </summary>
        internal static readonly PathSegmentToResourcePathTranslator KeyAsSegmentInstance = new PathSegmentToResourcePathTranslator(true);

        /// <summary>
        /// Key Serializer for KeySegment
        /// </summary>
        private KeySerializer KeySerializer;

        /// <summary>
        /// Private constructor since the singleton instance is sufficient.
        /// </summary>
        /// <param name="keyAsSegment">Whether use key as segment</param>
        private PathSegmentToResourcePathTranslator(bool keyAsSegment)
        {
            KeySerializer = KeySerializer.Create(UrlConvention.CreateWithExplicitValue(keyAsSegment));
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

            return "/" + type.ODataFullName();
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
        public override string Translate(OpenPropertySegment segment)
        {
            Debug.Assert(segment != null, "segment != null");
            return "/" + segment.PropertyName;
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

#endif
