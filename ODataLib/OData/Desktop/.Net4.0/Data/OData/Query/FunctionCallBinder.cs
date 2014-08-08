//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

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
            DebugUtils.CheckNoExternalCallers(); 
            this.bindMethod = bindMethod;
        }

        /// <summary>
        /// Promotes types of arguments to match signature if possible.
        /// </summary>
        /// <param name="signature">The signature to match the types to.</param>
        /// <param name="argumentNodes">The types to promote.</param>
        internal static void TypePromoteArguments(FunctionSignature signature, List<QueryNode> argumentNodes)
        {
            DebugUtils.CheckNoExternalCallers(); 
            
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
        /// <returns>Returns the types of the arguments provided.</returns>
        internal static IEdmTypeReference[] EnsureArgumentsAreSingleValue(string functionName, List<QueryNode> argumentNodes)
        {
            DebugUtils.CheckNoExternalCallers(); 
            ExceptionUtils.CheckArgumentNotNull(functionName, "functionCallToken");
            ExceptionUtils.CheckArgumentNotNull(argumentNodes, "argumentNodes");

            // Right now all functions take a single value for all arguments
            IEdmTypeReference[] argumentTypes = new IEdmTypeReference[argumentNodes.Count];
            for (int i = 0; i < argumentNodes.Count; i++)
            {
                SingleValueNode argumentNode = argumentNodes[i] as SingleValueNode;
                if (argumentNode == null)
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_FunctionArgumentNotSingleValue(functionName));
                }

                argumentTypes[i] = argumentNode.TypeReference;
            }

            return argumentTypes;
        }

        /// <summary>
        /// Finds the signature that best matches the arguments
        /// </summary>
        /// <param name="functionName">The name of the function</param>
        /// <param name="argumentTypes">The types of the arguments</param>
        /// <param name="signatures">The signatures to match against</param>
        /// <returns>Returns the matching signature or throws</returns>
        internal static FunctionSignatureWithReturnType MatchSignatureToBuiltInFunction(string functionName, IEdmTypeReference[] argumentTypes, FunctionSignatureWithReturnType[] signatures)
        {
            DebugUtils.CheckNoExternalCallers();
            FunctionSignatureWithReturnType signature;

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
                signature = TypePromotionUtils.FindBestFunctionSignature(signatures, argumentTypes);
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
            DebugUtils.CheckNoExternalCallers(); 

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
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(functionCallToken, "functionCallToken");
            ExceptionUtils.CheckArgumentNotNull(functionCallToken.Name, "functionCallToken.Name");

            // Bind the parent, if present.
            // TODO: parent can be a collection as well, so we need to loosen this to QueryNode.
            QueryNode parent = null;
            if (functionCallToken.Source != null)
            {
                parent = this.bindMethod(functionCallToken.Source);
            }
            else
            {
                parent = NodeFactory.CreateRangeVariableReferenceNode(state.ImplicitRangeVariable);
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            IEdmTypeReference[] argumentTypes = EnsureArgumentsAreSingleValue(functionCallToken.Name, argumentNodes);

            FunctionSignatureWithReturnType signature = MatchSignatureToBuiltInFunction(functionCallToken.Name, argumentTypes, signatures);
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
        /// <param name="boundFunction">a single value function call node representing this funciton call, if we found one.</param>
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
            
            IEdmFunctionImport functionImport;
            List<FunctionParameterToken> syntacticArguments = arguments == null ? new List<FunctionParameterToken>() : arguments.ToList();
            if (!FunctionOverloadResolver.ResolveFunctionsFromList(identifier, syntacticArguments.Select(ar => ar.ParameterName).ToList(), bindingType, state.Model, out functionImport))
            {
                return false;
            }

            if (singleValueParent != null && singleValueParent.TypeReference == null)
            {
                // if the parent exists, but has no type information, then we're in open type land, and we 
                // shouldn't go any farther.
                throw new ODataException(ODataErrorStrings.FunctionCallBinder_CallingFunctionOnOpenProperty(identifier));
            }

            if (functionImport.IsSideEffecting)
            {
                return false;
            }

            ICollection<FunctionParameterToken> parsedParameters;
            if (!FunctionParameterParser.TryParseFunctionParameters(syntacticArguments, state.Configuration, functionImport, out parsedParameters))
            {
                return false;
            }

            IEnumerable<QueryNode> boundArguments = parsedParameters.Select(p => this.bindMethod(p));

            IEdmTypeReference returnType = functionImport.ReturnType;
            IEdmEntitySet returnSet = null;
            var singleEntityNode = parent as SingleEntityNode;
            if (singleEntityNode != null)
            {
                returnSet = functionImport.GetTargetEntitySet(singleEntityNode.EntitySet, state.Model);
            }

            if (returnType.IsEntity())
            {
                boundFunction = new SingleEntityFunctionCallNode(identifier, new[] { functionImport }, boundArguments, (IEdmEntityTypeReference)returnType.Definition.ToTypeReference(), returnSet, parent);
            }
            else if (returnType.IsEntityCollection())
            {
                IEdmCollectionTypeReference collectionTypeReference = (IEdmCollectionTypeReference)returnType;
                boundFunction = new EntityCollectionFunctionCallNode(identifier, new[] { functionImport }, boundArguments, collectionTypeReference, returnSet, parent);
            }
            else if (returnType.IsCollection())
            {
                IEdmCollectionTypeReference collectionTypeReference = (IEdmCollectionTypeReference)returnType;
                boundFunction = new CollectionFunctionCallNode(identifier, new[] { functionImport }, boundArguments, collectionTypeReference, parent);
            }
            else
            {
                boundFunction = new SingleValueFunctionCallNode(identifier, new[] { functionImport }, boundArguments, returnType, parent);
            }

            return true;
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
                            return new SingleEntityFunctionCallNode(functionCallToken.Name, args, returnEntityType, entityNode.EntitySet);
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
