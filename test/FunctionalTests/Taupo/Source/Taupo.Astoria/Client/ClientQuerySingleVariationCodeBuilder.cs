//---------------------------------------------------------------------
// <copyright file="ClientQuerySingleVariationCodeBuilder.cs" company="Microsoft">
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
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Builds code for executing DataService queries against context.Execute
    /// </summary>
    public class ClientQuerySingleVariationCodeBuilder : ClientQuerySingleVariationCodeBuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the ClientQuerySingleVariationCodeBuilder class
        /// </summary>
        public ClientQuerySingleVariationCodeBuilder()
        {
            this.IsUri = false;
            this.IsAsync = true;
        }

        /// <summary>
        /// Gets or sets the code builder used to generate code.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaQuerySpanGenerator SpanGenerator { get; set; }

        /// <summary>
        /// Gets or sets the expression expression evaluator.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryExpressionEvaluator Evaluator { get; set; }

        /// <summary>
        /// Gets or sets the FreeVariableExtractor
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ClientQueryFreeVariableAssignmentsExtractingVisitor ClientQueryFreeVariableAssignmentsExtractingVisitor { get; set; }

        /// <summary>
        /// Gets or sets the expression code generator.
        /// </summary>
        /// <value>The expression code generator.</value>
        [InjectDependency(IsRequired = true)]
        public IClientQueryCodeGenerator QueryCodeGenerator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is URI.
        /// </summary>
        /// <value><c>true</c> if this instance is URI; otherwise, <c>false</c>.</value>
        [InjectTestParameter("ExecuteURI", DefaultValueDescription = "false")]
        public bool IsUri { get; set; }

        /// <summary>
        /// Build Override
        /// </summary>
        protected override void BuildOverride()
        {
            // generate source code (CodeDom tree) representing the expression
            var generatedCode = this.QueryCodeGenerator.Generate(this.QueryExpression, this.ContextVariable);

            CodeExpression continuationExpression = this.StartVariation();

            this.BuildClientExecuteCode(continuationExpression, generatedCode);

            this.CodeBuilder.EndVariation();
        }

        /// <summary>
        /// Build a query
        /// </summary>
        /// <param name="continuationExpression">Continuation Expression</param>
        /// <param name="queryVariable">Query Variable</param>
        /// <param name="freeVariableAssignments">Free Variables Assignment</param>
        /// <returns>CodeExpression that can call a Query and verify it</returns>
        private CodeExpression BuildQueryExecuteCallAndVariables(CodeExpression continuationExpression, CodeExpression queryVariable, Dictionary<string, QueryExpression> freeVariableAssignments)       
        {
            // Create variables for the Execute Call (expression was already previously created however)
            QueryValue baselineValue = this.Evaluator.Evaluate(this.QueryExpression, freeVariableAssignments);
            baselineValue = this.SpanGenerator.GenerateSpan(baselineValue, this.ExpandedPaths, this.SelectedPaths);
            string baselineVariableName = this.IIdentifierGenerator.GenerateIdentifier("Baseline");
            var baselineVariable = this.CodeBuilder.AddExternalProperty(baselineVariableName, baselineValue);
            
            CodeExpression uriVariable = null;
            if (this.IsUri)
            {
                uriVariable = this.CodeBuilder.Declare("uri", Code.Primitive(this.Uri));
            }
            
            List<CodeExpression> expressions = new List<CodeExpression>();
            string methodName = null;
            if (this.IsAsync)
            {
                methodName = this.IsUri ? "ExecuteUriAndCompare" : "ExecuteAndCompare";
                expressions.Add(continuationExpression); 
                expressions.Add(Code.Primitive(true));
                expressions.Add(queryVariable);
                if (this.IsUri)
                {
                    expressions.Add(uriVariable);
                }

                expressions.Add(baselineVariable);
                expressions.Add(this.ContextVariable);
                expressions.Add(this.ExpectedClientErrorValue);
            }
            else
            {
                methodName = this.IsUri ? "ExecuteUriAndCompareSync" : "ExecuteAndCompareSync";
                expressions.Add(queryVariable);
                if (this.IsUri)
                {
                    expressions.Add(uriVariable);
                }

                expressions.Add(baselineVariable);
                expressions.Add(this.ContextVariable);
                expressions.Add(this.ExpectedClientErrorValue);
            }

            return this.ResultComparerVariable.Call(methodName, expressions.ToArray());
        }

        private void BuildClientExecuteCode(CodeExpression continuationExpression, CodeExpressionWithFreeVariables generatedCode)
        {
            var freeVariableValues = this.ClientQueryFreeVariableAssignmentsExtractingVisitor.ExtractFreeVariableAssignments(this.QueryExpression);

            // find the number of possible values for all free variables in the expression
            var maxValueCount = 0;

            if (generatedCode.FreeVariables.Any())
            {
                maxValueCount = generatedCode.FreeVariables.Max(fv => fv.PossibleValues.Count);
            }

            var freeVariableAssignments = new Dictionary<string, QueryExpression>();

            // declare free variables and assign their first values
            foreach (var free in generatedCode.FreeVariables)
            {
                this.CodeBuilder.Add(Code.DeclareVariable(free.VariableType, free.Name, free.PossibleValues.First()));
                freeVariableAssignments[free.Name] = freeVariableValues[free.Name][0];
            }

            // var expression = ... generatedQuery
            var queryVariable = this.CodeBuilder.Declare("query", generatedCode.Expression);

            // simple case with 0 or 1 values for free variables
            if (maxValueCount <= 1)
            {
                this.CodeBuilder.Add(this.BuildQueryExecuteCallAndVariables(continuationExpression, queryVariable, freeVariableAssignments));
            }
            else
            {
                // create actionList variable which will hold all verification calls
                // since verification is asynchronous this will let us serialize the calls by doing RunActionSequence
                if (this.IsAsync)
                {
                    this.BuildExecuteAndCompareAsyncCall(freeVariableValues, continuationExpression, generatedCode, maxValueCount, freeVariableAssignments, queryVariable);
                }
                else
                {
                    this.BuildExecuteAndCompareSync(freeVariableValues, continuationExpression, generatedCode, maxValueCount, freeVariableAssignments, queryVariable);
                }
            }
        }

        private void BuildExecuteAndCompareAsyncCall(Dictionary<string, ReadOnlyCollection<QueryExpression>> freeVariableValues, CodeExpression continuationExpression, CodeExpressionWithFreeVariables generatedCode, int maxValueCount, Dictionary<string, QueryExpression> freeVariableAssignments, CodeExpression queryVariable)
        {
            var listVariable = this.CodeBuilder.Declare(
                "actionList",
                Code.New(Code.TypeRef<List<Action<IAsyncContinuation>>>()));

            for (int j = 0; j < maxValueCount; ++j)
            {
                if (j > 0)
                {
                    // for anything but the first value, set the new value of each free variable
                    var lambda = Code.Lambda().WithParameter("c");

                    this.CodeBuilder.BeginLambda(lambda);

                    foreach (var freeVar in generatedCode.FreeVariables)
                    {
                        this.CodeBuilder.Add(Code.Variable(freeVar.Name).Assign(freeVar.PossibleValues[j % freeVar.PossibleValues.Count]));
                    }

                    this.CodeBuilder.Add(Code.Argument("c").Call("Continue"));
                    this.CodeBuilder.EndLambda();
                    this.CodeBuilder.Add(listVariable.Call("Add", lambda));
                }

                foreach (var free in generatedCode.FreeVariables)
                {
                    freeVariableAssignments[free.Name] = freeVariableValues[free.Name][j % freeVariableValues[free.Name].Count];
                }

                var lambdaBody = this.BuildQueryExecuteCallAndVariables(Code.Argument("c"), queryVariable, freeVariableAssignments);

                // add the lambda body to the list
                this.CodeBuilder.Add(listVariable.Call("Add", Code.Lambda().WithParameter("c").WithBody(lambdaBody)));
            }

            // add code to run all lambdas accumulated so far
            this.CodeBuilder.Add(Code.Type(typeof(AsyncHelpers).Name).Call("RunActionSequence", continuationExpression, listVariable));
        }

        private void BuildExecuteAndCompareSync(Dictionary<string, ReadOnlyCollection<QueryExpression>> freeVariableValues, CodeExpression continuationExpression, CodeExpressionWithFreeVariables generatedCode, int maxValueCount, Dictionary<string, QueryExpression> freeVariableAssignments, CodeExpression queryVariable)
        {
            for (int j = 0; j < maxValueCount; ++j)
            {
                foreach (var free in generatedCode.FreeVariables)
                {
                    freeVariableAssignments[free.Name] = freeVariableValues[free.Name][j % freeVariableValues[free.Name].Count];
                }

                this.CodeBuilder.Add(this.BuildQueryExecuteCallAndVariables(continuationExpression, queryVariable, freeVariableAssignments));
            }
        }      
    }
}