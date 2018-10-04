//---------------------------------------------------------------------
// <copyright file="ODataPathParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Metadata;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.UriParser
{
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
        /// Last navigation source that has been parsed.
        /// </summary>
        private IEdmNavigationSource lastNavigationSource;

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
        /// Will throw if identifier is not found or if the parenthesis expression is malformed. This function does not validate
        /// anything and simply provides the raw text of both the identifier and parenthetical expression.
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

                    // Keep track of last navigation source.
                    IEdmNavigationSource navigationSource = parsedSegments.Last().TranslateWith(new DetermineNavigationSourceTranslator());
                    if (navigationSource != null)
                    {
                        lastNavigationSource = navigationSource;
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

            List<ODataPathSegment> validatedSegments = CreateValidatedPathSegments();
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
                ThrowIfMustBeLeafSegment(this.parsedSegments[this.parsedSegments.Count - 1]);
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
            if (!this.nextSegmentMustReferToMetadata && SegmentKeyHandler.TryHandleSegmentAsKey(segmentText, previous, previousKeySegment, this.configuration.UrlKeyDelimiter, this.configuration.Resolver, out keySegment, this.configuration.EnableUriTemplateParsing))
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
        private static void ThrowIfMustBeLeafSegment(ODataPathSegment previous)
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
                || previous.TargetKind == RequestTargetKind.DynamicValue   /* $value, see TryCreateValueSegment */
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
        /// <param name="identifier">The identifier that was parsed from this raw segment.</param>
        /// <param name="parenthesisExpression">The query portion was parsed from this raw segment.
        /// This value can be null if there is no query portion.</param>
        /// <returns>Whether the segment was $count.</returns>
        private bool TryCreateCountSegment(string identifier, string parenthesisExpression)
        {
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
        /// Creates a filter clause from a navigation source and filter expression.
        /// </summary>
        /// <param name="navigationSource">Navigation source to which the filter refers.</param>
        /// <param name="targetEdmType">Target type for the entity being referenced.</param>
        /// <param name="filter">Filter expression.</param>
        /// <returns>Filter clause with the navigation source and filter information.</returns>
        private FilterClause GenerateFilterClause(IEdmNavigationSource navigationSource, IEdmType targetEdmType, string filter)
        {
            Debug.Assert(navigationSource != null, "navigationSource != null");
            Debug.Assert(targetEdmType != null, "targetEdmType != null");
            Debug.Assert(filter.Length > 0, "filter.Length > 0");

            ODataPathInfo currentOdataPathInfo = new ODataPathInfo(targetEdmType, navigationSource);

            // Get the syntactic representation of the filter expression.
            UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(
                configuration.Settings.FilterLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier);
            QueryToken filterToken = expressionParser.ParseFilter(filter);

            // Bind it to metadata.
            BindingState state = new BindingState(configuration, currentOdataPathInfo.Segments.ToList())
            {
                ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(
                currentOdataPathInfo.TargetEdmType.ToTypeReference(), currentOdataPathInfo.TargetNavigationSource)
            };
            state.RangeVariables.Push(state.ImplicitRangeVariable);

            MetadataBinder binder = new MetadataBinder(state);
            FilterBinder filterBinder = new FilterBinder(binder.Bind, state);

            return filterBinder.BindFilter(filterToken);
        }

        /// <summary>
        /// Try to handle the segment as $filter.
        /// </summary>
        /// <param name="segmentText">The raw segment text.</param>
        /// <returns>Whether the segment was $filter.</returns>
        /// <remarks>$filter path segment is different from existing path segments in that it strictly
        /// follows the format of "$filter(@a)", where @a represents a parameter alias. Thus, this function
        /// should validate the format of the path segment closely.</remarks>
        private bool TryCreateFilterSegment(string segmentText)
        {
            Debug.Assert(segmentText != null, "segmentText != null");
            Debug.Assert(parsedSegments.Count > 0, "parsedSegments.Count > 0");

            /*
             * 1) Check whether the path segment starts with $filter.
             * 2) Ensure that the expression that follows the identifier is enclosed in parentheses.
             * 3) Extract the expression and validate it syntactically.
             * 4) Add the filter segment to list of parsed segments.
             */

            // 1) Check whether the path segment starts with $filter. Past this point, we will throw invalid syntax exceptions
            // because we will assume that the user is attempting to use the $filter path segment.
            if (!segmentText.StartsWith(
                UriQueryConstants.FilterSegment,
                this.configuration.EnableCaseInsensitiveUriFunctionIdentifier ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
            {
                return false;
            }

            // 2) The expression that follows UriQueryConstants.FilterSegment should be enclosed in parentheses.
            // Step 3) performs the expression validation (e.g. illegal characters).
            //      - the length of this segment should be longer than "$filter()", indicating that there's a valid expression
            int index = UriQueryConstants.FilterSegment.Length;
            if (segmentText.Length <= index + 2 || segmentText[index] != '(' || segmentText[segmentText.Length - 1] != ')')
            {
                throw new ODataException(ODataErrorStrings.RequestUriProcessor_FilterPathSegmentSyntaxError);
            }

            // 3) Extract the parameter alias and perform the rest of the validations on it.
            if (lastNavigationSource == null)
            {
                throw new ODataException(ODataErrorStrings.RequestUriProcessor_NoNavigationSourceFound(UriQueryConstants.FilterSegment));
            }

            if (lastNavigationSource is IEdmSingleton || this.parsedSegments.Last() is KeySegment)
            {
                throw new ODataException(ODataErrorStrings.RequestUriProcessor_CannotApplyFilterOnSingleEntities(lastNavigationSource.Name));
            }

            // The "index + 1" is to move past the '(' and the '-2' accounts for the two paren characters.
            string alias = segmentText.Substring(index + 1, segmentText.Length - UriQueryConstants.FilterSegment.Length - 2);

            // If the previous segment is a type segment, then the entity set has been casted and the filter expression should reflect the cast.
            TypeSegment typeSegment = this.parsedSegments.Last() as TypeSegment;

            // Creating a filter clause helps validate the expression and create the expression nodes (including nested parameter aliases).
            FilterClause filterClause = GenerateFilterClause(
                lastNavigationSource,
                typeSegment == null ? lastNavigationSource.EntityType() : typeSegment.TargetEdmType,
                alias);
            ParameterAliasNode aliasNode = filterClause.Expression as ParameterAliasNode;
            if (aliasNode == null)
            {
                throw new ODataException(ODataErrorStrings.RequestUriProcessor_FilterPathSegmentSyntaxError);
            }

            // 4) Create filter segment with the validated expression and add it to parsed segments.
            FilterSegment filterSegment = new FilterSegment(aliasNode, filterClause.RangeVariable, lastNavigationSource);
            this.parsedSegments.Add(filterSegment);

            return true;
        }

        /// <summary>
        /// Try to handle the segment as $each.
        /// </summary>
        /// <param name="identifier">The identifier that was parsed from this raw segment.</param>
        /// <param name="parenthesisExpression">The query portion was parsed from this raw segment.
        /// This value can be null if there is no query portion.</param>
        /// <returns>Whether the segment was $each.</returns>
        private bool TryCreateEachSegment(string identifier, string parenthesisExpression)
        {
            if (!IdentifierIs(UriQueryConstants.EachSegment, identifier))
            {
                return false;
            }

            // $each is not supposed to have parenthesis expressions after it.
            if (parenthesisExpression != null)
            {
                throw ExceptionUtil.CreateSyntaxError();
            }

            ODataPathSegment prevSegment = this.parsedSegments.Last();
            if (lastNavigationSource == null)
            {
                throw new ODataException(ODataErrorStrings.RequestUriProcessor_NoNavigationSourceFound(UriQueryConstants.EachSegment));
            }

            if (lastNavigationSource is IEdmSingleton || prevSegment is KeySegment)
            {
                throw new ODataException(ODataErrorStrings.RequestUriProcessor_CannotApplyEachOnSingleEntities(lastNavigationSource.Name));
            }

            EachSegment eachSegment = new EachSegment(
                lastNavigationSource,
                prevSegment is TypeSegment || prevSegment is FilterSegment ? prevSegment.TargetEdmType : null);
            this.parsedSegments.Add(eachSegment);

            return true;
        }

        /// <summary>
        /// Tries to handle the segment as $ref. If it is $ref, then the rest of the path will be parsed/validated in this call.
        /// </summary>
        /// <param name="identifier">The identifier that was parsed from this raw segment.</param>
        /// <param name="parenthesisExpression">The query portion was parsed from this raw segment.
        /// This value can be null if there is no query portion.</param>
        /// <returns>Whether the text was $ref.</returns>
        private bool TryCreateEntityReferenceSegment(string identifier, string parenthesisExpression)
        {
            if (!this.IdentifierIs(UriQueryConstants.RefSegment, identifier))
            {
                return false;
            }

            if (parenthesisExpression != null)
            {
                throw ExceptionUtil.CreateSyntaxError();
            }

            // The algorithm below looks for the last NavigationPropertySegment before the $ref segment. Whether a NavPropSeg exists
            // becomes two different code paths:
            // 1) Backwards compatibility behavior (<= ODL 7.4.x): If the NavPropSeg exists, then it is expected to either be before
            // KeySegments or before the $ref (i.e. NavPropSeg/KeySeg/KeySeg/.../$ref or NavPropSeg/$ref). Then the NavPropSeg is replaced
            // with NavigationPropertyLinkSegment. In a previous implementation, if either a NavPropSeg didn't exist before KeySegments or
            // $ref itself, then this function would throw. This was not correct as $ref can apply to entity sets and similar segments
            // (e.g. TypeSegment and FilterSegment), and therefore 2) below is implemented to support those scenarios.
            // 2) If the NavPropSeg does not exist, then this algorithm appends a ReferenceSegment to the existing list of parsed segments.

            // Determine the index of the NavigationPropertySegment in either of the following formats:
            // NavPropSeg/KeySeg/KeySeg/.../$ref or NavPropSeg/$ref
            int indexOfNavPropSeg = this.parsedSegments.Count - 1;
            while (indexOfNavPropSeg > 0 && this.parsedSegments[indexOfNavPropSeg] is KeySegment)
            {
                --indexOfNavPropSeg;
            }

            NavigationPropertySegment navPropSegment = this.parsedSegments[indexOfNavPropSeg] as NavigationPropertySegment;
            if (navPropSegment != null)
            {
                if (navPropSegment.TargetKind != RequestTargetKind.Resource)
                {
                    throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.PathParser_EntityReferenceNotSupported(navPropSegment.Identifier));
                }

                // If this is a navigation property, find target navigation source
                Debug.Assert(indexOfNavPropSeg - 1 >= 0, "indexOfNavPropSeg - 1 >= 0");
                IEdmPathExpression bindingPath;
                var targetNavigationSource = this.parsedSegments[indexOfNavPropSeg - 1].TargetEdmNavigationSource.FindNavigationTarget(
                    navPropSegment.NavigationProperty, BindingPathHelper.MatchBindingPath, this.parsedSegments, out bindingPath);

                // If we can't compute the target navigation source, then pretend the navigation property does not exist
                if (targetNavigationSource == null)
                {
                    throw ExceptionUtil.CreateResourceNotFoundError(navPropSegment.NavigationProperty.Name);
                }

                // Create new NavigationPropertyLinkSegment
                var navPropLinkSegment = new NavigationPropertyLinkSegment(navPropSegment.NavigationProperty, targetNavigationSource);

                // Replace the NavigationPropertySegment with the $ref NavigationPropertyLinkSegment
                this.parsedSegments[indexOfNavPropSeg] = navPropLinkSegment;
            }
            else
            {
                if (this.parsedSegments.Last().TargetKind != RequestTargetKind.Resource || this.parsedSegments.Last().SingleResult)
                {
                    throw ExceptionUtil.CreateBadRequestError(
                        ODataErrorStrings.PathParser_EntityReferenceNotSupported(this.parsedSegments.Last().Identifier));
                }

                ReferenceSegment referenceSegment = new ReferenceSegment(lastNavigationSource);
                this.parsedSegments.Add(referenceSegment);
            }

            // Nothing is allowed after $ref.
            string nextSegmentText;
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
            if (!SegmentKeyHandler.TryCreateKeySegmentFromParentheses(previous, previousKeySegment, parenthesesSection, this.configuration.Resolver, out keySegment, this.configuration.EnableUriTemplateParsing))
            {
                return false;
            }

            this.parsedSegments.Add(keySegment);
            return true;
        }

        /// <summary>
        /// Try to handle the segment as $value.
        /// </summary>
        /// <param name="identifier">The identifier that was parsed from this raw segment.</param>
        /// <param name="parenthesisExpression">The query portion was parsed from this raw segment.
        /// This value can be null if there is no query portion.</param>
        /// <returns>Whether the segment was $value.</returns>
        private bool TryCreateValueSegment(string identifier, string parenthesisExpression)
        {
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
            else if (previous.TargetKind == RequestTargetKind.Dynamic)
            {
                segment.TargetKind = RequestTargetKind.DynamicValue;
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
        /// Creates a new segment for an unknown path segment or an open property.
        /// </summary>
        /// <param name="previous">previous segment info.</param>
        /// <param name="identifier">name of the segment.</param>
        /// <param name="parenthesisExpression">whether this segment has a query portion or not.</param>
        private void CreateDynamicPathSegment(ODataPathSegment previous, string identifier, string parenthesisExpression)
        {
            if (this.configuration.ParseDynamicPathSegmentFunc != null)
            {
                var segments = this.configuration.ParseDynamicPathSegmentFunc(previous, identifier, parenthesisExpression);
                this.parsedSegments.AddRange(segments);
                return;
            }

            if (previous == null)
            {
                throw ExceptionUtil.CreateResourceNotFoundError(identifier);
            }

            CheckSingleResult(previous.SingleResult, previous.Identifier);

            // Handle an open type property. If the current leaf isn't an
            // object (which implies it's already an open type), then
            // it should be marked as an open type.
            if ((previous.TargetEdmType != null && !previous.TargetEdmType.IsOpen()))
            {
                throw ExceptionUtil.CreateResourceNotFoundError(identifier);
            }

            // Open navigation properties are not supported on OpenTypes.
            if (parenthesisExpression != null)
            {
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.OpenNavigationPropertiesNotSupportedOnOpenTypes(identifier));
            }

            ODataPathSegment segment = new DynamicPathSegment(identifier);
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
        private void CreateFirstSegment(string segmentText)
        {
            string identifier;
            string parenthesisExpression;
            ExtractSegmentIdentifierAndParenthesisExpression(segmentText, out identifier, out parenthesisExpression);

            Debug.Assert(identifier != null, "identifier != null");

            // Look for well-known system resource points.
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

            if (this.IdentifierIs(UriQueryConstants.FilterSegment, identifier))
            {
                // $filter on root: throw
                throw ExceptionUtil.ResourceNotFoundError(ODataErrorStrings.RequestUriProcessor_FilterOnRoot);
            }

            if (this.IdentifierIs(UriQueryConstants.EachSegment, identifier))
            {
                // $each on root: throw
                throw ExceptionUtil.ResourceNotFoundError(ODataErrorStrings.RequestUriProcessor_EachOnRoot);
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

            this.CreateDynamicPathSegment(null, identifier, parenthesisExpression);
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
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IEdmModel", Justification = "The spelling is correct.")]
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

            ODataPathSegment segment = new OperationImportSegment(singleImport, targetset, resolvedParameters);

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
                    ThrowIfMustBeLeafSegment(segment);
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
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IEdmModel", Justification = "The spelling is correct.")]
        private bool TryCreateSegmentForOperation(ODataPathSegment previousSegment, string identifier, string parenthesisExpression)
        {
            // Parse Arguments syntactically
            IEdmType bindingType = null;
            if (previousSegment != null)
            {
                // Use TargetEdmType for EachSegment to represent a pseudo-single entity.
                bindingType = (previousSegment is EachSegment) ? previousSegment.TargetEdmType : previousSegment.EdmType;
            }

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

            ODataPathSegment segment = new OperationSegment(singleOperation, resolvedParameters, targetset)
            {
                Identifier = identifier
            };

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
            string identifier;
            string parenthesisExpression;
            ExtractSegmentIdentifierAndParenthesisExpression(text, out identifier, out parenthesisExpression);

            /*
             * For Non-KeyAsSegment, try to handle it as a key property value, unless it was preceeded by an excape - marker segment('$').
             * For KeyAsSegment, the following precedence rules should be supported[ODATA - 799]:
             * Try to match an OData segment(starting with “$”).
             *   - Note: $filter path segment is a special case that has the format "$filter=@a", where @a represents an alias.
             * Try to match an alias - qualified bound action name, bound function overload, or type name.
             * Try to match a namespace-qualified bound action name, bound function overload, or type name.
             * Try to match an unqualified bound action name, bound function overload, or type name in a default namespace.
             * Treat as a key.
             */

            // $value
            if (this.TryCreateValueSegment(identifier, parenthesisExpression))
            {
                return;
            }

            ODataPathSegment previous = this.parsedSegments[this.parsedSegments.Count - 1];
            if (previous.TargetKind == RequestTargetKind.Primitive)
            {
                // only $value is allowed after a primitive property
                throw ExceptionUtil.ResourceNotFoundError(ODataErrorStrings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment(previous.Identifier, text));
            }

            // $ref
            if (this.TryCreateEntityReferenceSegment(identifier, parenthesisExpression))
            {
                return;
            }

            // $count
            if (this.TryCreateCountSegment(identifier, parenthesisExpression))
            {
                return;
            }

            // $filter
            if (this.TryCreateFilterSegment(text))
            {
                return;
            }

            // $each
            if (this.TryCreateEachSegment(identifier, parenthesisExpression))
            {
                return;
            }

            // property if previous is single
            if (previous.SingleResult)
            {
                // if its not one of the recognized special segments, then it must be a property, type-segment, or key value.
                Debug.Assert(
                    previous.TargetKind == RequestTargetKind.Resource
                    || previous.TargetKind == RequestTargetKind.Dynamic,
                    "previous.TargetKind(" + previous.TargetKind + ") can have properties");

                if (previous.TargetEdmType == null)
                {
                    // A segment will correspond to a property in the object model;
                    // if we are processing an open type, anything further in the
                    // URI also represents an open type property.
                    Debug.Assert(previous.TargetKind == RequestTargetKind.Dynamic, "For open properties, the target resource type must be null");
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

            // Type cast
            if (text.IndexOf('.') >= 0 && // type-cast should use qualified type names
                this.TryCreateTypeNameSegment(previous, identifier, parenthesisExpression))
            {
                return;
            }

            // Operation
            if (this.TryCreateSegmentForOperation(previous, identifier, parenthesisExpression))
            {
                return;
            }

            // For KeyAsSegment, try to handle as key segment
            if (this.configuration.UrlKeyDelimiter.EnableKeyAsSegment && this.TryHandleAsKeySegment(text))
            {
                return;
            }

            // Parse as path template segment if EnableUriTemplateParsing is enabled.
            if (this.configuration.EnableUriTemplateParsing && UriTemplateParser.IsValidTemplateLiteral(text))
            {
                this.parsedSegments.Add(new PathTemplateSegment(text));
                return;
            }

            // Dynamic property
            this.CreateDynamicPathSegment(previous, identifier, parenthesisExpression);
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
            Debug.Assert(previous.TargetEdmType != null, "Previous wasn't open, so it should have a resource type");
            Debug.Assert(previous.TargetEdmNavigationSource == null || previous.TargetEdmType.IsStructuredOrStructuredCollectionType(), "if the previous segment has a target resource set, then its target resource type must be an entity or a complex");

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

            var typeNameSegment = (ODataPathSegment)new TypeSegment(actualTypeOfTheTypeSegment, previous.EdmType, previous.TargetEdmNavigationSource)
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
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IEdmModel", Justification = "The spelling is correct.")]
        private void CreatePropertySegment(ODataPathSegment previous, IEdmProperty property, string queryPortion)
        {
            Debug.Assert(previous != null, "previous != null");

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

                IEdmNavigationSource navigationSource = null;
                if (previous.TargetEdmNavigationSource != null)
                {
                    IEdmPathExpression bindingPath;
                    navigationSource = previous.TargetEdmNavigationSource.FindNavigationTarget(navigationProperty, BindingPathHelper.MatchBindingPath, this.parsedSegments, out bindingPath);
                }

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
                        segment.TargetKind = RequestTargetKind.Resource;
                        segment.TargetEdmNavigationSource = previous.TargetEdmNavigationSource;
                        break;
                    case EdmTypeKind.Collection:
                        if (property.Type.IsStructuredCollectionType())
                        {
                            segment.TargetKind = RequestTargetKind.Resource;
                            segment.TargetEdmNavigationSource = previous.TargetEdmNavigationSource;
                        }

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

        /// <summary>
        /// Validates the existing parsed segments and returns a list of validated segments.
        /// </summary>
        /// <returns>List of validated path segments.</returns>
        private List<ODataPathSegment> CreateValidatedPathSegments()
        {
            List<ODataPathSegment> validatedSegments = new List<ODataPathSegment>(this.parsedSegments.Count);
            for (int index = 0, segmentCount = this.parsedSegments.Count; index < segmentCount; ++index)
            {
                CheckDollarEachSegmentRestrictions(index);
#if DEBUG
                this.parsedSegments[index].AssertValid();
#endif
                validatedSegments.Add(this.parsedSegments[index]);
            }

            return validatedSegments;
        }

        /// <summary>
        /// Per OData 4.01 spec, GET operation and functions may proceed $each, but we are limiting the scope of that spec
        /// by permitting only ONE action segment to follow $each. As such, at most one $each segment can exist in the URI.
        /// This function enforces those restrictions and throws if any of them are violated.
        /// </summary>
        /// <param name="index">Index of path segment to examine in the list of parsed segments.</param>
        /// <exception cref="ODataException">Throws if there's a violation of $each restrictions.</exception>
        /// <remarks>Should the restrictions on the $each be removed, this function can be deleted.</remarks>
        private void CheckDollarEachSegmentRestrictions(int index)
        {
            Debug.Assert(index < this.parsedSegments.Count, "index < this.parsedSegments.Count");

            int numOfSegmentsAfterDollarEach = this.parsedSegments.Count - index - 1;

            // Perform restriction checks only if the current segment being examined is $each and there are subsequent segments.
            if (this.parsedSegments[index] is EachSegment && numOfSegmentsAfterDollarEach > 0)
            {
                // Only one segment is allowed after $each...
                if (numOfSegmentsAfterDollarEach > 1)
                {
                    throw new ODataException(ODataErrorStrings.RequestUriProcessor_OnlySingleActionCanProceedEachPathSegment);
                }

                // And if there exists a single segment after $each, then it must be an action.
                OperationSegment operationSegment = this.parsedSegments[index + 1] as OperationSegment;
                if (operationSegment == null || !(operationSegment.Operations.All(a => a is IEdmAction)))
                {
                    throw new ODataException(ODataErrorStrings.RequestUriProcessor_OnlySingleActionCanProceedEachPathSegment);
                }
            }
        }
    }
}
