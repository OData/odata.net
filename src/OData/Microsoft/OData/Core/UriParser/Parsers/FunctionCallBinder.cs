//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Class that knows how to bind function call tokens.
    /// </summary>
    internal sealed class FunctionCallBinder
    {
        /// <summary>
        /// Method to use for binding the parent node, if needed.
        /// </summary>
        private readonly MetadataBinder.QueryTokenVisitor bindMethod;

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
        internal FunctionCallBinder(MetadataBinder.QueryTokenVisitor bindMethod)
        {
            this.bindMethod = bindMethod;
        }

        /// <summary>
        /// Promotes types of arguments to match signature if possible.
        /// </summary>
        /// <param name="signature">The signature to match the types to.</param>
        /// <param name="argumentNodes">The types to promote.</param>
        internal static void TypePromoteArguments(FunctionSignature signature, List<QueryNode> argumentNodes)
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
        internal static FunctionSignatureWithReturnType MatchSignatureToBuiltInFunction(string functionName, SingleValueNode[] argumentNodes, FunctionSignatureWithReturnType[] signatures)
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
                        BuiltInFunctions.BuildFunctionSignatureListDescription(functionName, signatures)));
                }
            }

            return signature;
        }

        /// <summary>
        /// Finds all signatures for the given function name.
        /// </summary>
        /// <param name="functionName">The function to get the signatures for.</param>
        /// <returns>The signatures which match the supplied function name.</returns>
        internal static FunctionSignatureWithReturnType[] GetBuiltInFunctionSignatures(string functionName)
        {
            // Try to find the function in our built-in functions
            FunctionSignatureWithReturnType[] signatures;
            if (!BuiltInFunctions.TryGetBuiltInFunction(functionName, out signatures))
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_UnknownFunction(functionName));
            }

            return signatures;
        }

        /// <summary>
        /// Binds the token to a SingleValueFunctionCallNode
        /// </summary>
        /// <param name="functionCallToken">Token to bind</param>
        /// <param name="state">The current state of the binding algorithm</param>
        /// <returns>The resulting SingleValueFunctionCallNode</returns>
        internal QueryNode BindFunctionCall(FunctionCallToken functionCallToken, BindingState state)
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

            // If there isn't, bind as built-in function
            // Bind all arguments
            List<QueryNode> argumentNodes = new List<QueryNode>(functionCallToken.Arguments.Select(ar => this.bindMethod(ar)));
            return BindAsBuiltInFunction(functionCallToken, state, argumentNodes);
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
        /// <param name="state">the current state of the binding algorithm</param>
        /// <param name="boundFunction">a single value function call node representing the function call, if it exists</param>
        /// <returns>true if we found a function for this token, false otherwise.</returns>
        internal bool TryBindInnerPathAsFunctionCall(InnerPathToken innerPathToken, QueryNode parent, BindingState state, out QueryNode boundFunction)
        {
            return this.TryBindIdentifier(innerPathToken.Identifier, null, parent, state, out boundFunction);
        }

        /// <summary>
        /// Try to bind a <see cref="DottedIdentifierToken"/> as a function call. Used for container qualified functions without parameters.
        /// </summary>
        /// <param name="dottedIdentifierToken">the dotted identifier token to bind</param>
        /// <param name="parent">the semantically bound parent node for this dotted identifier</param>
        /// <param name="state">the current stat of the binding algorithm</param>
        /// <param name="boundFunction">a single value function call node representing the function call, if we found one.</param>
        /// <returns>true if we found a function for this token, false otherwise.</returns>
        internal bool TryBindDottedIdentifierAsFunctionCall(DottedIdentifierToken dottedIdentifierToken, SingleValueNode parent, BindingState state, out QueryNode boundFunction)
        {
            return this.TryBindIdentifier(dottedIdentifierToken.Identifier, null, parent, state, out boundFunction);
        }

        /// <summary>
        /// Bind this function call token as a built in function
        /// </summary>
        /// <param name="functionCallToken">the function call token to bidn</param>
        /// <param name="state">the current state of the binding algorithm</param>
        /// <param name="argumentNodes">list of semantically bound arguments</param>
        /// <returns>A function call node bound to this function.</returns>
        private static QueryNode BindAsBuiltInFunction(FunctionCallToken functionCallToken, BindingState state, List<QueryNode> argumentNodes)
        {
            if (functionCallToken.Source != null)
            {
                // the parent must be null for a built in function.
                throw new ODataException(ODataErrorStrings.FunctionCallBinder_BuiltInFunctionMustHaveHaveNullParent(functionCallToken.Name));
            }

            // There are some functions (IsOf and Cast for example) that don't necessarily need to be bound to a BuiltInFunctionSignature,
            // for these, we just Bind them directly to a SingleValueFunctionCallNode
            if (IsUnboundFunction(functionCallToken.Name))
            {
                return CreateUnboundFunctionNode(functionCallToken, argumentNodes, state);
            }

            // Do some validation and get potential built-in functions that could match what we saw
            FunctionSignatureWithReturnType[] signatures = GetBuiltInFunctionSignatures(functionCallToken.Name);
            SingleValueNode[] argumentNodeArray = ValidateArgumentsAreSingleValue(functionCallToken.Name, argumentNodes);
            FunctionSignatureWithReturnType signature = MatchSignatureToBuiltInFunction(functionCallToken.Name, argumentNodeArray, signatures);
            if (signature.ReturnType != null)
            {
                TypePromoteArguments(signature, argumentNodes);
            }

            IEdmTypeReference returnType = signature.ReturnType;

            return new SingleValueFunctionCallNode(functionCallToken.Name, new ReadOnlyCollection<QueryNode>(argumentNodes), returnType);
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
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri Parser does not need to go through the ODL behavior knob.")]
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
            if (identifier.IndexOf(".", StringComparison.Ordinal) == -1)
            {
                return false;
            }

            IEdmOperation operation;
            List<FunctionParameterToken> syntacticArguments = arguments == null ? new List<FunctionParameterToken>() : arguments.ToList();
            if (!FunctionOverloadResolver.ResolveOperationFromList(identifier, syntacticArguments.Select(ar => ar.ParameterName).ToList(), bindingType, state.Model, out operation))
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
            ICollection<FunctionParameterToken> parsedParameters = HandleComplexOrCollectionParameterValueIfExists(state.Configuration.Model, function, syntacticArguments);

            IEnumerable<QueryNode> boundArguments = parsedParameters.Select(p => this.bindMethod(p));
            boundArguments = boundArguments.ToList(); // force enumerable to run : will immediately evaluate all this.bindMethod(p).
            IEdmTypeReference returnType = function.ReturnType;
            IEdmEntitySetBase returnSet = null;
            var singleEntityNode = parent as SingleEntityNode;
            if (singleEntityNode != null)
            {
                returnSet = function.GetTargetEntitySet(singleEntityNode.NavigationSource, state.Model);
            }

            if (returnType.IsEntity())
            {
                boundFunction = new SingleEntityFunctionCallNode(identifier, new[] { function }, boundArguments, (IEdmEntityTypeReference)returnType.Definition.ToTypeReference(), returnSet, parent);
            }
            else if (returnType.IsEntityCollection())
            {
                IEdmCollectionTypeReference collectionTypeReference = (IEdmCollectionTypeReference)returnType;
                boundFunction = new EntityCollectionFunctionCallNode(identifier, new[] { function }, boundArguments, collectionTypeReference, returnSet, parent);
            }
            else if (returnType.IsCollection())
            {
                IEdmCollectionTypeReference collectionTypeReference = (IEdmCollectionTypeReference)returnType;
                boundFunction = new CollectionFunctionCallNode(identifier, new[] { function }, boundArguments, collectionTypeReference, parent);
            }
            else
            {
                boundFunction = new SingleValueFunctionCallNode(identifier, new[] { function }, boundArguments, returnType, parent);
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
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed",
             Justification = "Parameter type is needed here to check whether can be convert from source")]
        internal static List<OperationSegmentParameter> BindSegmentParameters(ODataUriParserConfiguration configuration, IEdmOperation functionOrOpertion, ICollection<FunctionParameterToken> segmentParameterTokens)
        {
            // TODO: HandleComplexOrCollectionParameterValueIfExists  is temp work around for single copmlex or colleciton type, it can't handle nested complex or collection value.
            ICollection<FunctionParameterToken> parametersParsed = FunctionCallBinder.HandleComplexOrCollectionParameterValueIfExists(configuration.Model, functionOrOpertion, segmentParameterTokens, configuration.EnableUriTemplateParsing);

            // Bind it to metadata
            BindingState state = new BindingState(configuration);
            state.ImplicitRangeVariable = null;
            state.RangeVariables.Clear();
            MetadataBinder binder = new MetadataBinder(state);
            List<OperationSegmentParameter> boundParameters = new List<OperationSegmentParameter>();
            foreach (var paraToken in parametersParsed)
            {
                // TODO: considering another better exception
                if (paraToken.ValueToken is EndPathToken)
                {
                    throw new ODataException(Strings.MetadataBinder_ParameterNotInScope(
                        string.Format(CultureInfo.InvariantCulture, "{0}={1}", paraToken.ParameterName, (paraToken.ValueToken as EndPathToken).Identifier)));
                }

                SingleValueNode boundNode = (SingleValueNode)binder.Bind(paraToken.ValueToken);

                // ensure parameter name existis
                var functionParameter = functionOrOpertion.FindParameter(paraToken.ParameterName);
                if (functionParameter == null)
                {
                    throw new ODataException(Strings.ODataParameterWriterCore_ParameterNameNotFoundInOperation(paraToken.ParameterName, functionOrOpertion.Name));
                }

                // ensure node type is compatible with parameter type.
                var sourceTypeReference = boundNode.GetEdmTypeReference();
                bool sourceIsNullOrOpenType = (sourceTypeReference == null);
                if (!sourceIsNullOrOpenType)
                {
                    boundNode = MetadataBindingUtils.ConvertToTypeIfNeeded(boundNode, functionParameter.Type);
                }

                OperationSegmentParameter boundParamer = new OperationSegmentParameter(paraToken.ParameterName, boundNode);
                boundParameters.Add(boundParamer);
            }

            return boundParameters;
        }

        /// <summary>
        /// This is temp work around for $filter $orderby parameter expression which contains complex or collection
        ///     like "Fully.Qualified.Namespace.CanMoveToAddresses(addresses=[{\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"},{\"Street\":\"Pine St.\",\"City\":\"Seattle\"}])";
        /// TODO:  $filter $orderby parameter expression which contains nested complex or collection should NOT be supported in this way
        ///     but should be parsed into token tree, and binded to node tree: parsedParameters.Select(p => this.bindMethod(p));
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="functionOrOpertion">IEdmFunction or IEdmOperation</param>
        /// <param name="parameterTokens">The tokens to bind.</param>
        /// <param name="enableUriTemplateParsing">Whether Uri template parsing is enabled.</param>
        /// <returns>The FunctionParameterTokens with complex or collection values converted from string like "{...}", or "[..,..,..]".</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed",
             Justification = "Parameter type is needed to convert from uri literal.")]
        private static ICollection<FunctionParameterToken> HandleComplexOrCollectionParameterValueIfExists(IEdmModel model, IEdmOperation functionOrOpertion, ICollection<FunctionParameterToken> parameterTokens, bool enableUriTemplateParsing = false)
        {
            ICollection<FunctionParameterToken> partiallyParsedParametersWithComplexOrCollection = new Collection<FunctionParameterToken>();
            foreach (FunctionParameterToken funcParaToken in parameterTokens)
            {
                LiteralToken valueToken = funcParaToken.ValueToken as LiteralToken;
                string valueStr = null;
                if (valueToken != null && (valueStr = valueToken.Value as string) != null)
                {
                    var lexer = new ExpressionLexer(valueStr, true /*moveToFirstToken*/, false /*useSemicolonDelimiter*/, true /*parsingFunctionParameters*/);
                    if (lexer.CurrentToken.Kind == ExpressionTokenKind.BracketedExpression)
                    {
                        var functionParameter = functionOrOpertion.FindParameter(funcParaToken.ParameterName);
                        if (functionParameter == null)
                        {
                            throw new ODataException(Strings.ODataParameterWriterCore_ParameterNameNotFoundInOperation(funcParaToken.ParameterName, functionOrOpertion.Name));
                        }

                        object result;
                        UriTemplateExpression expression;

                        if (enableUriTemplateParsing && UriTemplateParser.TryParseLiteral(lexer.CurrentToken.Text, functionParameter.Type, out expression))
                        {
                            result = expression;
                        }
                        else
                        {
                            // ExpressionTokenKind.BracketedExpression means text like [{\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"},{\"Street\":\"Pine St.\",\"City\":\"Seattle\"}]
                            // so now try convert it into complex or collection type value:
                            result = ODataUriUtils.ConvertFromUriLiteral(valueStr, ODataVersion.V4, model, functionParameter.Type);
                        }

                        LiteralToken newValueToken = new LiteralToken(result);
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
        /// <param name="functionCallToken">original query token for this function</param>
        /// <param name="args">list of already bound query nodes for this function</param>
        /// <param name="state">The current state of the binding algorithm.</param>
        /// <returns>A single value function call node bound to this function.</returns>
        private static SingleValueNode CreateUnboundFunctionNode(FunctionCallToken functionCallToken, List<QueryNode> args, BindingState state)
        {
            // need to figure out the return type and check the correct number of arguments based on the function name
            IEdmTypeReference returnType = null;
            switch (functionCallToken.Name)
            {
                case ExpressionConstants.UnboundFunctionIsOf:
                    {
                        returnType = ValidateAndBuildIsOfArgs(state, ref args);
                        break;
                    }

                case ExpressionConstants.UnboundFunctionCast:
                    {
                        returnType = ValidateAndBuildCastArgs(state, ref args);
                        if (returnType.IsEntity())
                        {
                            IEdmEntityTypeReference returnEntityType = returnType.AsEntity();
                            SingleEntityNode entityNode = args.ElementAt(0) as SingleEntityNode;
                            if (entityNode != null)
                            {
                                return new SingleEntityFunctionCallNode(functionCallToken.Name, args, returnEntityType, entityNode.NavigationSource);
                            }
                        }

                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            // we have everything else we need, so return the new SingleValueFunctionCallNode.
            return new SingleValueFunctionCallNode(functionCallToken.Name, args, returnType);
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
                returnType = TryGetTypeReference(state.Model, typeArgument.Value as string);
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
                        new EntityRangeVariableReferenceNode(
                                                             state.ImplicitRangeVariable.Name,
                                                             state.ImplicitRangeVariable as EntityRangeVariable),
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
                    && !string.Equals(typeArgument.Value as string, Microsoft.OData.Core.Metadata.EdmConstants.EdmStringTypeName, StringComparison.Ordinal))
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
        /// <returns>an IEdmTypeReference for this type string.</returns>
        private static IEdmTypeReference TryGetTypeReference(IEdmModel model, string fullTypeName)
        {
            IEdmTypeReference typeReference = UriEdmHelpers.FindTypeFromModel(model, fullTypeName).ToTypeReference();
            if (typeReference == null)
            {
                return UriEdmHelpers.FindCollectionTypeFromModel(model, fullTypeName);
            }

            return typeReference;
        }
    }
}
