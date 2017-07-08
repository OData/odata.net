//---------------------------------------------------------------------
// <copyright file="FunctionCallBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.OData.Metadata;
using Microsoft.OData.Edm;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Class that knows how to bind function call tokens.
    /// </summary>
    internal sealed class FunctionCallBinder : BinderBase
    {
        /// <summary>
        /// The names of functions that we don't bind to BuiltInFunctions
        /// </summary>
        private static readonly string[] UnboundFunctionNames = new string[]
        {
            ExpressionConstants.UnboundFunctionCast,
            ExpressionConstants.UnboundFunctionIsOf,
        };

        /// <summary>
        /// Constructs a FunctionCallBinder with the given method to be used binding the parent token if needed.
        /// </summary>
        /// <param name="bindMethod">Method to use for binding the parent token, if needed.</param>
        /// <param name="state">State of the metadata binding.</param>
        internal FunctionCallBinder(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
            : base(bindMethod, state)
        {
        }

        /// <summary>
        /// Promotes types of arguments to match signature if possible.
        /// </summary>
        /// <param name="signature">The signature to match the types to.</param>
        /// <param name="argumentNodes">The types to promote.</param>
        internal static void TypePromoteArguments(FunctionSignatureWithReturnType signature, List<QueryNode> argumentNodes)
        {
            // Convert all argument nodes to the best signature argument type
            Debug.Assert(signature.ArgumentTypes.Length == argumentNodes.Count, "The best signature match doesn't have the same number of arguments.");
            for (int i = 0; i < argumentNodes.Count; i++)
            {
                Debug.Assert(argumentNodes[i] is SingleValueNode, "We should have already verified that all arguments are single values.");
                SingleValueNode argumentNode = (SingleValueNode)argumentNodes[i];
                IEdmTypeReference signatureArgumentType = signature.ArgumentTypes[i];
                Debug.Assert(signatureArgumentType.IsODataPrimitiveTypeKind(), "Only primitive types should be able to get here.");
                argumentNodes[i] = MetadataBindingUtils.ConvertToTypeIfNeeded(argumentNode, signatureArgumentType);
            }
        }

        /// <summary>
        /// Checks that all arguments are SingleValueNodes
        /// </summary>
        /// <param name="functionName">The name of the function the arguments are from.</param>
        /// <param name="argumentNodes">The arguments to validate.</param>
        /// <returns>SingleValueNode array</returns>
        internal static SingleValueNode[] ValidateArgumentsAreSingleValue(string functionName, List<QueryNode> argumentNodes)
        {
            ExceptionUtils.CheckArgumentNotNull(functionName, "functionCallToken");
            ExceptionUtils.CheckArgumentNotNull(argumentNodes, "argumentNodes");

            // Right now all functions take a single value for all arguments
            SingleValueNode[] ret = new SingleValueNode[argumentNodes.Count];
            for (int i = 0; i < argumentNodes.Count; i++)
            {
                SingleValueNode argumentNode = argumentNodes[i] as SingleValueNode;
                if (argumentNode == null)
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_FunctionArgumentNotSingleValue(functionName));
                }

                ret[i] = argumentNode;
            }

            return ret;
        }

        /// <summary>
        /// Finds the signature that best matches the arguments
        /// </summary>
        /// <param name="functionName">The name of the function</param>
        /// <param name="argumentNodes">The nodes of the arguments, can be new {null,null}.</param>
        /// <param name="signatures">The signatures to match against</param>
        /// <returns>Returns the matching signature or throws</returns>
        internal static FunctionSignatureWithReturnType MatchSignatureToUriFunction(string functionName, SingleValueNode[] argumentNodes, FunctionSignatureWithReturnType[] signatures)
        {
            FunctionSignatureWithReturnType signature;
            IEdmTypeReference[] argumentTypes = argumentNodes.Select(s => s.TypeReference).ToArray();

            // Handle the cases where we don't have type information (null literal, open properties) for ANY of the arguments
            int argumentCount = argumentTypes.Length;
            if (argumentTypes.All(a => a == null) && argumentCount > 0)
            {
                // we specifically want to find just the first function that matches the number of arguments, we don't care about
                // ambiguity here because we're already in an ambiguous case where we don't know what kind of types
                // those arguments are.
                signature = signatures.FirstOrDefault(candidateFunction => candidateFunction.ArgumentTypes.Count() == argumentCount);
                if (signature == null)
                {
                    throw new ODataException(ODataErrorStrings.FunctionCallBinder_CannotFindASuitableOverload(functionName, argumentTypes.Count()));
                }
                else
                {
                    // in this case we can't assert the return type, we can only assert that a function exists... so
                    // we need to set the return type to null.
                    signature = new FunctionSignatureWithReturnType(null, signature.ArgumentTypes);
                }
            }
            else
            {
                signature = TypePromotionUtils.FindBestFunctionSignature(signatures, argumentNodes);
                if (signature == null)
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_NoApplicableFunctionFound(
                        functionName,
                        UriFunctionsHelper.BuildFunctionSignatureListDescription(functionName, signatures)));
                }
            }

            return signature;
        }

        /// <summary>
        /// Finds all signatures for the given function name.
        /// Search in both BuiltIn uri functions and Custom uri functions.
        /// Combine and return the signatures overloads of the results.
        /// </summary>
        /// <param name="functionName">The function to get the signatures for.</param>
        /// <returns>The signatures which match the supplied function name.</returns>
        internal static FunctionSignatureWithReturnType[] GetUriFunctionSignatures(string functionName)
        {
            FunctionSignatureWithReturnType[] customUriFunctionsSignatures = null;
            FunctionSignatureWithReturnType[] builtInUriFunctionsSignatures = null;

            // Try to find the function in the user custom functions and in our built-in functions
            bool customFound = CustomUriFunctions.TryGetCustomFunction(functionName, out customUriFunctionsSignatures);
            bool builtInFound = BuiltInUriFunctions.TryGetBuiltInFunction(functionName, out builtInUriFunctionsSignatures);

            if (!customFound && !builtInFound)
            {
                // Not found in both built-in and custom.
                throw new ODataException(ODataErrorStrings.MetadataBinder_UnknownFunction(functionName));
            }

            if (!customFound)
            {
                Debug.Assert(builtInUriFunctionsSignatures != null, "No Built-in functions found");
                return builtInUriFunctionsSignatures;
            }

            if (!builtInFound)
            {
                Debug.Assert(customUriFunctionsSignatures != null, "No Custom functions found");
                return customUriFunctionsSignatures;
            }

            return builtInUriFunctionsSignatures.Concat(customUriFunctionsSignatures).ToArray();
        }

        /// <summary>
        /// Binds the token to a SingleValueFunctionCallNode or a SingleResourceFunctionCallNode for complex
        /// </summary>
        /// <param name="functionCallToken">Token to bind</param>
        /// <returns>The resulting SingleValueFunctionCallNode/SingleResourceFunctionCallNode</returns>
        internal QueryNode BindFunctionCall(FunctionCallToken functionCallToken)
        {
            ExceptionUtils.CheckArgumentNotNull(functionCallToken, "functionCallToken");
            ExceptionUtils.CheckArgumentNotNull(functionCallToken.Name, "functionCallToken.Name");

            // Bind the parent, if present.
            // TODO: parent can be a collection as well, so we need to loosen this to QueryNode.
            QueryNode parent = null;
            if (state.ImplicitRangeVariable != null)
            {
                if (functionCallToken.Source != null)
                {
                    parent = this.bindMethod(functionCallToken.Source);
                }
                else
                {
                    parent = NodeFactory.CreateRangeVariableReferenceNode(state.ImplicitRangeVariable);
                }
            }

            // First see if there is a custom function for this
            QueryNode boundFunction;
            if (this.TryBindIdentifier(functionCallToken.Name, functionCallToken.Arguments, parent, state, out boundFunction))
            {
                return boundFunction;
            }

            // then check if there is a global custom function(i.e with out a parent node)
            if (this.TryBindIdentifier(functionCallToken.Name, functionCallToken.Arguments, null, state, out boundFunction))
            {
                return boundFunction;
            }

            // If there isn't, bind as Uri function
            // Bind all arguments
            List<QueryNode> argumentNodes = new List<QueryNode>(functionCallToken.Arguments.Select(ar => this.bindMethod(ar)));
            return BindAsUriFunction(functionCallToken, argumentNodes);
        }

        /// <summary>
        /// Try to bind an end path token as a function call. Used for bound functions without parameters
        /// that parse as end path tokens syntactically
        /// </summary>
        /// <param name="endPathToken">the end path token to bind</param>
        /// <param name="parent">the parent node to this end path token.</param>
        /// <param name="state">the current state of the binding algorithm</param>
        /// <param name="boundFunction">a single value function call node representing the function call, if it exists</param>
        /// <returns>true if we found a function for this token, false otherwise.</returns>
        internal bool TryBindEndPathAsFunctionCall(EndPathToken endPathToken, QueryNode parent, BindingState state, out QueryNode boundFunction)
        {
            return this.TryBindIdentifier(endPathToken.Identifier, null, parent, state, out boundFunction);
        }

        /// <summary>
        /// Try to bind an inner path token as a function call. Used for bound functions without parameters
        /// that parse as inner path tokens syntactically
        /// </summary>
        /// <param name="innerPathToken">the end path token to bind</param>
        /// <param name="parent">the parent node to this end path token.</param>
        /// <param name="boundFunction">a single value function call node representing the function call, if it exists</param>
        /// <returns>true if we found a function for this token, false otherwise.</returns>
        internal bool TryBindInnerPathAsFunctionCall(InnerPathToken innerPathToken, QueryNode parent, out QueryNode boundFunction)
        {
            return this.TryBindIdentifier(innerPathToken.Identifier, null, parent, state, out boundFunction);
        }

        /// <summary>
        /// Try to bind a <see cref="DottedIdentifierToken"/> as a function call. Used for container qualified functions without parameters.
        /// </summary>
        /// <param name="dottedIdentifierToken">the dotted identifier token to bind</param>
        /// <param name="parent">the semantically bound parent node for this dotted identifier</param>
        /// <param name="boundFunction">a single value function call node representing the function call, if we found one.</param>
        /// <returns>true if we found a function for this token, false otherwise.</returns>
        internal bool TryBindDottedIdentifierAsFunctionCall(DottedIdentifierToken dottedIdentifierToken, SingleValueNode parent, out QueryNode boundFunction)
        {
            return this.TryBindIdentifier(dottedIdentifierToken.Identifier, null, parent, state, out boundFunction);
        }

        /// <summary>
        /// Bind this function call token as a Uri function
        /// </summary>
        /// <param name="functionCallToken">the function call token to bind</param>
        /// <param name="argumentNodes">list of semantically bound arguments</param>
        /// <returns>A function call node bound to this function.</returns>
        private QueryNode BindAsUriFunction(FunctionCallToken functionCallToken, List<QueryNode> argumentNodes)
        {
            if (functionCallToken.Source != null)
            {
                // the parent must be null for a Uri function.
                throw new ODataException(ODataErrorStrings.FunctionCallBinder_UriFunctionMustHaveHaveNullParent(functionCallToken.Name));
            }

            string functionCallTokenName = this.state.Configuration.EnableCaseInsensitiveUriFunctionIdentifier ? functionCallToken.Name.ToLowerInvariant() : functionCallToken.Name;

            // There are some functions (IsOf and Cast for example) that don't necessarily need to be bound to a function signature,
            // for these, we just Bind them directly to a SingleValueFunctionCallNode
            if (IsUnboundFunction(functionCallTokenName))
            {
                return CreateUnboundFunctionNode(functionCallTokenName, argumentNodes);
            }

            // Do some validation and get potential Uri functions that could match what we saw
            FunctionSignatureWithReturnType[] signatures = GetUriFunctionSignatures(functionCallTokenName);
            SingleValueNode[] argumentNodeArray = ValidateArgumentsAreSingleValue(functionCallTokenName, argumentNodes);
            FunctionSignatureWithReturnType signature = MatchSignatureToUriFunction(functionCallTokenName, argumentNodeArray, signatures);
            if (signature.ReturnType != null)
            {
                TypePromoteArguments(signature, argumentNodes);
            }

            if (signature.ReturnType != null && signature.ReturnType.IsStructured())
            {
                return new SingleResourceFunctionCallNode(functionCallTokenName, new ReadOnlyCollection<QueryNode>(argumentNodes), signature.ReturnType.AsStructured(), null);
            }

            return new SingleValueFunctionCallNode(functionCallTokenName, new ReadOnlyCollection<QueryNode>(argumentNodes), signature.ReturnType);
        }

        /// <summary>
        /// Try to bind an identifier to a FunctionCallNode
        /// </summary>
        /// <param name="identifier">the identifier to bind</param>
        /// <param name="arguments">the semantically bound list of arguments.</param>
        /// <param name="parent">a semantically bound parent node.</param>
        /// <param name="state">the current state of the binding algorithm</param>
        /// <param name="boundFunction">a single value function call node representing this function call, if we found one.</param>
        /// <returns>true if we found a function for this token.</returns>
        private bool TryBindIdentifier(string identifier, IEnumerable<FunctionParameterToken> arguments, QueryNode parent, BindingState state, out QueryNode boundFunction)
        {
            boundFunction = null;

            IEdmType bindingType = null;
            var singleValueParent = parent as SingleValueNode;
            if (singleValueParent != null)
            {
                if (singleValueParent.TypeReference != null)
                {
                    bindingType = singleValueParent.TypeReference.Definition;
                }
            }
            else
            {
                var collectionValueParent = parent as CollectionNode;
                if (collectionValueParent != null)
                {
                    bindingType = collectionValueParent.CollectionType.Definition;
                }
            }

            if (!UriEdmHelpers.IsBindingTypeValid(bindingType))
            {
                return false;
            }

            // All functions should be fully qualified, if they aren't they they aren't functions.
            // When using extension, there may be function call with unqualified name. So loose the restriction here.
            if (identifier.IndexOf(".", StringComparison.Ordinal) == -1 && this.Resolver.GetType() == typeof(ODataUriResolver))
            {
                return false;
            }

            IEdmOperation operation;
            List<FunctionParameterToken> syntacticArguments = arguments == null ? new List<FunctionParameterToken>() : arguments.ToList();
            if (!FunctionOverloadResolver.ResolveOperationFromList(identifier, syntacticArguments.Select(ar => ar.ParameterName).ToList(), bindingType, state.Model, out operation, this.Resolver))
            {
                // TODO: FunctionOverloadResolver.ResolveOperationFromList() looks up the function by parameter names, but it shouldn't ignore parameter types. (test case ParseFilter_AliasInFunction_PropertyAsValue_TypeMismatch should fail)
                return false;
            }

            if (singleValueParent != null && singleValueParent.TypeReference == null)
            {
                // if the parent exists, but has no type information, then we're in open type land, and we
                // shouldn't go any farther.
                throw new ODataException(ODataErrorStrings.FunctionCallBinder_CallingFunctionOnOpenProperty(identifier));
            }

            if (operation.IsAction())
            {
                return false;
            }

            IEdmFunction function = (IEdmFunction)operation;

            // TODO:  $filter $orderby parameter expression which contains complex or collection should NOT be supported in this way
            //     but should be parsed into token tree, and binded to node tree: parsedParameters.Select(p => this.bindMethod(p));
            ICollection<FunctionParameterToken> parsedParameters = HandleComplexOrCollectionParameterValueIfExists(state.Configuration.Model, function, syntacticArguments, state.Configuration.Resolver.EnableCaseInsensitive);

            IEnumerable<QueryNode> boundArguments = parsedParameters.Select(p => this.bindMethod(p));
            boundArguments = boundArguments.ToList(); // force enumerable to run : will immediately evaluate all this.bindMethod(p).
            IEdmTypeReference returnType = function.ReturnType;
            IEdmEntitySetBase returnSet = null;
            var singleEntityNode = parent as SingleResourceNode;
            if (singleEntityNode != null)
            {
                returnSet = function.GetTargetEntitySet(singleEntityNode.NavigationSource, state.Model);
            }

            string functionName = function.FullName();

            if (returnType.IsEntity())
            {
                boundFunction = new SingleResourceFunctionCallNode(functionName, new[] { function }, boundArguments, (IEdmEntityTypeReference)returnType.Definition.ToTypeReference(), returnSet, parent);
            }
            else if (returnType.IsStructuredCollection())
            {
                IEdmCollectionTypeReference collectionTypeReference = (IEdmCollectionTypeReference)returnType;
                boundFunction = new CollectionResourceFunctionCallNode(functionName, new[] { function }, boundArguments, collectionTypeReference, returnSet, parent);
            }
            else if (returnType.IsCollection())
            {
                IEdmCollectionTypeReference collectionTypeReference = (IEdmCollectionTypeReference)returnType;
                boundFunction = new CollectionFunctionCallNode(functionName, new[] { function }, boundArguments, collectionTypeReference, parent);
            }
            else
            {
                boundFunction = new SingleValueFunctionCallNode(functionName, new[] { function }, boundArguments,
                    returnType, parent);
            }

            return true;
        }

        /// <summary>
        /// Bind path segment's operation or operationImport's parameters.
        /// </summary>
        /// <param name="configuration">The ODataUriParserConfiguration.</param>
        /// <param name="functionOrOpertion">The function or operation.</param>
        /// <param name="segmentParameterTokens">The parameter tokens to be binded.</param>
        /// <returns>The binded semantic nodes.</returns>
        internal static List<OperationSegmentParameter> BindSegmentParameters(ODataUriParserConfiguration configuration, IEdmOperation functionOrOpertion, ICollection<FunctionParameterToken> segmentParameterTokens)
        {
            // TODO: HandleComplexOrCollectionParameterValueIfExists is temp work around for single copmlex or colleciton type, it can't handle nested complex or collection value.
            ICollection<FunctionParameterToken> parametersParsed = FunctionCallBinder.HandleComplexOrCollectionParameterValueIfExists(configuration.Model, functionOrOpertion, segmentParameterTokens, configuration.Resolver.EnableCaseInsensitive, configuration.EnableUriTemplateParsing);

            // Bind it to metadata
            BindingState state = new BindingState(configuration);
            state.ImplicitRangeVariable = null;
            state.RangeVariables.Clear();
            MetadataBinder binder = new MetadataBinder(state);
            List<OperationSegmentParameter> boundParameters = new List<OperationSegmentParameter>();

            IDictionary<string, SingleValueNode> input = new Dictionary<string, SingleValueNode>(StringComparer.Ordinal);
            foreach (var paraToken in parametersParsed)
            {
                // TODO: considering another better exception
                if (paraToken.ValueToken is EndPathToken)
                {
                    throw new ODataException(Strings.MetadataBinder_ParameterNotInScope(
                        string.Format(CultureInfo.InvariantCulture, "{0}={1}", paraToken.ParameterName, (paraToken.ValueToken as EndPathToken).Identifier)));
                }

                SingleValueNode boundNode = (SingleValueNode)binder.Bind(paraToken.ValueToken);

                if (!input.ContainsKey(paraToken.ParameterName))
                {
                    input.Add(paraToken.ParameterName, boundNode);
                }
            }

            IDictionary<IEdmOperationParameter, SingleValueNode> result = configuration.Resolver.ResolveOperationParameters(functionOrOpertion, input);

            foreach (var item in result)
            {
                SingleValueNode boundNode = item.Value;

                // ensure node type is compatible with parameter type.
                var sourceTypeReference = boundNode.GetEdmTypeReference();
                bool sourceIsNullOrOpenType = (sourceTypeReference == null);
                if (!sourceIsNullOrOpenType)
                {
                    // if the node has been rewritten, no further conversion is needed.
                    if (!TryRewriteIntegralConstantNode(ref boundNode, item.Key.Type))
                    {
                        boundNode = MetadataBindingUtils.ConvertToTypeIfNeeded(boundNode, item.Key.Type);
                    }
                }

                OperationSegmentParameter boundParamer = new OperationSegmentParameter(item.Key.Name, boundNode);
                boundParameters.Add(boundParamer);
            }

            return boundParameters;
        }

        /// <summary>
        /// Try to rewrite an Edm.Int32 constant node if its value is within the valid range of the target integer type.
        /// </summary>
        /// <param name="boundNode">The node to be rewritten.</param>
        /// <param name="targetType">The target type reference.</param>
        /// <returns>If the node is successfully rewritten.</returns>
        private static bool TryRewriteIntegralConstantNode(ref SingleValueNode boundNode, IEdmTypeReference targetType)
        {
            if (targetType == null || !targetType.IsByte() && !targetType.IsSByte() && !targetType.IsInt16())
            {
                return false;
            }

            var constantNode = boundNode as ConstantNode;
            if (constantNode == null)
            {
                return false;
            }

            var sourceType = constantNode.TypeReference;
            if (sourceType == null || !sourceType.IsInt32())
            {
                return false;
            }

            var sourceValue = (int)constantNode.Value;
            object targetValue = null;
            switch (targetType.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Byte:
                    if (sourceValue >= byte.MinValue && sourceValue <= byte.MaxValue)
                    {
                        targetValue = (byte)sourceValue;
                    }

                    break;
                case EdmPrimitiveTypeKind.SByte:
                    if (sourceValue >= sbyte.MinValue && sourceValue <= sbyte.MaxValue)
                    {
                        targetValue = (sbyte)sourceValue;
                    }

                    break;
                case EdmPrimitiveTypeKind.Int16:
                    if (sourceValue >= short.MinValue && sourceValue <= short.MaxValue)
                    {
                        targetValue = (short)sourceValue;
                    }

                    break;
            }

            if (targetValue == null)
            {
                return false;
            }

            boundNode = new ConstantNode(targetValue, constantNode.LiteralText, targetType);
            return true;
        }

        /// <summary>
        /// This is temp work around for $filter $orderby parameter expression which contains complex or collection
        ///     like "Fully.Qualified.Namespace.CanMoveToAddresses(addresses=[{\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"},{\"Street\":\"Pine St.\",\"City\":\"Seattle\"}])";
        /// TODO:  $filter $orderby parameter expression which contains nested complex or collection should NOT be supported in this way
        ///     but should be parsed into token tree, and binded to node tree: parsedParameters.Select(p => this.bindMethod(p));
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="operation">IEdmFunction or IEdmOperation</param>
        /// <param name="parameterTokens">The tokens to bind.</param>
        /// <param name="enableCaseInsensitive">Whether to enable case-insensitive when resolving parameter name.</param>
        /// <param name="enableUriTemplateParsing">Whether Uri template parsing is enabled.</param>
        /// <returns>The FunctionParameterTokens with complex or collection values converted from string like "{...}", or "[..,..,..]".</returns>
        private static ICollection<FunctionParameterToken> HandleComplexOrCollectionParameterValueIfExists(IEdmModel model, IEdmOperation operation, ICollection<FunctionParameterToken> parameterTokens, bool enableCaseInsensitive, bool enableUriTemplateParsing = false)
        {
            ICollection<FunctionParameterToken> partiallyParsedParametersWithComplexOrCollection = new Collection<FunctionParameterToken>();
            foreach (FunctionParameterToken paraToken in parameterTokens)
            {
                FunctionParameterToken funcParaToken;
                IEdmOperationParameter functionParameter = operation.FindParameter(paraToken.ParameterName);
                if (enableCaseInsensitive && functionParameter == null)
                {
                    functionParameter = ODataUriResolver.ResolveOpearationParameterNameCaseInsensitive(operation, paraToken.ParameterName);

                    // The functionParameter can not be null here, else this method won't be called.
                    funcParaToken = new FunctionParameterToken(functionParameter.Name, paraToken.ValueToken);
                }
                else
                {
                    funcParaToken = paraToken;
                }

                FunctionParameterAliasToken aliasToken = funcParaToken.ValueToken as FunctionParameterAliasToken;
                if (aliasToken != null)
                {
                    aliasToken.ExpectedParameterType = functionParameter.Type;
                }

                LiteralToken valueToken = funcParaToken.ValueToken as LiteralToken;
                string valueStr = null;
                if (valueToken != null && (valueStr = valueToken.Value as string) != null && !string.IsNullOrEmpty(valueToken.OriginalText))
                {
                    var lexer = new ExpressionLexer(valueToken.OriginalText, true /*moveToFirstToken*/, false /*useSemicolonDelimiter*/, true /*parsingFunctionParameters*/);
                    if (lexer.CurrentToken.Kind == ExpressionTokenKind.BracketedExpression || lexer.CurrentToken.Kind == ExpressionTokenKind.BracedExpression)
                    {
                        object result;
                        UriTemplateExpression expression;

                        if (enableUriTemplateParsing && UriTemplateParser.TryParseLiteral(lexer.CurrentToken.Text, functionParameter.Type, out expression))
                        {
                            result = expression;
                        }
                        else if (!functionParameter.Type.IsStructured() && !functionParameter.Type.IsStructuredCollectionType())
                        {
                            // ExpressionTokenKind.BracketedExpression means text like [1,2]
                            // so now try convert it to collection type value:
                            result = ODataUriUtils.ConvertFromUriLiteral(valueStr, ODataVersion.V4, model, functionParameter.Type);
                        }
                        else
                        {
                            // For complex & colleciton of complex directly return the raw string.
                            partiallyParsedParametersWithComplexOrCollection.Add(funcParaToken);
                            continue;
                        }

                        LiteralToken newValueToken = new LiteralToken(result, valueToken.OriginalText);
                        FunctionParameterToken newFuncParaToken = new FunctionParameterToken(funcParaToken.ParameterName, newValueToken);
                        partiallyParsedParametersWithComplexOrCollection.Add(newFuncParaToken);
                        continue;
                    }
                }

                partiallyParsedParametersWithComplexOrCollection.Add(funcParaToken);
            }

            return partiallyParsedParametersWithComplexOrCollection;
        }

        /// <summary>
        /// Determines whether this is a function that we don't bind to a BuiltInFunction
        /// </summary>
        /// <param name="functionName">name of the function</param>
        /// <returns>true if this is a function that we don't bind</returns>
        private static bool IsUnboundFunction(string functionName)
        {
            return UnboundFunctionNames.Contains(functionName);
        }

        /// <summary>
        /// Build a SingleValueFunctionCallNode for a function that isn't bound to a BuiltInFunction
        /// </summary>
        /// <param name="functionCallTokenName">Name for the function</param>
        /// <param name="args">list of already bound query nodes for this function</param>
        /// <returns>A single value function call node bound to this function.</returns>
        private SingleValueNode CreateUnboundFunctionNode(string functionCallTokenName, List<QueryNode> args)
        {
            // need to figure out the return type and check the correct number of arguments based on the function name
            IEdmTypeReference returnType = null;
            switch (functionCallTokenName)
            {
                case ExpressionConstants.UnboundFunctionIsOf:
                    {
                        returnType = ValidateAndBuildIsOfArgs(state, ref args);
                        break;
                    }

                case ExpressionConstants.UnboundFunctionCast:
                    {
                        returnType = ValidateAndBuildCastArgs(state, ref args);
                        if (returnType.IsStructured())
                        {
                            SingleResourceNode entityNode = args.ElementAt(0) as SingleResourceNode;

                            return new SingleResourceFunctionCallNode(functionCallTokenName, args,
                                returnType.AsStructured(), entityNode != null ? entityNode.NavigationSource : null);
                        }

                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            // we have everything else we need, so return the new SingleValueFunctionCallNode.
            return new SingleValueFunctionCallNode(functionCallTokenName, args, returnType);
        }

        /// <summary>
        /// Validate the args list (adding the implicit range variable if necessary), and determine the correct return type for a cast function
        /// </summary>
        /// <param name="state">current binding state, used to get the implicit range variable if necessary</param>
        /// <param name="args">list of arguments, could be changed</param>
        /// <returns>the return type from this cast function</returns>
        private static IEdmTypeReference ValidateAndBuildCastArgs(BindingState state, ref List<QueryNode> args)
        {
            return ValidateIsOfOrCast(state, true, ref args);
        }

        /// <summary>
        /// Validate the arguments (adding the implicit range variable if necessary), and determine the correct return type
        /// for an IsOf function
        /// </summary>
        /// <param name="state">the current state of the binding algorithm, used to get the implicit range variable if necessary</param>
        /// <param name="args">current list of args, can be changed</param>
        /// <returns>the correct return type for this function.</returns>
        private static IEdmTypeReference ValidateAndBuildIsOfArgs(BindingState state, ref List<QueryNode> args)
        {
            return ValidateIsOfOrCast(state, false, ref args);
        }

        /// <summary>
        /// Validate the arguments to either isof or cast
        /// </summary>
        /// <param name="state">the current state of the binding algorithm</param>
        /// <param name="isCast">flag to indicate which function we're validating</param>
        /// <param name="args">the list of arguments, which could be changed</param>
        /// <returns>the return type of the function.</returns>
        private static IEdmTypeReference ValidateIsOfOrCast(BindingState state, bool isCast, ref List<QueryNode> args)
        {
            if (args.Count != 1 && args.Count != 2)
            {
                throw new ODataErrorException(
                    ODataErrorStrings.MetadataBinder_CastOrIsOfExpressionWithWrongNumberOfOperands(args.Count));
            }

            ConstantNode typeArgument = args.Last() as ConstantNode;

            IEdmTypeReference returnType = null;
            if (typeArgument != null)
            {
                returnType = TryGetTypeReference(state.Model, typeArgument.Value as string, state.Configuration.Resolver);
            }

            if (returnType == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument);
            }

            if (returnType.IsCollection())
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_CastOrIsOfCollectionsNotSupported);
            }

            // if we only have one argument, then add the implicit range variable as the first argument.
            if (args.Count == 1)
            {
                args = new List<QueryNode>()
                    {
                        new ResourceRangeVariableReferenceNode(
                                                             state.ImplicitRangeVariable.Name,
                                                             state.ImplicitRangeVariable as ResourceRangeVariable),
                        args[0]
                    };
            }
            else if (!(args[0] is SingleValueNode))
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_CastOrIsOfCollectionsNotSupported);
            }

            if (isCast && (args.Count == 2))
            {
                // throw if cast enum to not-string :
                if ((args[0].GetEdmTypeReference() is IEdmEnumTypeReference)
                    && !string.Equals(typeArgument.Value as string, Microsoft.OData.Metadata.EdmConstants.EdmStringTypeName, StringComparison.Ordinal))
                {
                    throw new ODataException(ODataErrorStrings.CastBinder_EnumOnlyCastToOrFromString);
                }

                // throw if cast not-string to enum :
                while (returnType is IEdmEnumTypeReference)
                {
                    IEdmTypeReference edmTypeReference = args[0].GetEdmTypeReference();
                    if (edmTypeReference == null)
                    {
                        // Support cast null to enum
                        break;
                    }

                    IEdmPrimitiveTypeReference referenceTmp = edmTypeReference as IEdmPrimitiveTypeReference;
                    if (referenceTmp != null)
                    {
                        IEdmPrimitiveType typeTmp = referenceTmp.Definition as IEdmPrimitiveType;
                        if ((typeTmp != null) && (typeTmp.PrimitiveKind == EdmPrimitiveTypeKind.String))
                        {
                            break;
                        }
                    }

                    throw new ODataException(ODataErrorStrings.CastBinder_EnumOnlyCastToOrFromString);
                }
            }

            if (isCast)
            {
                return returnType;
            }
            else
            {
                return EdmCoreModel.Instance.GetBoolean(true);
            }
        }

        /// <summary>
        /// Try to get an IEdmTypeReference for a given type as a string, returns null if none exists
        /// </summary>
        /// <param name="model">the model for validation</param>
        /// <param name="fullTypeName">the type name to find</param>
        /// <param name="resolver">Resolver for this func.</param>
        /// <returns>an IEdmTypeReference for this type string.</returns>
        private static IEdmTypeReference TryGetTypeReference(IEdmModel model, string fullTypeName, ODataUriResolver resolver)
        {
            IEdmTypeReference typeReference = UriEdmHelpers.FindTypeFromModel(model, fullTypeName, resolver).ToTypeReference();
            if (typeReference == null)
            {
                if (fullTypeName.StartsWith("Collection", StringComparison.Ordinal))
                {
                    string[] tokenizedString = fullTypeName.Split('(');
                    string baseElementType = tokenizedString[1].Split(')')[0];
                    return EdmCoreModel.GetCollection(UriEdmHelpers.FindTypeFromModel(model, baseElementType, resolver).ToTypeReference());
                }
                else
                {
                    return null;
                }
            }

            return typeReference;
        }
    }
}
