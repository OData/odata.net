//---------------------------------------------------------------------
// <copyright file="PathSegmentToResourcePathTranslator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.UriHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    /// <summary>
    /// Translator to translate segments to strings.
    /// </summary>
    internal class PathSegmentToResourcePathTranslator : PathSegmentTranslator<String>
    {
        private static PathSegmentToResourcePathTranslator instance;

        public static PathSegmentToResourcePathTranslator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PathSegmentToResourcePathTranslator();
                }

                return instance;
            }
        }

        /// <summary>
        /// Translate a TypeSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer</returns>
        public override string Translate(TypeSegment segment)
        {
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
            builder.Append("(");

            for (int i = 0; i < keys.Count; i++)
            {
                if (i != 0)
                {
                    builder.Append(",");
                }

                if (keys.Count == 1)
                {
                    builder.Append(ODataUriUtils.ConvertToUriLiteral(keys[i].Value, ODataVersion.V4, DataSourceManager.GetCurrentDataSource().Model));
                }
                else
                {
                    builder.Append(string.Format("{0}={1}", keys[i].Key, ODataUriUtils.ConvertToUriLiteral(keys[i].Value, ODataVersion.V4, DataSourceManager.GetCurrentDataSource().Model)));
                }
            }

            builder.Append(")");

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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a OperationSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(OperationImportSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate an OpenPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(DynamicPathSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a CountSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(CountSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a NavigationPropertyLinkSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(NavigationPropertyLinkSegment segment)
        {
            return "/" + segment.NavigationProperty.Name;
        }

        /// <summary>
        /// Translate a ValueSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(ValueSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a BatchSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(BatchSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a BatchReferenceSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(BatchReferenceSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a MetadataSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override string Translate(MetadataSegment segment)
        {
            throw new NotImplementedException();
        }
    }
}