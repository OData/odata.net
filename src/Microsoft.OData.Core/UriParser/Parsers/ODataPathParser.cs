//---------------------------------------------------------------------
// <copyright file="ODataPathParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Semantic parser for the path of the request URI.
    /// </summary>
    internal sealed class ODataPathParser
    {
        /// <summary>
        /// regex pattern to match a contentID
        /// </summary>
        internal static readonly Regex ContentIdRegex = PlatformHelper.CreateCompiled(@"^\$[0-9]+$", RegexOptions.Singleline);

        /// <summary>Empty list of strings.</summary>
        private static readonly IList<string> EmptyList = new List<string>();

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
            Debug.Assert(segmentText != null, "segment != null");

            int parenthesisStart = segmentText.IndexOf('(');
            if (parenthesisStart < 0)
            {
                identifier = segmentText;
                parenthesisExpression = null;
            }
            else
            {
                if (segmentText[segmentText.Length - 1] != ')')
                {
                    throw ExceptionUtil.CreateSyntaxError();
                }

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
            Debug.Assert(segments != null, "segments != null");
            Debug.Assert(this.parsedSegments.Count == 0, "Segment storage should be empty.");
            Debug.Assert(this.segmentQueue.Count == 0, "Segment queue should be empty.");

            // populate the queue that will be used to drive the rest of the algorithm.
            foreach (var segment in segments)
            {
                this.segmentQueue.Enqueue(segment);
            }

            string segmentText = null;

            try
            {
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
            }
            catch (ODataUnrecognizedPathException ex)
            {
                ex.ParsedSegments = this.parsedSegments;
                ex.CurrentSegment = segmentText;
                ex.UnparsedSegments = this.segmentQueue.ToList();
                throw;
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
        /// Tries to find a single matching operation import for the given identifier, and parameters.
        /// </summary>
        /// <param name="identifier">The identifier from the URI.</param>
        /// <param name="parenthesisExpression">The parenthesis expression contianing parameters, if any.</param>
        /// <param name="configuration">The configuration of the parser.</param>
        /// <param name="boundParameters">The parsed parameters from the parenthesis expression.</param>
        /// <param name="matchingFunctionImport">The single matching operation import if one could be determined.</param>
        /// <returns>Whether or not a matching operation import could be found.</returns>
        private static bool TryBindingParametersAndMatchingOperationImport(string identifier, string parenthesisExpression, ODataUriParserConfiguration configuration, out ICollection<OperationSegmentParameter> boundParameters, out IEdmOperationImport matchingFunctionImport)
        {
            matchingFunctionImport = null;
            ICollection<FunctionParameterToken> splitParameters = null;
            if (!String.IsNullOrEmpty(parenthesisExpression))
            {
                if (!FunctionParameterParser.TrySplitOperationParameters(parenthesisExpression, configuration, out splitParameters))
                {
                    IEdmOperationImport possibleMatchingOperationImport = null;

                    // Look for an overload that returns an entity collection by the specified name. If so parthensis is just key parameters.
                    if (FunctionOverloadResolver.ResolveOperationImportFromList(identifier, EmptyList, configuration.Model, out possibleMatchingOperationImport, configuration.Resolver))
                    {
                        IEdmCollectionTypeReference collectionReturnType = possibleMatchingOperationImport.Operation.ReturnType as IEdmCollectionTypeReference;
                        if (collectionReturnType != null && collectionReturnType.ElementType().IsEntity())
                        {
                            matchingFunctionImport = possibleMatchingOperationImport;
                            boundParameters = null;
                            return true;
                        }
                        else
                        {
                            throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(identifier));
                        }
                    }

                    boundParameters = null;
                    return false;
                }
            }
            else
            {
                splitParameters = new Collection<FunctionParameterToken>();
            }

            // Resolve the specific overload.
            if (FunctionOverloadResolver.ResolveOperationImportFromList(identifier, splitParameters.Select(k => k.ParameterName).ToList(), configuration.Model, out matchingFunctionImport, configuration.Resolver))
            {
                var matchingOperation = matchingFunctionImport.Operation;
                boundParameters = FunctionCallBinder.BindSegmentParameters(configuration, matchingOperation, splitParameters);
                return true;
            }

            boundParameters = null;
            return false;
        }

        /// <summary>
        /// Tries to find a single matching operation import for the given identifier, parametes, and binding type.
        /// </summary>
        /// <param name="identifier">The identifier from the URI.</param>
        /// <param name="parenthesisExpression">The parenthesis expression contianing parameters, if any.</param>
        /// <param name="bindingType">The current binding type or null if there isn't one.</param>
        /// <param name="configuration">The configuration of the parser.</param>
        /// <param name="boundParameters">The parsed parameters from the parenthesis expression.</param>
        /// <param name="matchingOperation">The single matching operation import if one could be determined.</param>
        /// <returns>Whether or not a matching operation import could be found.</returns>
        private static bool TryBindingParametersAndMatchingOperation(string identifier, string parenthesisExpression, IEdmType bindingType, ODataUriParserConfiguration configuration, out ICollection<OperationSegmentParameter> boundParameters, out IEdmOperation matchingOperation)
        {
            // If the name isn't fully qualified then it can't be a function or action.
            // When using extension, there may be function call with unqualified name. So loose the restriction here.
            if (identifier != null && identifier.IndexOf(".", StringComparison.Ordinal) == -1 && configuration.Resolver.GetType() == typeof(ODataUriResolver))
            {
                boundParameters = null;
                matchingOperation = null;
                return false;
            }

            // TODO: update code that is duplicate between operation and operation import, add more tests.
            matchingOperation = null;
            ICollection<FunctionParameterToken> splitParameters;
            if (!String.IsNullOrEmpty(parenthesisExpression))
            {
                if (!FunctionParameterParser.TrySplitOperationParameters(parenthesisExpression, configuration, out splitParameters))
                {
                    IEdmOperation possibleMatchingOperation = null;

                    // Look for an overload that returns an entity collection by the specified name. If so parthensis is just key parameters.
                    if (FunctionOverloadResolver.ResolveOperationFromList(identifier, new List<string>(), bindingType, configuration.Model, out possibleMatchingOperation, configuration.Resolver))
                    {
                        IEdmCollectionTypeReference collectionReturnType = possibleMatchingOperation.ReturnType as IEdmCollectionTypeReference;
                        if (collectionReturnType != null && collectionReturnType.ElementType().IsEntity())
                        {
                            matchingOperation = possibleMatchingOperation;
                            boundParameters = null;
                            return true;
                        }
                        else
                        {
                            throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(identifier));
                        }
                    }

                    boundParameters = null;
                    return false;
                }
            }
            else
            {
                splitParameters = new Collection<FunctionParameterToken>();
            }

            // Resolve the specific overload.
            if (FunctionOverloadResolver.ResolveOperationFromList(identifier, splitParameters.Select(k => k.ParameterName).ToList(), bindingType, configuration.Model, out matchingOperation, configuration.Resolver))
            {
                boundParameters = FunctionCallBinder.BindSegmentParameters(configuration, matchingOperation, splitParameters);
                return true;
            }

            boundParameters = null;
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
                case EdmTypeKind.Enum:
                    return RequestTargetKind.Enum;
                case EdmTypeKind.TypeDefinition:
                    return RequestTargetKind.Primitive;
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
        /// Determines the entity set for segment.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="targetset">The targetset.</param>
        /// <param name="singleOperation">The single operation.</param>
        /// <exception cref="ODataException">Throws and exception if entity set not specified.</exception>
        private static void DetermineEntitySetForSegment(string identifier, IEdmTypeReference returnType, ODataPathSegment segment, IEdmEntitySetBase targetset, IEdmOperation singleOperation)
        {
            if (returnType != null)
            {
                segment.TargetEdmNavigationSource = targetset;
                segment.TargetEdmType = returnType.Definition;
                segment.TargetKind = TargetKindFromType(segment.TargetEdmType);
                segment.SingleResult = !singleOperation.ReturnType.IsCollection();
            }
            else
            {
                segment.TargetEdmNavigationSource = null;
                segment.TargetEdmType = null;
                segment.TargetKind = RequestTargetKind.VoidOperation;
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
            KeySegment previousKeySegment = this.FindPreviousKeySegment();

            KeySegment keySegment;
            if (!this.nextSegmentMustReferToMetadata && SegmentKeyHandler.TryHandleSegmentAsKey(segmentText, previous, previousKeySegment, this.configuration.UrlConventions.UrlConvention, out keySegment, this.configuration.EnableUriTemplateParsing, this.configuration.Resolver))
            {
                this.parsedSegments.Add(keySegment);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Find the ParentNode's key segment
        /// </summary>
        /// <returns>The parent nodes key segment.</returns>
        private KeySegment FindPreviousKeySegment()
        {
            return (KeySegment)this.parsedSegments.LastOrDefault(s => s is KeySegment);
        }

        /// <summary>
        /// Throws if the given segment must be a leaf, as a later segment is being created.
        /// </summary>
        /// <param name="previous">The previous segment which may need to be a leaf.</param>
        private void ThrowIfMustBeLeafSegment(ODataPathSegment previous)
        {
            OperationImportSegment operationImportSegment = previous as OperationImportSegment;
            if (operationImportSegment != null)
            {
                foreach (var operationImport in operationImportSegment.OperationImports)
                {
                    if (operationImport.IsActionImport() || (operationImport.IsFunctionImport() && !((IEdmFunctionImport)operationImport).Function.IsComposable))
                    {
                        throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment(previous.Identifier));
                    }
                }
            }

            OperationSegment operationSegment = previous as OperationSegment;
            if (operationSegment != null)
            {
                foreach (var operation in operationSegment.Operations)
                {
                    if (operation.IsAction() || (operation.IsFunction() && !((IEdmFunction)operation).IsComposable))
                    {
                        throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment(previous.Identifier));
                    }
                }
            }

            if (previous.TargetKind == RequestTargetKind.Batch                  /* $batch */
                || previous.TargetKind == RequestTargetKind.Metadata            /* $metadata */
                || previous.TargetKind == RequestTargetKind.PrimitiveValue      /* $value, see TryCreateValueSegment */
                || previous.TargetKind == RequestTargetKind.OpenPropertyValue   /* $value, see TryCreateValueSegment */
                || previous.TargetKind == RequestTargetKind.EnumValue           /* $value, see TryCreateValueSegment */
                || previous.TargetKind == RequestTargetKind.MediaResource       /* $value or Media resource, see TryCreateValueSegment/CreateNamedStreamSegment */
                || previous.TargetKind == RequestTargetKind.VoidOperation       /* service operation with void return type */
                || previous.TargetKind == RequestTargetKind.Nothing             /* Nothing targeted (e.g. PathTemplate) */)
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

            if (!IdentifierIs(UriQueryConstants.CountSegment, identifier))
            {
                return false;
            }

            // The server used to allow arbitrary key expressions after $count because this check was missing.
            if (parenthesisExpression != null)
            {
                throw ExceptionUtil.CreateSyntaxError();
            }

            ODataPathSegment previous = this.parsedSegments[this.parsedSegments.Count - 1];
            if ((previous.TargetKind != RequestTargetKind.Resource || previous.SingleResult) && previous.TargetKind != RequestTargetKind.Collection)
            {
                throw ExceptionUtil.ResourceNotFoundError(ODataErrorStrings.RequestUriProcessor_CountNotSupported(previous.Identifier));
            }

            this.parsedSegments.Add(CountSegment.Instance);
            return true;
        }

        /// <summary>
        /// Tries to handle the segment as $ref. If it is $ref, then the rest of the path will be parsed/validated in this call.
        /// </summary>
        /// <param name="text">The text of the segment.</param>
        /// <returns>Whether the text was $ref.</returns>
        private bool TryCreateEntityReferenceSegment(string text)
        {
            string identifier;
            string parenthesisExpression;
            ExtractSegmentIdentifierAndParenthesisExpression(text, out identifier, out parenthesisExpression);

            if (!this.IdentifierIs(UriQueryConstants.RefSegment, identifier))
            {
                return false;
            }

            if (parenthesisExpression != null)
            {
                throw ExceptionUtil.CreateSyntaxError();
            }

            // Create a stack to keep track of KeySegments
            Stack<KeySegment> keySegmentsForPreviousPathSegment = new Stack<KeySegment>();

            while (true)
            {
                KeySegment key = this.parsedSegments[this.parsedSegments.Count - 1] as KeySegment;

                if (key == null)
                {
                    break;
                }

                keySegmentsForPreviousPathSegment.Push(key);
                this.parsedSegments.Remove(key);
            }

            // Get the previous Path segment
            ODataPathSegment previous = this.parsedSegments[this.parsedSegments.Count - 1];
            NavigationPropertySegment navPropSegment = previous as NavigationPropertySegment;

            // $ref can not be applied for entity set. It can be applied only on entity and collection of entities
            if ((navPropSegment == null) || navPropSegment.TargetKind != RequestTargetKind.Resource)
            {
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.PathParser_EntityReferenceNotSupported(previous.Identifier));
            }

            // Since this $ref segment, remove the previous segment from parsedSegments as we need to repalce that with new ref segment
            this.parsedSegments.Remove(previous);

            // If this is a navigation property, find target navigation source
            var targetNavigationSource = this.parsedSegments[parsedSegments.Count - 1].TargetEdmNavigationSource.FindNavigationTarget(navPropSegment.NavigationProperty);

            // If we can't compute the target navigation source, then pretend the navigation property does not exist
            if (targetNavigationSource == null)
            {
                throw ExceptionUtil.CreateResourceNotFoundError(navPropSegment.NavigationProperty.Name);
            }

            // Create new NavigationPropertyLinkSegment
            var navPropLinkSegment = new NavigationPropertyLinkSegment(navPropSegment.NavigationProperty, targetNavigationSource);

            // Add this segment to parsedSegments
            this.parsedSegments.Add(navPropLinkSegment);

            // Add key segments back to the parsed segments
            while (keySegmentsForPreviousPathSegment.Count > 0)
            {
                this.parsedSegments.Add(keySegmentsForPreviousPathSegment.Pop());
            }

            string nextSegmentText;

            // Nothing is allowed after $ref.
            if (this.TryGetNextSegmentText(out nextSegmentText))
            {
                throw ExceptionUtil.ResourceNotFoundError(ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
            }

            return true;
        }

        /// <summary>
        /// Tries to bind a key from the parenthetical section of a segment.
        /// </summary>
        /// <param name="parenthesesSection">The section of the segment inside parentheses, or null if there was none.</param>
        /// <returns>Returns True if a key segment was found and added from the paratheses section otherwise false.</returns>
        private bool TryBindKeyFromParentheses(string parenthesesSection)
        {
            if (parenthesesSection == null)
            {
                return false;
            }

            ODataPathSegment keySegment;
            ODataPathSegment previous = this.parsedSegments[this.parsedSegments.Count - 1];
            KeySegment previousKeySegment = this.FindPreviousKeySegment();
            if (!SegmentKeyHandler.TryCreateKeySegmentFromParentheses(previous, previousKeySegment, parenthesesSection, out keySegment, this.configuration.EnableUriTemplateParsing, this.configuration.Resolver))
            {
                return false;
            }

            this.parsedSegments.Add(keySegment);
            return true;
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

            if (!this.IdentifierIs(UriQueryConstants.ValueSegment, identifier))
            {
                return false;
            }

            if (parenthesisExpression != null)
            {
                throw ExceptionUtil.CreateSyntaxError();
            }

            ODataPathSegment previous = this.parsedSegments[this.parsedSegments.Count - 1];

            ODataPathSegment segment = new ValueSegment(previous.EdmType);
            if ((previous.TargetKind == RequestTargetKind.Primitive)
                || (previous.TargetKind == RequestTargetKind.Enum))
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
            }
            else if (previous.TargetKind == RequestTargetKind.Enum)
            {
                segment.TargetKind = RequestTargetKind.EnumValue;
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
            if (previous.TargetEdmType != null && !previous.TargetEdmType.IsOpenType())
            {
                throw ExceptionUtil.CreateResourceNotFoundError(segment.Identifier);
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
            if (this.IdentifierIs(UriQueryConstants.MetadataSegment, identifier))
            {
                if (parenthesisExpression != null)
                {
                    throw ExceptionUtil.CreateSyntaxError();
                }

                this.parsedSegments.Add(MetadataSegment.Instance);
                return;
            }

            if (this.IdentifierIs(UriQueryConstants.BatchSegment, identifier))
            {
                if (parenthesisExpression != null)
                {
                    throw ExceptionUtil.CreateSyntaxError();
                }

                this.parsedSegments.Add(BatchSegment.Instance);
                return;
            }

            if (this.IdentifierIs(UriQueryConstants.CountSegment, identifier))
            {
                // $count on root: throw
                throw ExceptionUtil.ResourceNotFoundError(ODataErrorStrings.RequestUriProcessor_CountOnRoot);
            }

            if (this.configuration.BatchReferenceCallback != null && ContentIdRegex.IsMatch(identifier))
            {
                if (parenthesisExpression != null)
                {
                    throw ExceptionUtil.CreateSyntaxError();
                }

                BatchReferenceSegment crossReferencedSegement = this.configuration.BatchReferenceCallback(identifier);
                if (crossReferencedSegement != null)
                {
                    this.parsedSegments.Add(crossReferencedSegement);
                    return;
                }
            }

            if (this.TryCreateSegmentForNavigationSource(identifier, parenthesisExpression))
            {
                return;
            }

            if (this.TryCreateSegmentForOperationImport(identifier, parenthesisExpression))
            {
                return;
            }

            throw ExceptionUtil.CreateResourceNotFoundError(identifier);
        }

        /// <summary>
        /// Tries to parse a segment as an entity set or singleton.
        /// </summary>
        /// <param name="identifier">The name of the segment</param>
        /// <param name="parenthesisExpression">The parenthesis expression</param>
        /// <returns>Whether or not the identifier referred to an entity set or singleton.</returns>
        private bool TryCreateSegmentForNavigationSource(string identifier, string parenthesisExpression)
        {
            ODataPathSegment segment = null;
            IEdmEntitySet targetEdmEntitySet;
            IEdmSingleton targetEdmSingleton;

            IEdmNavigationSource source = this.configuration.Resolver.ResolveNavigationSource(this.configuration.Model, identifier);

            if ((targetEdmEntitySet = source as IEdmEntitySet) != null)
            {
                segment = new EntitySetSegment(targetEdmEntitySet) { Identifier = identifier };
            }
            else if ((targetEdmSingleton = source as IEdmSingleton) != null)
            {
                segment = new SingletonSegment(targetEdmSingleton) { Identifier = identifier };
            }

            if (segment != null)
            {
                this.parsedSegments.Add(segment);
                this.TryBindKeyFromParentheses(parenthesisExpression);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to parse a segment as a functionImport or actionImport.
        /// </summary>
        /// <param name="identifier">The name of the segment</param>
        /// <param name="parenthesisExpression">The query portion</param>
        /// <returns>Whether or not the identifier referred to an action.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:Do not use string.format", Justification = "Will be removed when string freeze is over.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IEdmModel", Justification = "The spelling is correct.")]
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri parsing does not go through the same resolvers/settings that payload reading/writing does.")]
        private bool TryCreateSegmentForOperationImport(string identifier, string parenthesisExpression)
        {
            ICollection<OperationSegmentParameter> resolvedParameters;
            IEdmOperationImport singleImport;
            if (!TryBindingParametersAndMatchingOperationImport(identifier, parenthesisExpression, this.configuration, out resolvedParameters, out singleImport))
            {
                return false;
            }

            IEdmTypeReference returnType = singleImport.Operation.ReturnType;
            IEdmEntitySetBase targetset = null;

            if (returnType != null)
            {
                targetset = singleImport.GetTargetEntitySet(null, this.configuration.Model);
            }

            // TODO: change constructor to take single import
            ODataPathSegment segment = new OperationImportSegment(new[] { singleImport }, targetset, resolvedParameters);

            DetermineEntitySetForSegment(identifier, returnType, segment, targetset, singleImport.Operation);

            this.parsedSegments.Add(segment);

            this.TryBindKeySegmentIfNoResolvedParametersAndParathesisValueExsts(parenthesisExpression, returnType, resolvedParameters, segment);

            return true;
        }

        /// <summary>
        /// Tries the bind key segment if no resolved parameters and parathesis value exsts.
        /// </summary>
        /// <param name="parenthesisExpression">The parenthesis expression.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="resolvedParameters">The resolved parameters.</param>
        /// <param name="segment">The segment.</param>
        private void TryBindKeySegmentIfNoResolvedParametersAndParathesisValueExsts(string parenthesisExpression, IEdmTypeReference returnType, ICollection<OperationSegmentParameter> resolvedParameters, ODataPathSegment segment)
        {
            IEdmCollectionTypeReference collectionTypeReference = returnType as IEdmCollectionTypeReference;
            if (collectionTypeReference != null && collectionTypeReference.ElementType().IsEntity() && resolvedParameters == null && parenthesisExpression != null)
            {
                // The parameters in the parathesis is a key segment.
                if (this.TryBindKeyFromParentheses(parenthesisExpression))
                {
                    this.ThrowIfMustBeLeafSegment(segment);
                }
            }
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
            IEdmOperation singleOperation;
            if (!TryBindingParametersAndMatchingOperation(identifier, parenthesisExpression, bindingType, this.configuration, out resolvedParameters, out singleOperation))
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

            IEdmTypeReference returnType = singleOperation.ReturnType;
            IEdmEntitySetBase targetset = null;

            if (returnType != null)
            {
                IEdmNavigationSource source = previousSegment == null ? null : previousSegment.TargetEdmNavigationSource;
                targetset = singleOperation.GetTargetEntitySet(source, this.configuration.Model);
            }

            // If previous segment is cross-referenced then we explicitly dissallow the service action call
            if (previousSegment is BatchReferenceSegment)
            {
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_BatchedActionOnEntityCreatedInSameChangeset(identifier));
            }

            // TODO: change constructor to take single import
            ODataPathSegment segment = new OperationSegment(new[] { singleOperation }, resolvedParameters, targetset)
            {
                Identifier = identifier
            };

            DetermineEntitySetForSegment(identifier, returnType, segment, targetset, singleOperation);

            this.parsedSegments.Add(segment);

            this.TryBindKeySegmentIfNoResolvedParametersAndParathesisValueExsts(parenthesisExpression, returnType, resolvedParameters, segment);

            return true;
        }

        /// <summary>
        /// Creates the next segment.
        /// </summary>
        /// <param name="text">The text for the next segment.</param>
        private void CreateNextSegment(string text)
        {
            // before treating this as a property, try to handle it as a key property value, unless it was preceeded by an escape-marker segment ('$').
            // But when use ODataSimplified convention, only do this if the segment should not be interpreted as a type.
            if (!this.configuration.UrlConventions.UrlConvention.ODataSimplified && this.TryHandleAsKeySegment(text))
            {
                return;
            }

            // Parse as path template segment if EnableUriTemplateParsing is enabled.
            if (this.configuration.EnableUriTemplateParsing && UriTemplateParser.IsValidTemplateLiteral(text))
            {
                this.parsedSegments.Add(new PathTemplateSegment(text));
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

            if (this.TryCreateEntityReferenceSegment(text))
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

            // OData simplified convention, try to handle it as a key property value after can't parse as type and operation
            if (this.configuration.UrlConventions.UrlConvention.ODataSimplified && this.TryHandleAsKeySegment(text))
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
            Debug.Assert(previous.TargetEdmNavigationSource == null || previous.TargetEdmType.IsEntityOrEntityCollectionType(), "if the previous segment has a target resource set, then its target resource type must be an entity");

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

            projectedProperty = this.configuration.Resolver.ResolveProperty(structuredType, identifier);
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
            if (previous.TargetEdmType == null || (targetEdmType = UriEdmHelpers.FindTypeFromModel(this.configuration.Model, identifier, this.configuration.Resolver)) == null)
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
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType(targetEdmType.FullTypeName(), previousEdmType.FullTypeName()));
            }

            // We want the type of the type segment to be a collection if the previous segment was a collection
            IEdmType actualTypeOfTheTypeSegment = targetEdmType;

            if (previous.EdmType.TypeKind == EdmTypeKind.Collection)
            {
                // creating a new collection type here because the type in the request is just the item type, there is no user-provided collection type.
                var actualEntityTypeOfTheTypeSegment = actualTypeOfTheTypeSegment as IEdmEntityType;
                if (actualEntityTypeOfTheTypeSegment != null)
                {
                    actualTypeOfTheTypeSegment = new EdmCollectionType(new EdmEntityTypeReference(actualEntityTypeOfTheTypeSegment, false));
                }
                else
                {
                    // Complex collection supports type cast too.
                    var actualComplexTypeOfTheTypeSegment = actualTypeOfTheTypeSegment as IEdmComplexType;
                    if (actualComplexTypeOfTheTypeSegment != null)
                    {
                        actualTypeOfTheTypeSegment = new EdmCollectionType(new EdmComplexTypeReference(actualComplexTypeOfTheTypeSegment, false));
                    }
                    else
                    {
                        throw new ODataException(Strings.PathParser_TypeCastOnlyAllowedAfterStructuralCollection(identifier));
                    }
                }
            }

            var typeNameSegment = (ODataPathSegment)new TypeSegment(actualTypeOfTheTypeSegment, previous.TargetEdmNavigationSource)
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
                if (queryPortion != null)
                {
                    throw ExceptionUtil.CreateSyntaxError();
                }

                this.CreateNamedStreamSegment(previous, property);
                return;
            }

            // Handle a strongly-typed property.
            ODataPathSegment segment = null;

            if (property.PropertyKind == EdmPropertyKind.Navigation)
            {
                var navigationProperty = (IEdmNavigationProperty)property;
                IEdmNavigationSource navigationSource = previous.TargetEdmNavigationSource.FindNavigationTarget(navigationProperty);

                // Relationship between TargetMultiplicity and navigation property:
                //  1) EdmMultiplicity.Many <=> collection navigation property
                //  2) EdmMultiplicity.ZeroOrOne <=> nullable singleton navigation property
                //  3) EdmMultiplicity.One <=> non-nullable singleton navigation property
                //
                // According to OData Spec CSDL 7.1.3:
                //  1) non-nullable singleton navigation property => navigation source required
                //  2) the other cases => navigation source optional
                if (navigationProperty.TargetMultiplicity() == EdmMultiplicity.One
                    && navigationSource is IEdmUnknownEntitySet)
                {
                    // Specifically not throwing ODataUriParserException since it's more an an internal server error
                    throw new ODataException(ODataErrorStrings.RequestUriProcessor_TargetEntitySetNotFound(property.Name));
                }

                segment = new NavigationPropertySegment(navigationProperty, navigationSource);
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
                    case EdmTypeKind.Enum:
                        segment.TargetKind = RequestTargetKind.Enum;
                        break;
                    default:
                        Debug.Assert(property.Type.IsPrimitive() || property.Type.IsTypeDefinition(), "must be primitive type or type definition property");
                        segment.TargetKind = RequestTargetKind.Primitive;
                        break;
                }
            }

            this.parsedSegments.Add(segment);

            if (!(queryPortion == null || property.Type.IsCollection() && property.Type.AsCollection().ElementType().IsEntity()))
            {
                throw ExceptionUtil.CreateSyntaxError();
            }

            this.TryBindKeyFromParentheses(queryPortion);
        }

        /// <summary>
        /// Check whether identifiers matches according to case in sensitive option.
        /// </summary>
        /// <param name="expected">The expected identifer.</param>
        /// <param name="identifier">Identifier to be evaluated.</param>
        /// <returns>Whether the identifier matches.</returns>
        private bool IdentifierIs(string expected, string identifier)
        {
            return string.Equals(
                expected,
                identifier,
                this.configuration.EnableCaseInsensitiveUriFunctionIdentifier ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }
    }
}
