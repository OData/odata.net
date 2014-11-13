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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Evaluation;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Semantic parser for the path of the request URI.
    /// </summary>
    internal sealed class ODataPathParser
    {
        /// <summary>
        /// regex pattern to match a contentID
        /// </summary>
#if SILVERLIGHT || ORCAS || PORTABLELIB
        internal static readonly Regex ContentIdRegex = new Regex(@"^\$[0-9]+$", RegexOptions.Singleline); // RegexOptions.Compiled does not exist in Silverlight
#else
        internal static readonly Regex ContentIdRegex = new Regex(@"^\$[0-9]+$", RegexOptions.Singleline | RegexOptions.Compiled);
#endif
        /// <summary>
        /// The queue of segments remaining to be parsed. Should be populated and cleared out on each pass through the main path parsing loop.
        /// </summary>
        private readonly Queue<string> segmentQueue = new Queue<string>();

        /// <summary>
        /// The collection of segments that have been parsed so far.
        /// </summary>
        private readonly List<ODataPathSegment> parsedSegments = new List<ODataPathSegment>();

        /// <summary>
        /// The parser's current configuration.
        /// </summary>
        private readonly ODataUriParserConfiguration configuration;

        /// <summary>
        /// Indicates that the next segment encountered must refer to something in 'metadata-space' and cannot be a key expression.
        /// </summary>
        private bool nextSegmentMustReferToMetadata;

        /// <summary>
        /// Initializes a new instance of <see cref="ODataPathParser"/>.
        /// </summary>
        /// <param name="configuration">The parser's current configuration.</param>
        internal ODataPathParser(ODataUriParserConfiguration configuration)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(configuration != null, "configuration != null");

            this.configuration = configuration;
        }

        /// <summary>
        /// Extracts the segment identifier and, if there are parenthesis in the segment, the expression in the parenthesis.  
        /// Will throw if identifier is not found or if the parenthesis expression is malformed.
        /// </summary>
        /// <remarks>Internal only so it can be called from tests. Should not be used outside <see cref="ODataPathParser"/>.</remarks>
        /// <param name="segmentText">The segment text.</param>
        /// <param name="identifier">The identifier that was found.</param>
        /// <param name="parenthesisExpression">The query portion that was found. Will be null after the call if no query portion was present.</param>
        internal static void ExtractSegmentIdentifierAndParenthesisExpression(string segmentText, out string identifier, out string parenthesisExpression)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(segmentText != null, "segment != null");

            int parenthesisStart = segmentText.IndexOf('(');
            if (parenthesisStart < 0)
            {
                identifier = segmentText;
                parenthesisExpression = null;
            }
            else
            {
                ExceptionUtil.ThrowSyntaxErrorIfNotValid(segmentText[segmentText.Length - 1] == ')');

                // split the string to grab the identifier and remove the parentheses
                identifier = segmentText.Substring(0, parenthesisStart);
                parenthesisExpression = segmentText.Substring(parenthesisStart + 1, segmentText.Length - identifier.Length - 2);
            }

            // We allow a single trailing '/', which results in an empty segment.
            // However System.Uri removes it, so any empty segment we see is a 404 error.
            if (identifier.Length == 0)
            {
                throw ExceptionUtil.ResourceNotFoundError(ODataErrorStrings.RequestUriProcessor_EmptySegmentInRequestUrl);
            }
        }

        /// <summary>Creates an <see cref="ODataPathSegment"/> array for the given <paramref name="segments"/>.</summary>
        /// <param name="segments">Segments to process.</param>
        /// <returns>Segment information describing the given <paramref name="segments"/>.</returns>
        internal IList<ODataPathSegment> ParsePath(ICollection<string> segments)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(segments != null, "segments != null");
            Debug.Assert(this.parsedSegments.Count == 0, "Segment storage should be empty.");
            Debug.Assert(this.segmentQueue.Count == 0, "Segment queue should be empty.");

            // populate the queue that will be used to drive the rest of the algorithm.
            foreach (var segment in segments)
            {
                this.segmentQueue.Enqueue(segment);
            }
            
            string segmentText;
            while (this.TryGetNextSegmentText(out segmentText))
            {
                if (this.parsedSegments.Count == 0)
                {
                    this.CreateFirstSegment(segmentText);
                }
                else
                {
                    this.CreateNextSegment(segmentText);
                }
            }

            List<ODataPathSegment> validatedSegments = new List<ODataPathSegment>(this.parsedSegments.Count);
            foreach (var segment in this.parsedSegments)
            {
#if DEBUG
                segment.AssertValid();
#endif
                validatedSegments.Add(segment);
            }

            this.parsedSegments.Clear();
            return validatedSegments;
        }

        /// <summary>
        /// Tries to find a single matching function import for the given identifier, parametes, and binding type.
        /// </summary>
        /// <param name="identifier">The identifier from the URI.</param>
        /// <param name="parenthesisExpression">The parenthesis expression contianing parameters, if any.</param>
        /// <param name="bindingType">The current binding type or null if there isn't one.</param>
        /// <param name="configuration">The configuration of the parser.</param>
        /// <param name="parsedParameters">The parsed parameters from the parenthesis expression.</param>
        /// <param name="matchingFunctionImport">The single matching function import if one could be determined.</param>
        /// <returns>Whether or not a matching function import could be found.</returns>
        private static bool TryBindingParametersAndMatchingOperation(string identifier, string parenthesisExpression, IEdmType bindingType, ODataUriParserConfiguration configuration, out ICollection<OperationSegmentParameter> parsedParameters, out IEdmFunctionImport matchingFunctionImport)
        {
            matchingFunctionImport = null;
            ICollection<FunctionParameterToken> splitParameters;
            if (!String.IsNullOrEmpty(parenthesisExpression))
            {
                if (!FunctionParameterParser.TrySplitFunctionParameters(identifier, parenthesisExpression, out splitParameters))
                { 
                    // to maintain a nice exception message when an action is used with positional parameters.
                    var extendedModel = configuration.Model as IODataUriParserModelExtensions;
                    if (extendedModel != null && configuration.Settings.UseWcfDataServicesServerBehavior)
                    {
                        var specificFunctionImport = extendedModel.FindFunctionImportByBindingParameterType(bindingType, identifier, Enumerable.Empty<string>());
                        if (specificFunctionImport != null)
                        {
                            throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(identifier));
                        }
                    }

                    parsedParameters = null;
                    return false;
                }
            }
            else
            {
                splitParameters = new Collection<FunctionParameterToken>();
            }

            // Resolve the specific overload.
            if (FunctionOverloadResolver.ResolveFunctionsFromList(identifier, splitParameters.Select(k => k.ParameterName).ToList(), bindingType, configuration.Model, out matchingFunctionImport))
            {
                // Bind the actual parameter values to CLR objects
                if (FunctionParameterParser.TryParseFunctionParameters(splitParameters, configuration, matchingFunctionImport, out parsedParameters))
                {
                    return true;
                }
            }

            parsedParameters = null;
            return false;
        }

        /// <summary>Determines a matching target kind from the specified type.</summary>
        /// <param name="type">ResourceType of element to get kind for.</param>
        /// <returns>An appropriate <see cref="RequestTargetKind"/> for the specified <paramref name="type"/>.</returns>
        private static RequestTargetKind TargetKindFromType(IEdmType type)
        {
            Debug.Assert(type != null, "type != null");

            switch (type.TypeKind)
            {
                case EdmTypeKind.Complex:
                    return RequestTargetKind.ComplexObject;
                case EdmTypeKind.Entity:
                    return RequestTargetKind.Resource;
                case EdmTypeKind.Collection:
                    if (type.IsEntityOrEntityCollectionType())
                    {
                        return RequestTargetKind.Resource;
                    }

                    return RequestTargetKind.Collection;
                default:
                    Debug.Assert(type.TypeKind == EdmTypeKind.Primitive, "typeKind == ResourceTypeKind.Primitive");
                    return RequestTargetKind.Primitive;
            }
        }

        /// <summary>
        /// Checks for single result, otherwise throws.
        /// </summary>
        /// <param name="isSingleResult">indicates whether the current result is single result or not.</param>
        /// <param name="identifier">current segment identifier.</param>
        private static void CheckSingleResult(bool isSingleResult, string identifier)
        {
            if (!isSingleResult)
            {
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_CannotQueryCollections(identifier));
            }
        }

        /// <summary>
        /// Tries to get the next segment's text to parse.
        /// </summary>
        /// <param name="segmentText">The segment text to parse.</param>
        /// <returns>Whether there was a next segment.</returns>
        private bool TryGetNextSegmentText(out string segmentText)
        {
            return TryGetNextSegmentText(false, out segmentText);
        }

        /// <summary>
        /// Tries to get the next segment's text to parse. Should not be called except by the other overload which does not have the extra parameter.
        /// </summary>
        /// <param name="previousSegmentWasEscapeMarker">Whether the previous segment was an escape marker.</param>
        /// <param name="segmentText">The segment text to parse.</param>
        /// <returns>Whether there was a next segment.</returns>
        private bool TryGetNextSegmentText(bool previousSegmentWasEscapeMarker, out string segmentText)
        {
            if (this.segmentQueue.Count == 0)
            {
                segmentText = null;
                return false;
            }

            segmentText = this.segmentQueue.Dequeue();

            // If this segment is the special escape-marker segment, then remember that the next segment cannot be a key,
            // even if we are in key-as-segments mode. Essentially, it is an escape into 'metadata-space', so to speak.
            // 
            // DEVNOTE (mmeehan): We went back and forth several times on whether this should be allowed everywhere or only
            // where a key could appear. We landed on allowing it absolutely everywhere for several reasons:
            //   1) The WCF DS client naively adds the escape marker before all type segments, regardless of whether the 
            //      prior segment is a collection.
            //   2) The WCF DS server already allowed the escape marker almost everywhere in 5.3
            //   3) It's better to be either extremely loose or extremely strict than allow it in some cases and not in others.
            // Note that this is not publicly documented in OData V3 nor is it planned to be documented in OData V4, but it
            // is a part of supporting the Key-As-Segment conventions that are used by many Azure services.
            if (segmentText == "$")
            {
                this.nextSegmentMustReferToMetadata = true;
                return TryGetNextSegmentText(true, out segmentText);
            }
            
            if (!previousSegmentWasEscapeMarker)
            {
                this.nextSegmentMustReferToMetadata = false;
            }

            if (this.parsedSegments.Count > 0)
            {
                this.ThrowIfMustBeLeafSegment(this.parsedSegments[this.parsedSegments.Count - 1]);
            }

            return true;
        }

        /// <summary>
        /// Tries to handle the given text as a key if the URL conventions support it and it was not preceeded by an escape segment.
        /// </summary>
        /// <param name="segmentText">The text which might be a key.</param>
        /// <returns>Whether or not the text was handled as a key.</returns>
        private bool TryHandleAsKeySegment(string segmentText)
        {
            ODataPathSegment previous = this.parsedSegments[this.parsedSegments.Count - 1];

            KeySegment keySegment;
            if (!this.nextSegmentMustReferToMetadata && SegmentKeyHandler.TryHandleSegmentAsKey(segmentText, previous, this.configuration.UrlConventions.UrlConvention, out keySegment))
            {
                this.parsedSegments.Add(keySegment);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Throws if the given segment must be a leaf, as a later segment is being created.
        /// </summary>
        /// <param name="previous">The previous segment which may need to be a leaf.</param>
        private void ThrowIfMustBeLeafSegment(ODataPathSegment previous)
        {
            OperationSegment operationSegment = previous as OperationSegment;
            if (operationSegment != null)
            {
                foreach (var operation in operationSegment.Operations)
                {
                    bool isAction = this.configuration.Model.IsAction(operation);

                    if (isAction)
                    {
                        throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment(previous.Identifier));
                    }
                }
            }

            if (previous.TargetKind == RequestTargetKind.Batch 
                || previous.TargetKind == RequestTargetKind.Metadata
                || previous.TargetKind == RequestTargetKind.PrimitiveValue 
                || previous.TargetKind == RequestTargetKind.VoidOperation 
                || previous.TargetKind == RequestTargetKind.OpenPropertyValue 
                || previous.TargetKind == RequestTargetKind.MediaResource 
                || (previous.TargetKind == RequestTargetKind.Collection && operationSegment == null))
            {
                // Nothing can come after a $metadata, $value or $batch segment.
                // Nothing can come after a service operation with void return type.
                // Nothing can come after a collection property.
                throw ExceptionUtil.ResourceNotFoundError(ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment(previous.Identifier));
            }
        }

        /// <summary>
        /// Try to handle the segment as $count.
        /// </summary>
        /// <param name="segmentText">The segment text to handle.</param>
        /// <returns>Whether the segment was $count.</returns>
        private bool TryCreateCountSegment(string segmentText)
        {
            string identifier;
            string parenthesisExpression;
            ExtractSegmentIdentifierAndParenthesisExpression(segmentText, out identifier, out parenthesisExpression);

            if (identifier != UriQueryConstants.CountSegment)
            {
                return false;
            }

            // The server used to allow arbitrary key expressions after $count because this check was missing.
            ExceptionUtil.ThrowSyntaxErrorIfNotValid(parenthesisExpression == null || this.configuration.Settings.UseWcfDataServicesServerBehavior);

            ODataPathSegment previous = this.parsedSegments[this.parsedSegments.Count - 1];
            if (previous.TargetKind != RequestTargetKind.Resource)
            {
                throw ExceptionUtil.ResourceNotFoundError(ODataErrorStrings.RequestUriProcessor_CountNotSupported(previous.Identifier));
            }

            if (previous.SingleResult)
            {
                throw ExceptionUtil.ResourceNotFoundError(ODataErrorStrings.RequestUriProcessor_CannotQuerySingletons(previous.Identifier, UriQueryConstants.CountSegment));
            }

            this.parsedSegments.Add(CountSegment.Instance);
            return true;
        }

        /// <summary>
        /// Tries to handle the segment as $links. If it is $links, then the rest of the path will be parsed/validated in this call.
        /// </summary>
        /// <param name="text">The text of the segment.</param>
        /// <returns>Whether the text was $links.</returns>
        private bool TryCreateLinksSegment(string text)
        {
            string identifier;
            string parenthesisExpression;
            ExtractSegmentIdentifierAndParenthesisExpression(text, out identifier, out parenthesisExpression);

            if (identifier != UriQueryConstants.LinkSegment)
            {
                return false;
            }

            ExceptionUtil.ThrowSyntaxErrorIfNotValid(parenthesisExpression == null);

            ODataPathSegment previous = this.parsedSegments[this.parsedSegments.Count - 1];
            if (previous.TargetKind != RequestTargetKind.Resource)
            {
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.PathParser_LinksNotSupported(previous.Identifier));
            }

            CheckSingleResult(previous.SingleResult, previous.Identifier);

            string nextSegmentText;
            if (!this.TryGetNextSegmentText(out nextSegmentText))
            {
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_MissingSegmentAfterLink(UriQueryConstants.LinkSegment));
            }

            ExtractSegmentIdentifierAndParenthesisExpression(nextSegmentText, out identifier, out parenthesisExpression);

            IEdmProperty property;
            if (!this.TryBindProperty(identifier, out property))
            {
                // if its not an open type, this call will throw
                ExceptionUtil.ThrowIfResourceDoesNotExist(previous.TargetEdmType.IsOpenType(), identifier);
                
                // if it was open, then $links indicates this was meant to be an 'open navigation'.
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.OpenNavigationPropertiesNotSupportedOnOpenTypes(identifier));
            }

            var navigationProperty = property as IEdmNavigationProperty;
            if (navigationProperty == null)
            {
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_LinkSegmentMustBeFollowedByEntitySegment(property.Name, UriQueryConstants.LinkSegment));
            }

            var targetSet = previous.TargetEdmEntitySet.FindNavigationTarget(navigationProperty);

            // If we can't compute the target entity set, then pretend the navigation property does not exist
            if (targetSet == null)
            {
                throw ExceptionUtil.CreateResourceNotFound(property.Name);
            }

            this.parsedSegments.Add(new NavigationPropertyLinkSegment(navigationProperty, targetSet));

            this.TryBindKeyFromParentheses(parenthesisExpression);

            // if there is another segment, it must either be a key or $count.
            if (this.TryGetNextSegmentText(out nextSegmentText))
            {
                // DEVNOTE(pqian): [Resources]/$links/[Property]/$count is valid
                if (!this.TryHandleAsKeySegment(nextSegmentText) && !this.TryCreateCountSegment(nextSegmentText))
                {
                    throw ExceptionUtil.ResourceNotFoundError(ODataErrorStrings.RequestUriProcessor_CannotSpecifyAfterPostLinkSegment(nextSegmentText, UriQueryConstants.LinkSegment));
                }
            }

            // nothing else is allowed after $links.
            if (this.TryGetNextSegmentText(out nextSegmentText))
            {
                throw ExceptionUtil.ResourceNotFoundError(ODataErrorStrings.RequestUriProcessor_CannotSpecifyAfterPostLinkSegment(nextSegmentText, UriQueryConstants.LinkSegment));
            }

            return true;
        }

        /// <summary>
        /// Tries to bind a key from the parenthetical section of a segment.
        /// </summary>
        /// <param name="parenthesesSection">The section of the segment inside parentheses, or null if there was none.</param>
        private void TryBindKeyFromParentheses(string parenthesesSection)
        {
            if (parenthesesSection == null)
            {
                return;
            }

            ODataPathSegment keySegment;
            ODataPathSegment previous = this.parsedSegments[this.parsedSegments.Count - 1];
            if (!SegmentKeyHandler.TryCreateKeySegmentFromParentheses(previous, parenthesesSection, out keySegment))
            {
                return;
            }

            this.parsedSegments.Add(keySegment);
        }

        /// <summary>
        /// Try to handle the segment as $value.
        /// </summary>
        /// <param name="text">The segment text.</param>
        /// <returns>Whether the segment was $value.</returns>
        private bool TryCreateValueSegment(string text)
        {
            string identifier;
            string parenthesisExpression;
            ExtractSegmentIdentifierAndParenthesisExpression(text, out identifier, out parenthesisExpression);

            if (identifier != UriQueryConstants.ValueSegment)
            {
                return false;
            }

            ExceptionUtil.ThrowSyntaxErrorIfNotValid(parenthesisExpression == null);

            ODataPathSegment previous = this.parsedSegments[this.parsedSegments.Count - 1];

            ODataPathSegment segment = new ValueSegment(previous.EdmType);
            if (previous.TargetKind == RequestTargetKind.Primitive)
            {
                segment.CopyValuesFrom(previous);
            }
            else
            {
                segment.TargetEdmType = previous.TargetEdmType;
            }

            segment.Identifier = UriQueryConstants.ValueSegment;
            segment.SingleResult = true;
            CheckSingleResult(previous.SingleResult, previous.Identifier);

            if (previous.TargetKind == RequestTargetKind.Primitive)
            {
                segment.TargetKind = RequestTargetKind.PrimitiveValue;
                if (previous.TargetEdmType.IsSpatial())
                {
                    throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.BadRequest_ValuesCannotBeReturnedForSpatialTypes);
                }
            }
            else if (previous.TargetKind == RequestTargetKind.OpenProperty)
            {
                segment.TargetKind = RequestTargetKind.OpenPropertyValue;
            }
            else
            {
                // If the previous segment is an entity, we expect it to be an MLE. We cannot validate our assumption
                // until later when we get the actual instance of the entity because the type hierarchy can contain
                // a mix of MLE and non-MLE types.
                segment.TargetKind = RequestTargetKind.MediaResource;
            }

            this.parsedSegments.Add(segment);
            return true;
        }

        /// <summary>
        /// Creates a new segment for an open property.
        /// </summary>
        /// <param name="previous">previous segment info.</param>
        /// <param name="identifier">name of the segment.</param>
        /// <param name="parenthesisExpression">whether this segment has a query portion or not.</param>
        private void CreateOpenPropertySegment(ODataPathSegment previous, string identifier, string parenthesisExpression)
        {
            ODataPathSegment segment = new OpenPropertySegment(identifier);

            // Handle an open type property. If the current leaf isn't an 
            // object (which implies it's already an open type), then
            // it should be marked as an open type.
            if (previous.TargetEdmType != null)
            {
                ExceptionUtil.ThrowIfResourceDoesNotExist(previous.TargetEdmType.IsOpenType(), segment.Identifier);
            }

            // Open navigation properties are not supported on OpenTypes.
            if (parenthesisExpression != null)
            {
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.OpenNavigationPropertiesNotSupportedOnOpenTypes(segment.Identifier));
            }

            this.parsedSegments.Add(segment);
        }

        /// <summary>
        /// Creates a named stream segment
        /// </summary>
        /// <param name="previous">previous segment info.</param>
        /// <param name="streamProperty">stream property to create the segment for.</param>
        private void CreateNamedStreamSegment(ODataPathSegment previous, IEdmProperty streamProperty)
        {
            Debug.Assert(streamProperty.Type.IsStream(), "streamProperty.Type.IsStream()");

            // Handle Named Stream.
            ODataPathSegment segment = new PropertySegment((IEdmStructuralProperty)streamProperty);
            segment.TargetKind = RequestTargetKind.MediaResource;
            segment.SingleResult = true;
            segment.TargetEdmType = previous.TargetEdmType;
            Debug.Assert(segment.Identifier != UriQueryConstants.ValueSegment, "'$value' cannot be the name of a named stream.");

            this.parsedSegments.Add(segment);
        }

        /// <summary>Creates the first <see cref="ODataPathSegment"/> for a request.</summary>
        /// <param name="segmentText">The text of the segment.</param>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri parsing does not go through the same resolvers/settings that payload reading/writing does.")]
        private void CreateFirstSegment(string segmentText)
        {
            string identifier;
            string parenthesisExpression;
            ExtractSegmentIdentifierAndParenthesisExpression(segmentText, out identifier, out parenthesisExpression);

            Debug.Assert(identifier != null, "identifier != null");

            // Look for well-known system entry points.
            if (identifier == UriQueryConstants.MetadataSegment)
            {
                ExceptionUtil.ThrowSyntaxErrorIfNotValid(parenthesisExpression == null);
                this.parsedSegments.Add(MetadataSegment.Instance);
                return;
            }

            if (identifier == UriQueryConstants.BatchSegment)
            {
                ExceptionUtil.ThrowSyntaxErrorIfNotValid(parenthesisExpression == null);
                this.parsedSegments.Add(BatchSegment.Instance);
                return;
            }

            if (identifier == UriQueryConstants.CountSegment)
            {
                // $count on root: throw
                throw ExceptionUtil.CreateResourceNotFound(ODataErrorStrings.RequestUriProcessor_CountOnRoot);
            }

            // Look for a service operation.
            if (this.TryCreateSegmentForServiceOperation(identifier, parenthesisExpression))
            {
                return;
            }

            if (this.configuration.BatchReferenceCallback != null && ContentIdRegex.IsMatch(identifier))
            {
                ExceptionUtil.ThrowSyntaxErrorIfNotValid(parenthesisExpression == null);
                BatchReferenceSegment crossReferencedSegement = this.configuration.BatchReferenceCallback(identifier);
                if (crossReferencedSegement != null)
                {
                    this.parsedSegments.Add(crossReferencedSegement);
                    return;
                }
            }

            // Look for an entity set.
            IEdmEntitySet targetEdmEntitySet;
            var extendedModel = this.configuration.Model as IODataUriParserModelExtensions;
            if (extendedModel != null)
            {
                targetEdmEntitySet = extendedModel.FindEntitySetFromContainerQualifiedName(identifier);
            }
            else
            {
                targetEdmEntitySet = EdmLibraryExtensions.ResolveEntitySet(this.configuration.Model, identifier);
            }

            if (targetEdmEntitySet != null)
            {
                ODataPathSegment segment = new EntitySetSegment(targetEdmEntitySet)
                {
                    Identifier = identifier
                };
                
                this.parsedSegments.Add(segment);
                this.TryBindKeyFromParentheses(parenthesisExpression);
                
                return;
            }

            // Look for a service action.
            ExceptionUtil.ThrowIfResourceDoesNotExist(this.TryCreateSegmentForOperation(null /*previousSegment*/, identifier, parenthesisExpression), identifier);
        }

        /// <summary>
        /// Tries to parse the segment as a service operation
        /// </summary>
        /// <param name="identifier">The identifier for the segment.</param>
        /// <param name="queryPortion">The query portion</param>
        /// <returns>Whether or not the identifier referred to a service operation.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:Do not use string.format", Justification = "Will be removed when string freeze is over.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IEdmModel", Justification = "The spelling is correct.")]
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri parsing does not go through the same resolvers/settings that payload reading/writing does.")]
        private bool TryCreateSegmentForServiceOperation(string identifier, string queryPortion)
        {
            IEdmFunctionImport functionImport;
            var extendedModel = this.configuration.Model as IODataUriParserModelExtensions;
            if (extendedModel == null)
            {
                // If they don't have the extensions, we do a full search of the model
                var functionImports = this.configuration.Model.ResolveFunctionImports(identifier).Where(f => this.configuration.Model.IsServiceOperation(f)).ToList();

                if (functionImports.Count == 0)
                {
                    return false;
                }

                if (functionImports.Count == 1)
                {
                    functionImport = functionImports.Single();
                }
                else
                {
                    // TODO: This is really an error with the model, right?
                    throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.PathParser_ServiceOperationsWithSameName(identifier));
                }
            }
            else
            {
                // If they do, we let them to an optimized lookup
                functionImport = extendedModel.FindServiceOperation(identifier);
                if (functionImport == null)
                {
                    return false;
                }
            }

            var entitySet = functionImport.GetTargetEntitySet(null /*sourceEntitySet*/, this.configuration.Model);

            if (entitySet == null && functionImport.ReturnType != null)
            {
                if (functionImport.ReturnType.Definition.IsEntityOrEntityCollectionType())
                {
                    // Specifically not throwing ODataUriParserException since it's more an an internal server error
                    throw new ODataException(ODataErrorStrings.RequestUriProcessor_EntitySetNotSpecified(functionImport.Name));
                }
            }

            ODataPathSegment segment = new OperationSegment(functionImport, entitySet)
            {
                Identifier = identifier,
            };

            segment.TargetEdmEntitySet = entitySet;

            if (functionImport.ReturnType != null)
            {
                segment.TargetEdmType = functionImport.ReturnType.Definition;
                segment.TargetKind = TargetKindFromType(segment.TargetEdmType);

                // Whether the segment is a single result is based on the return type.
                segment.SingleResult = !functionImport.ReturnType.IsCollection();

                // All service operations can have '()', but only those that are collections can have an actual key value.
                if (segment.SingleResult && !String.IsNullOrEmpty(queryPortion))
                {
                    throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(segment.Identifier));
                }

                this.parsedSegments.Add(segment);

                if (!String.IsNullOrEmpty(queryPortion))
                {
                    this.TryBindKeyFromParentheses(queryPortion);
                }
            }
            else
            {
                // The server used to ignore the query portion for void service operations, but we want the ODL parser to fail by default.
                if (!String.IsNullOrEmpty(queryPortion) && !this.configuration.Settings.UseWcfDataServicesServerBehavior)
                {
                    throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(segment.Identifier));
                }

                segment.TargetEdmType = null;
                segment.TargetKind = RequestTargetKind.VoidOperation;
                this.parsedSegments.Add(segment);
            }

            return true;
        }

        /// <summary>
        /// Tries to parse a segment as a function or action.
        /// </summary>
        /// <param name="previousSegment">The previous segment before the operation to be invoked.</param>
        /// <param name="identifier">The name of the segment</param>
        /// <param name="parenthesisExpression">The query portion</param>
        /// <returns>Whether or not the identifier referred to an action.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:Do not use string.format", Justification = "Will be removed when string freeze is over.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IEdmModel", Justification = "The spelling is correct.")]
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri parsing does not go through the same resolvers/settings that payload reading/writing does.")]
        private bool TryCreateSegmentForOperation(ODataPathSegment previousSegment, string identifier, string parenthesisExpression)
        {
            // Parse Arguments syntactically
            var bindingType = previousSegment == null ? null : previousSegment.EdmType;

            ICollection<OperationSegmentParameter> resolvedParameters;
            IEdmFunctionImport singleImport;
            if (!TryBindingParametersAndMatchingOperation(identifier, parenthesisExpression, bindingType, this.configuration, out resolvedParameters, out singleImport))
            {
                return false;
            }

            if (!UriEdmHelpers.IsBindingTypeValid(bindingType))
            {
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_OperationSegmentBoundToANonEntityType);
            }

            if (previousSegment != null && bindingType == null)
            {
                throw new ODataException(ODataErrorStrings.FunctionCallBinder_CallingFunctionOnOpenProperty(identifier));
            }

            IEdmTypeReference returnType = singleImport.ReturnType;
            IEdmEntitySet targetset = null;

            if (returnType != null)
            {
                IEdmEntitySet sourceEntitySet = previousSegment == null ? null : previousSegment.TargetEdmEntitySet;
                targetset = singleImport.GetTargetEntitySet(sourceEntitySet, this.configuration.Model);
            } 
            
            // If previous segment is cross-referenced then we explicitly dissallow the service action call
            if (previousSegment is BatchReferenceSegment)
            {
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_BatchedActionOnEntityCreatedInSameChangeset(identifier));
            }

            // TODO: change constructor to take single import
            ODataPathSegment segment = new OperationSegment(new[] { singleImport }, resolvedParameters, targetset)
                {
                    Identifier = identifier
                };

            if (returnType != null)
            {
                segment.TargetEdmEntitySet = targetset;
                segment.TargetEdmType = returnType.Definition;
                segment.TargetKind = TargetKindFromType(segment.TargetEdmType);
                if (segment.TargetEdmType.IsEntityOrEntityCollectionType() && segment.TargetEdmEntitySet == null)
                {
                    // Specifically not throwing ODataUriParserException since it's more an an internal server error
                    throw new ODataException(ODataErrorStrings.RequestUriProcessor_EntitySetNotSpecified(identifier));
                }

                segment.SingleResult = !singleImport.ReturnType.IsCollection();
            }
            else
            {
                segment.TargetEdmEntitySet = null;
                segment.TargetEdmType = null;
                segment.TargetKind = RequestTargetKind.VoidOperation;
            }

            this.parsedSegments.Add(segment);
            return true;
        }

        /// <summary>
        /// Creates the next segment.
        /// </summary>
        /// <param name="text">The text for the next segment.</param>
        private void CreateNextSegment(string text)
        {
            // before treating this as a property, try to handle it as a key property value, unless it was preceeded by an escape-marker segment ('$').
            if (this.TryHandleAsKeySegment(text))
            {
                return;
            }

            if (this.TryCreateValueSegment(text))
            {
                return;
            }

            ODataPathSegment previous = this.parsedSegments[this.parsedSegments.Count - 1];
            if (previous.TargetKind == RequestTargetKind.Primitive)
            {
                // only $value is allowed after a primitive property
                throw ExceptionUtil.ResourceNotFoundError(ODataErrorStrings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment(previous.Identifier, text));
            }

            if (this.TryCreateLinksSegment(text))
            {
                return;
            }

            if (this.TryCreateCountSegment(text))
            {
                return;
            }

            string identifier;
            string parenthesisExpression;
            ExtractSegmentIdentifierAndParenthesisExpression(text, out identifier, out parenthesisExpression);

            if (previous.SingleResult)
            {
                // if its not one of the recognized special segments, then it must be a property, type-segment, or key value.
                Debug.Assert(
                    previous.TargetKind == RequestTargetKind.ComplexObject
                    || previous.TargetKind == RequestTargetKind.Resource 
                    || previous.TargetKind == RequestTargetKind.OpenProperty,
                    "previous.TargetKind(" + previous.TargetKind + ") can have properties");

                if (previous.TargetEdmType == null)
                {
                    // A segment will correspond to a property in the object model;
                    // if we are processing an open type, anything further in the
                    // URI also represents an open type property.
                    Debug.Assert(previous.TargetKind == RequestTargetKind.OpenProperty, "For open properties, the target resource type must be null");
                }
                else
                {
                    // if the segment corresponds to a declared property, handle it
                    // otherwise, fall back to type-segments, actions, and dynamic/open properties
                    IEdmProperty projectedProperty;
                    if (this.TryBindProperty(identifier, out projectedProperty))
                    {
                        CheckSingleResult(previous.SingleResult, previous.Identifier);
                        this.CreatePropertySegment(previous, projectedProperty, parenthesisExpression);
                        return;
                    }
                }
            }

            // If the property resolution failed, and the previous segment was targeting an entity, then we should
            // try and resolve the identifier as type name.
            if (this.TryCreateTypeNameSegment(previous, identifier, parenthesisExpression))
            {
                return;
            }

            if (this.TryCreateSegmentForOperation(previous, identifier, parenthesisExpression))
            {
                return;
            }

            // Try to create an open property if applicable, or throw
            CheckSingleResult(previous.SingleResult, previous.Identifier);
            this.CreateOpenPropertySegment(previous, identifier, parenthesisExpression);
        }

        /// <summary>
        /// Tries to bind the identifier as a property.
        /// </summary>
        /// <param name="identifier">The identifier to bind.</param>
        /// <param name="projectedProperty">The property, if one was found.</param>
        /// <returns>Whether a property matching the identifier was found.</returns>
        private bool TryBindProperty(string identifier, out IEdmProperty projectedProperty)
        {
            ODataPathSegment previous = this.parsedSegments[this.parsedSegments.Count - 1];
            Debug.Assert(previous.TargetKind != RequestTargetKind.OpenProperty, "Since the query element type is known, this can't be open property");
            Debug.Assert(previous.TargetEdmType != null, "Previous wasn't open, so it should have a resource type");
            Debug.Assert(previous.TargetEdmEntitySet == null || previous.TargetEdmType.IsEntityOrEntityCollectionType(), "if the previous segment has a target resource set, then its target resource type must be an entity");

            // Note that we try resolve the property on the root entity type for the set. Properties/Name streams defined on derived types
            // are not supported. This is a general problem with properties as we don't have the entity instance here to validate
            // whether the property exists.
            projectedProperty = null;
            var structuredType = previous.TargetEdmType as IEdmStructuredType;
            if (structuredType == null)
            {
                var collectionType = previous.TargetEdmType as IEdmCollectionType;
                if (collectionType != null)
                {
                    structuredType = collectionType.ElementType.Definition as IEdmStructuredType;
                }
            }

            if (structuredType == null)
            {
                return false;
            }

            projectedProperty = structuredType.FindProperty(identifier);
            return projectedProperty != null;
        }

        /// <summary>
        /// Tries to create a type name segment if the given identifier refers to a known type.
        /// </summary>
        /// <param name="previous">previous segment info.</param>
        /// <param name="identifier">The current raw segment identifier being interpreted.</param>
        /// <param name="parenthesisExpression">Parenthesis expression of this segment.</param>
        /// <returns>Whether or not a type segment was created for the identifier.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri parsing does not go through the same resolvers/settings that payload reading/writing does.")]
        private bool TryCreateTypeNameSegment(ODataPathSegment previous, string identifier, string parenthesisExpression)
        {
            IEdmType targetEdmType;
            if (previous.TargetEdmType == null || (targetEdmType = this.configuration.Model.FindType(identifier)) == null)
            {
                return false;
            }

            // if the new type segment prevents any results from possibly being returned, then short-circuit and throw a 404.
            IEdmType previousEdmType = previous.TargetEdmType;
            Debug.Assert(previousEdmType != null, "previous.TargetEdmType != null");

            if (previousEdmType.TypeKind == EdmTypeKind.Collection)
            {
                previousEdmType = ((IEdmCollectionType)previousEdmType).ElementType.Definition;
            }

            if (!targetEdmType.IsOrInheritsFrom(previousEdmType) && !previousEdmType.IsOrInheritsFrom(targetEdmType))
            {
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType(targetEdmType.ODataFullName(), previousEdmType.ODataFullName()));
            }

            // We want the type of the type segment to be a collection if the previous segment was a collection
            IEdmType actualTypeOfTheTypeSegment = targetEdmType;

            if (previous.EdmType.TypeKind == EdmTypeKind.Collection)
            {
                // creating a new collection type here because the type in the request is just the item type, there is no user-provided collection type.
                actualTypeOfTheTypeSegment = new EdmCollectionType(new EdmEntityTypeReference((IEdmEntityType)actualTypeOfTheTypeSegment, false));
            }

            var typeNameSegment = (ODataPathSegment)new TypeSegment(actualTypeOfTheTypeSegment, previous.TargetEdmEntitySet)
            {
                Identifier = identifier,
                TargetKind = previous.TargetKind,
                SingleResult = previous.SingleResult,
                TargetEdmType = targetEdmType
            };

            this.parsedSegments.Add(typeNameSegment);

            // Key expressions are allowed on Type segments
            this.TryBindKeyFromParentheses(parenthesisExpression);
            
            return true;
        }

        /// <summary>
        /// Creates a property segment
        /// </summary>
        /// <param name="previous">previous segment info.</param>
        /// <param name="property">property to create the segment for.</param>
        /// <param name="queryPortion">query portion for this segment, if specified.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1305:Do not use string.format", Justification = "Will be removed when string freeze is over.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IEdmModel", Justification = "The spelling is correct.")]
        private void CreatePropertySegment(ODataPathSegment previous, IEdmProperty property, string queryPortion)
        {
            if (property.Type.IsStream())
            {
                // The server used to allow arbitrary key expressions after named streams because this check was missing.
                ExceptionUtil.ThrowSyntaxErrorIfNotValid(queryPortion == null || this.configuration.Settings.UseWcfDataServicesServerBehavior);
                this.CreateNamedStreamSegment(previous, property);
                return;
            }

            // Handle a strongly-typed property.
            ODataPathSegment segment = null;

            if (property.PropertyKind == EdmPropertyKind.Navigation)
            {
                var navigationProperty = (IEdmNavigationProperty)property;

                var targetSet = previous.TargetEdmEntitySet.FindNavigationTarget(navigationProperty);

                // If we can't compute the target entity set, then throw
                if (targetSet == null)
                {
                    // Specifically not throwing ODataUriParserException since it's more an an internal server error
                    throw new ODataException(ODataErrorStrings.RequestUriProcessor_TargetEntitySetNotFound(property.Name));
                }

                segment = new NavigationPropertySegment(navigationProperty, targetSet);
            }
            else
            {
                segment = new PropertySegment((IEdmStructuralProperty)property);
                switch (property.Type.TypeKind())
                {
                    case EdmTypeKind.Complex:
                        segment.TargetKind = RequestTargetKind.ComplexObject;
                        break;
                    case EdmTypeKind.Collection:
                        segment.TargetKind = RequestTargetKind.Collection;
                        break;
                    default:
                        Debug.Assert(property.Type.IsPrimitive(), "must be primitive type property");
                        segment.TargetKind = RequestTargetKind.Primitive;
                        break;
                }
            }

            this.parsedSegments.Add(segment);

            ExceptionUtil.ThrowSyntaxErrorIfNotValid(queryPortion == null || property.Type.IsCollection() && property.Type.AsCollection().ElementType().IsEntity());
            this.TryBindKeyFromParentheses(queryPortion);
        }
    }
}
