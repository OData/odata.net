//---------------------------------------------------------------------
// <copyright file="ClientQueryInvokeActionSingleVariationCodeBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// Builds Client query code for InvokeAction
    /// </summary>
    public class ClientQueryInvokeActionSingleVariationCodeBuilder : ClientQuerySingleVariationCodeBuilderBase
    {
        /// <summary>
        /// Gets or sets the ClientQueryExpressionCodeExpressionConverter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IClientQueryExpressionCodeExpressionConverter ClientQueryExpressionCodeExpressionConverter { get; set; }

        /// <summary>
        /// Determines the HttpMethod to execute based on its type and annotations
        /// </summary>
        /// <param name="function">Function to review</param>
        /// <returns>HttpMethod to use</returns>
        internal static HttpVerb DetermineHttpMethod(Function function)
        {
            var httpMethod = HttpVerb.Post;

            if (function.IsFunction())
            {
                httpMethod = HttpVerb.Get;
            }
            else if (function.IsServiceOperation())
            {
                var legacyServiceOp = function.Annotations.OfType<LegacyServiceOperationAnnotation>().Single();
                httpMethod = (HttpVerb)Enum.Parse(typeof(HttpVerb), legacyServiceOp.Method.ToString(), true);
            }

            return httpMethod;
        }

        /// <summary>
        /// Builds the statements to call execute on the client
        /// </summary>
        /// <param name="continuationExpression">continuation variable</param>
        /// <param name="functionExpression">functionexpression to build</param>
        /// <param name="httpMethod">Method to execute</param>
        /// <returns>List of code statements</returns>
        protected internal virtual IList<CodeStatement> BuildExecuteUriCall(CodeExpression continuationExpression, QueryCustomFunctionCallExpression functionExpression, HttpVerb httpMethod)
        {
            List<CodeStatement> codeStatements = new List<CodeStatement>();
            List<CodeExpression> inputParametersVariables = new List<CodeExpression>();

            ExceptionUtilities.Assert(functionExpression.Arguments.Count == functionExpression.Function.Parameters.Count, "FunctionExpression Argument must have the same number of parameters as Function");

            bool buildFirstParameter = true;
            if (functionExpression.Function.IsAction())
            {
                ServiceOperationAnnotation serviceOperationAnnotation = functionExpression.Function.Annotations.OfType<ServiceOperationAnnotation>().Single();
                if (serviceOperationAnnotation.BindingKind.IsBound())
                {
                    // Skip the first argument for bound action as this is the bound parameter
                    buildFirstParameter = false;

                    // build service operation parameters if calling action on servic operation results
                    QueryCustomFunctionCallExpression serviceOperationExpression = functionExpression.Arguments[0] as QueryCustomFunctionCallExpression;
                    if (serviceOperationExpression != null)
                    {
                        this.AppendFunctionParametersForExecuteUriCall(true, codeStatements, inputParametersVariables, serviceOperationExpression);
                    }
                }
            }

            this.AppendFunctionParametersForExecuteUriCall(buildFirstParameter, codeStatements, inputParametersVariables, functionExpression);

            codeStatements.Add(Code.DeclareVariable("uri", Code.Primitive(this.Uri)));

            if (functionExpression.Function.ReturnType != null)
            {
                var queryClrType = functionExpression.ExpressionType as IQueryClrType;

                var collectionDataType = functionExpression.ExpressionType as QueryCollectionType;
                bool isSingleResult = true;
                if (collectionDataType != null)
                {
                    queryClrType = collectionDataType.ElementType as IQueryClrType;
                    isSingleResult = false;
                }

                ExceptionUtilities.CheckObjectNotNull(queryClrType, "Need to know the CLR Type to query it");

                // (IAsyncContinuation continuation, bool isAsync, string uriString, HttpMethod method, object[] inputParameters, DataServiceContext dataContext, ExpectedClientErrorBaseline clientExpectedError)
                codeStatements.Add(new CodeExpressionStatement(this.ResultComparerVariable.Call(
                    "ExecuteUriAndCompare",
                    new CodeTypeReference[] { Code.TypeRef(queryClrType.ClrType) },
                    continuationExpression,
                    Code.Primitive(this.IsAsync),
                    Code.Variable("uri"),
                    Code.ObjectValue(httpMethod),
                    new CodeArrayCreateExpression(Code.TypeRef("Microsoft.OData.Client.OperationParameter"), inputParametersVariables.ToArray()),
                    Code.Primitive(isSingleResult),
                    this.ContextVariable,
                    this.ExpectedClientErrorValue)));
            }
            else
            {
                // (IAsyncContinuation continuation, bool isAsync, string uriString, HttpMethod method, object[] inputParameters, DataServiceContext dataContext, ExpectedClientErrorBaseline clientExpectedError)
                codeStatements.Add(new CodeExpressionStatement(this.ResultComparerVariable.Call(
                    "ExecuteUriAndCompare",
                    continuationExpression,
                    Code.Primitive(this.IsAsync),
                    Code.Variable("uri"),
                    Code.ObjectValue(httpMethod),
                    new CodeArrayCreateExpression(Code.TypeRef("Microsoft.OData.Client.OperationParameter"), inputParametersVariables.ToArray()),
                    this.ContextVariable,
                    this.ExpectedClientErrorValue)));
            }

            return codeStatements;
        }

        /// <summary>
        /// Build the Invoke Uri
        /// </summary>
        protected override void BuildOverride()
        {
            string queryTestName = this.IIdentifierGenerator.GenerateIdentifier("OperationQuery");
            var continuationExpression = this.CodeBuilder.BeginAsyncVariation(queryTestName);

            var functionExpression = this.QueryExpression.ExtractAction();
            if (functionExpression == null)
            {
                functionExpression = this.QueryExpression.ExtractServiceOperation();
            }

            ExceptionUtilities.CheckObjectNotNull(functionExpression, "Expression must have an action in it");
            var httpMethod = DetermineHttpMethod(functionExpression.Function);

            var codeStatements = this.BuildExecuteUriCall(continuationExpression, functionExpression, httpMethod);

            foreach (var codeStatement in codeStatements)
            {
                this.CodeBuilder.Add(codeStatement);
            }

            this.CodeBuilder.EndVariation();
        }

        /// <summary>
        /// Build parameters for ExecuteUri
        /// </summary>
        /// <param name="buildFirstParameter">Whether to create OperationParameter for the first function parameter</param>
        /// <param name="codeStatements">The code statements</param>
        /// <param name="inputParametersVariables">The parameter variables</param>
        /// <param name="functionExpression">The function expression</param>
        private void AppendFunctionParametersForExecuteUriCall(bool buildFirstParameter, List<CodeStatement> codeStatements, List<CodeExpression> inputParametersVariables, QueryCustomFunctionCallExpression functionExpression)
        {
            int nonBindingParameterStartingIndex = buildFirstParameter ? 0 : 1;
            for (int i = nonBindingParameterStartingIndex; i < functionExpression.Arguments.Count; i++)
            {
                var argument = functionExpression.Arguments[i];
                var functionParameter = functionExpression.Function.Parameters[i];

                ExceptionUtilities.Assert(!(functionParameter.DataType is EntityDataType), "Cannot process entity types");
                ExceptionUtilities.Assert(functionParameter.Mode == FunctionParameterMode.In, "Unsupported function parameter mode");

                // declare the new correct operation parameter type
                var clientParameterTypeRef = Code.TypeRef("Microsoft.OData.Client.UriOperationParameter");
                if (functionExpression.Function.IsAction())
                {
                    clientParameterTypeRef = Code.TypeRef("Microsoft.OData.Client.BodyOperationParameter");
                }

                var argumentExpression = this.ClientQueryExpressionCodeExpressionConverter.Convert(argument);

                // Convert array expression to list. This is necessary since client sends byte[] as binary which is not compatible with collection type in metadata. 
                if (functionParameter.DataType is CollectionDataType)
                {
                    argumentExpression = argumentExpression.Call("ToList");
                    string argumentValueVariableName = functionParameter.Name + "Value";
                    codeStatements.Add(Code.DeclareVariable(argumentValueVariableName, argumentExpression));
                    argumentExpression = Code.Variable(argumentValueVariableName);
                }

                codeStatements.Add(Code.DeclareVariable(functionParameter.Name, Code.New(clientParameterTypeRef, Code.Primitive(functionParameter.Name), argumentExpression)));

                inputParametersVariables.Add(Code.Variable(functionParameter.Name));
            }
        }
    }
}