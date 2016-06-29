//---------------------------------------------------------------------
// <copyright file="SegmentKeyHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using ErrorStrings = Microsoft.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// Component for handling key expressions in URIs.
    /// </summary>
    internal static class SegmentKeyHandler
    {
        /// <summary>Tries to create a key segment for the given filter if it is non empty.</summary>
        /// <param name="previous">Segment on which to compose.</param>
        /// <param name="previousKeySegment">The parent node's key segment.</param>
        /// <param name="parenthesisExpression">Parenthesis expression of segment.</param>
        /// <param name="resolver">The resolver to use.</param>
        /// <param name="keySegment">The key segment that was created if the key was non-empty.</param>
        /// <param name="enableUriTemplateParsing">Whether Uri template parsing is enabled.</param>
        /// <returns>Whether the key was non-empty.</returns>
        internal static bool TryCreateKeySegmentFromParentheses(ODataPathSegment previous, KeySegment previousKeySegment, string parenthesisExpression, ODataUriResolver resolver, out ODataPathSegment keySegment, bool enableUriTemplateParsing = false)
        {
            Debug.Assert(parenthesisExpression != null, "parenthesisExpression != null");
            Debug.Assert(previous != null, "segment!= null");
            Debug.Assert(resolver != null, "resolver != null");

            if (previous.SingleResult)
            {
                throw ExceptionUtil.CreateSyntaxError();
            }

            SegmentArgumentParser key;
            if (!SegmentArgumentParser.TryParseKeysFromUri(parenthesisExpression, out key, enableUriTemplateParsing))
            {
                throw ExceptionUtil.CreateSyntaxError();
            }

            // People/NS.Employees() is OK, just like People() is OK
            if (key.IsEmpty)
            {
                keySegment = null;
                return false;
            }

            keySegment = CreateKeySegment(previous, previousKeySegment, key, resolver);
            return true;
        }

        /// <summary>
        /// Tries to handle the current segment as a key property value.
        /// </summary>
        /// <param name="segmentText">The segment text.</param>
        /// <param name="previous">The previous segment.</param>
        /// <param name="previousKeySegment">The parent node's key segment.</param>
        /// <param name="odataUrlKeyDelimiter">Key delimiter used in url.</param>
        /// <param name="resolver">The resolver to use.</param>
        /// <param name="keySegment">The key segment that was created if the segment could be interpreted as a key.</param>
        /// <param name="enableUriTemplateParsing">Whether Uri template parsing is enabled.</param>
        /// <returns>Whether or not the segment was interpreted as a key.</returns>
        internal static bool TryHandleSegmentAsKey(string segmentText, ODataPathSegment previous, KeySegment previousKeySegment, ODataUrlKeyDelimiter odataUrlKeyDelimiter, ODataUriResolver resolver, out KeySegment keySegment, bool enableUriTemplateParsing = false)
        {
            Debug.Assert(previous != null, "previous != null");
            Debug.Assert(odataUrlKeyDelimiter != null, "odataUrlKeyDelimiter != null");
            Debug.Assert(resolver != null, "resolver != null");

            keySegment = null;

            // If the current convention does not support keys-as-segments, then this does not apply.
            if (!odataUrlKeyDelimiter.EnableKeyAsSegment)
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

            // Previously KeyAsSegment only allows single key, but we can also leverage related key finder to auto fill
            // missed key value from referential constraint information, which would be done in CreateKeySegment.
            // CreateKeySegment method will check whether key properties are missing after taking in related key values.
            keySegment = CreateKeySegment(previous, previousKeySegment, SegmentArgumentParser.FromSegment(segmentText, enableUriTemplateParsing), resolver);

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
        /// <param name="previousKeySegment">The parent node's key segment.</param>
        /// <param name="key">The key to apply.</param>
        /// <param name="resolver">The resolver to use.</param>
        /// <returns>The newly created key segment.</returns>
        private static KeySegment CreateKeySegment(ODataPathSegment segment, KeySegment previousKeySegment, SegmentArgumentParser key, ODataUriResolver resolver)
        {
            Debug.Assert(segment != null, "segment != null");
            Debug.Assert(key != null && !key.IsEmpty, "key != null && !key.IsEmpty");
            Debug.Assert(segment.SingleResult == false, "segment.SingleResult == false");

            IEdmEntityType targetEntityType = null;
            if (!(segment.TargetEdmType != null && segment.TargetEdmType.IsEntityOrEntityCollectionType(out targetEntityType)))
            {
                throw ExceptionUtil.CreateSyntaxError();
            }

            Debug.Assert(targetEntityType != null, "targetEntityType != null");

            // Make sure the keys specified in the uri matches with the number of keys in the metadata
            var keyProperties = targetEntityType.Key().ToList();
            if (keyProperties.Count != key.ValueCount)
            {
                NavigationPropertySegment currentNavPropSegment = segment as NavigationPropertySegment;
                if (currentNavPropSegment != null)
                {
                    key = KeyFinder.FindAndUseKeysFromRelatedSegment(key, keyProperties, currentNavPropSegment.NavigationProperty, previousKeySegment);
                }

                // if we still didn't find any keys, then throw an error.
                if (keyProperties.Count != key.ValueCount && resolver.GetType() == typeof(ODataUriResolver))
                {
                    throw ExceptionUtil.CreateBadRequestError(ErrorStrings.BadRequest_KeyCountMismatch(targetEntityType.FullName()));
                }
            }

            if (!key.AreValuesNamed && key.ValueCount > 1 && resolver.GetType() == typeof(ODataUriResolver))
            {
                throw ExceptionUtil.CreateBadRequestError(ErrorStrings.RequestUriProcessor_KeysMustBeNamed);
            }

            IEnumerable<KeyValuePair<string, object>> keyPairs;
            if (!key.TryConvertValues(targetEntityType, out keyPairs, resolver))
            {
                throw ExceptionUtil.CreateSyntaxError();
            }

            return new KeySegment(segment, keyPairs, targetEntityType, segment.TargetEdmNavigationSource);
        }
    }
}
