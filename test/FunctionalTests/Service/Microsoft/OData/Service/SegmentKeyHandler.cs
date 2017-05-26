//---------------------------------------------------------------------
// <copyright file="SegmentKeyHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CORE
namespace Microsoft.OData.Core.Query
#else
namespace Microsoft.OData.Service
#endif
{
    #region Namespaces
#if !ODATA_CORE
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
#endif
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.OData.Edm;
#if ODATA_CORE
    using Microsoft.OData.Core.Evaluation;
    using ErrorStrings = Microsoft.OData.Core.Strings;
    using ExceptionUtil = ODataUriParserException;
    using VersionUtil = TemporaryVersionUtil;
    using WebUtil = TemporaryWebUtil;
#else
    using Microsoft.OData.Core;
    using ErrorStrings = Microsoft.OData.Service.Strings;
    using ExceptionUtil = DataServiceException;
#endif
    #endregion Namespaces

    /// <summary>
    /// Component for handling key expressions in URIs.
    /// </summary>
    internal static class SegmentKeyHandler
    {
        /// <summary>Tries to create a key segment for the given filter if it is non empty.</summary>
        /// <param name="previous">Segment on which to compose.</param>
        /// <param name="filter">Filter portion of segment, possibly null.</param>
        /// <param name="keySegment">The key segment that was created if the key was non-empty.</param>
        /// <returns>Whether the key was non-empty.</returns>
        internal static bool TryCreateKeySegmentFromParentheses(IPathSegment previous, string filter, out IPathSegment keySegment)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(filter != null, "filter != null");
            Debug.Assert(previous != null, "segment!= null");

            WebUtil.CheckSyntaxValid(!previous.SingleResult);

            KeyInstance key;
            WebUtil.CheckSyntaxValid(KeyInstance.TryParseKeysFromUri(filter, out key));

            if (key.IsEmpty)
            {
                keySegment = null;
                return false;
            }

            keySegment = CreateKeySegment(previous, key);
            return true;
        }

        /// <summary>
        /// Tries to handle the current segment as a key property value.
        /// </summary>
        /// <param name="segmentText">The segment text.</param>
        /// <param name="previous">The previous segment.</param>
        /// <param name="urlConvention">The current url convention for the server.</param>
        /// <param name="keySegment">The key segment that was created if the segment could be interpreted as a key.</param>
        /// <returns>Whether or not the segment was interpreted as a key.</returns>
        internal static bool TryHandleSegmentAsKey(string segmentText, IPathSegment previous, UrlConvention urlConvention, out IPathSegment keySegment)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(previous != null, "previous != null");
            Debug.Assert(urlConvention != null, "urlConvention != null");

            keySegment = null;

            // If the current convention is not keys-as-segments, then this does not apply.
            if (!urlConvention.GenerateKeyAsSegment)
            {
                return false;
            }

            // If the current identifier was preceeded by a '$' segment, then do not treat it as a key.
            if (previous.IsEscapeMarker)
            {
                return false;
            }

            // Keys only apply to collections, so if the prior segment is already a singleton, do not treat this segment as a key.
            if (previous.SingleResult)
            {
                return false;
            }

            // System segments (ie '$count') are never keys.
            if (IsSystemSegment(segmentText))
            {
                return false;
            }

            // If the previous type is not an entity collection type
            // TODO: collapse this and SingleResult.
            IEdmEntityType targetEntityType;
            if (previous.TargetEdmType == null || !previous.TargetEdmType.IsEntityOrEntityCollectionType(out targetEntityType))
            {
                return false;
            }

            // If the resource type has more than 1 key property, then do not treat the segment as a key.
            var keyProperties = targetEntityType.Key().ToList();
            if (keyProperties.Count > 1)
            {
                return false;
            }

            // At this point it must be treated as a key, so fail if it is malformed.
            Debug.Assert(keyProperties.Count == 1, "keyProperties.Count == 1");
            keySegment = CreateKeySegment(previous, KeyInstance.FromSegment(segmentText));

            return true;
        }

        /// <summary>
        /// Determines whether the segment text is a system-reserved identifier like $'count'.
        /// </summary>
        /// <param name="segmentText">The segment text.</param>
        /// <returns>
        ///   <c>true</c> if the segment text is a system-reserved identifier like $'count'; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSystemSegment(string segmentText)
        {
            Debug.Assert(!string.IsNullOrEmpty(segmentText), "!string.IsNullOrEmpty(segmentText)");

            // system segments must start with '$'
            if (segmentText[0] != '$')
            {
                return false;
            }
            
            // if the 2nd character is also a '$' then its an escaped key, not a system segment;
            return segmentText.Length < 2 || segmentText[1] != '$';
        }

        /// <summary>
        /// Parses the key properties based on the segment's target type, then creates a new segment for the key.
        /// </summary>
        /// <param name="segment">The segment to apply the key to.</param>
        /// <param name="key">The key to apply.</param>
        /// <returns>The newly created key segment.</returns>
        private static IPathSegment CreateKeySegment(IPathSegment segment, KeyInstance key)
        {
            Debug.Assert(segment != null, "segment != null");
            Debug.Assert(key != null && !key.IsEmpty, "key != null && !key.IsEmpty");
            Debug.Assert(segment.SingleResult == false, "segment.SingleResult == false");

            IEdmEntityType targetEntityType = null;
            WebUtil.CheckSyntaxValid(segment.TargetEdmType != null && segment.TargetEdmType.IsEntityOrEntityCollectionType(out targetEntityType));
            Debug.Assert(targetEntityType != null, "targetEntityType != null");

            // Make sure the keys specified in the uri matches with the number of keys in the metadata
            var keyProperties = targetEntityType.Key().ToList();
            if (keyProperties.Count != key.ValueCount)
            {
                throw ExceptionUtil.CreateBadRequestError(ErrorStrings.BadRequest_KeyCountMismatch(targetEntityType.FullName()));
            }

            if (!key.AreValuesNamed && key.ValueCount > 1)
            {
                throw ExceptionUtil.CreateBadRequestError(ErrorStrings.RequestUriProcessor_KeysMustBeNamed);
            }

            WebUtil.CheckSyntaxValid(key.TryConvertValues(keyProperties));

            return new PathSegment(segment)
            {
                Key = key,
                SingleResult = true,
            };
        }
    }
}
