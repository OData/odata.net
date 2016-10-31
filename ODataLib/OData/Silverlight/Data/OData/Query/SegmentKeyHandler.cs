//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Evaluation;
    using Microsoft.Data.OData.Query.SemanticAst;
    using ErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Component for handling key expressions in URIs.
    /// </summary>
    internal static class SegmentKeyHandler
    {
        /// <summary>Tries to create a key segment for the given filter if it is non empty.</summary>
        /// <param name="previous">Segment on which to compose.</param>
        /// <param name="parenthesisExpression">Parenthesis expression of segment.</param>
        /// <param name="keySegment">The key segment that was created if the key was non-empty.</param>
        /// <returns>Whether the key was non-empty.</returns>
        internal static bool TryCreateKeySegmentFromParentheses(ODataPathSegment previous, string parenthesisExpression, out ODataPathSegment keySegment)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(parenthesisExpression != null, "parenthesisExpression != null");
            Debug.Assert(previous != null, "segment!= null");

            ExceptionUtil.ThrowSyntaxErrorIfNotValid(!previous.SingleResult);

            SegmentArgumentParser key;
            ExceptionUtil.ThrowSyntaxErrorIfNotValid(SegmentArgumentParser.TryParseKeysFromUri(parenthesisExpression, out key));

            // People/NS.Employees() is OK, just like People() is OK
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
        internal static bool TryHandleSegmentAsKey(string segmentText, ODataPathSegment previous, UrlConvention urlConvention, out KeySegment keySegment)
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
            keySegment = CreateKeySegment(previous, SegmentArgumentParser.FromSegment(segmentText));

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
        private static KeySegment CreateKeySegment(ODataPathSegment segment, SegmentArgumentParser key)
        {
            Debug.Assert(segment != null, "segment != null");
            Debug.Assert(key != null && !key.IsEmpty, "key != null && !key.IsEmpty");
            Debug.Assert(segment.SingleResult == false, "segment.SingleResult == false");

            IEdmEntityType targetEntityType = null;
            ExceptionUtil.ThrowSyntaxErrorIfNotValid(segment.TargetEdmType != null && segment.TargetEdmType.IsEntityOrEntityCollectionType(out targetEntityType));
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

            IEnumerable<KeyValuePair<string, object>> keyPairs;
            ExceptionUtil.ThrowSyntaxErrorIfNotValid(key.TryConvertValues(keyProperties, out keyPairs));

            IEdmEntityType entityType;
            bool isEntity = segment.TargetEdmType.IsEntityOrEntityCollectionType(out entityType);
            Debug.Assert(isEntity, "Key target type should be an entity type.");

            var keySegment = new KeySegment(keyPairs, entityType, segment.TargetEdmEntitySet);
            keySegment.CopyValuesFrom(segment);
            keySegment.SingleResult = true;

            return keySegment;
        }
    }
}
