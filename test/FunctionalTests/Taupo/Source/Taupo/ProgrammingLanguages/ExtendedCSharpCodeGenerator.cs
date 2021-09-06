//---------------------------------------------------------------------
// <copyright file="ExtendedCSharpCodeGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.ProgrammingLanguages
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;
    using Microsoft.CSharp;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;

    /// <summary>
    /// Extended C# Code Generator based on CodeDOM with extensions.
    /// </summary>
    public class ExtendedCSharpCodeGenerator : ExtendedCodeGenerator
    {
        /// <summary>
        /// Initializes a new instance of the ExtendedCSharpCodeGenerator class.
        /// </summary>
        public ExtendedCSharpCodeGenerator()
        {
        }

        /// <summary>
        /// Creates the <see cref="ICodeGenerator"/> that this <see cref="ExtendedCodeGenerator"/> wraps.
        /// </summary>
        /// <returns>
        /// The wrapped <see cref="ICodeGenerator"/>.
        /// </returns>
        [SecuritySafeCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Code generators don't hold unmanaged resources.")]
        protected override ICodeGenerator CreateCodeGenerator()
        {
#pragma warning disable 618
            return new CSharpCodeProvider().CreateGenerator();
#pragma warning restore 618
        }

        /// <summary>
        /// Creates the <see cref="CodeDomTreeRewriter"/> that rewrites
        /// custom CodeDOM expression nodes into a facility that the original
        /// CodeDOM code generator can understand.
        /// </summary>
        /// <returns>The <see cref="CodeDomTreeRewriter"/>.</returns>
        protected override CodeDomTreeRewriter CreateRewriter()
        {
            return new ExtendedCSharpTreeRewriter(this);
        }

        /// <summary>
        /// CodeDom tree this.rewriter capable of handling extended CodeDOM nodes (lambdas, anonymous types, etc).
        /// </summary>
        private class ExtendedCSharpTreeRewriter : CodeDomTreeRewriter
        {
            private ExtendedCodeGenerator generator;
            private int nestingLevel = 0;

            internal ExtendedCSharpTreeRewriter(ExtendedCodeGenerator generator)
            {
                this.generator = generator;
            }

            /// <summary>
            /// Rewrites CodeTypeDeclaration
            /// </summary>
            /// <param name="e">A CodeTypeDeclaration to be rewritten.</param>
            /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
            /// <returns>Rewritten CodeTypeDeclaration.</returns>
            /// <remarks>This function just keeps track of nesting level.</remarks>
            protected override CodeTypeDeclaration Rewrite(CodeTypeDeclaration e, ref bool didRewrite)
            {
                this.nestingLevel++;

                try
                {
                    return base.Rewrite(e, ref didRewrite);
                }
                finally
                {
                    this.nestingLevel--;
                }
            }

            /// <summary>
            /// Rewrites CodeNamespace
            /// </summary>
            /// <param name="e">A CodeNamespace to be rewritten.</param>
            /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
            /// <returns>Rewritten CodeNamespace.</returns>
            /// <remarks>This function just keeps track of nesting level.</remarks>
            protected override CodeNamespace Rewrite(CodeNamespace e, ref bool didRewrite)
            {
                this.nestingLevel++;

                try
                {
                    return base.Rewrite(e, ref didRewrite);
                }
                finally
                {
                    this.nestingLevel--;
                }
            }

            /// <summary>
            /// Rewrites CodeMemberMethod
            /// </summary>
            /// <param name="e">A CodeMemberMethod to be rewritten.</param>
            /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
            /// <returns>Rewritten CodeMemberMethod.</returns>
            /// <remarks>This function just keeps track of nesting level.</remarks>
            protected override CodeMemberMethod Rewrite(CodeMemberMethod e, ref bool didRewrite)
            {
                this.nestingLevel++;
                try
                {
                    return base.Rewrite(e, ref didRewrite);
                }
                finally
                {
                    this.nestingLevel--;
                }
            }

            /// <summary>
            /// Rewrites CodeTypeMember
            /// </summary>
            /// <param name="e">A CodeTypeMember to be rewritten.</param>
            /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
            /// <returns>Rewritten CodeTypeMember.</returns>
            protected override CodeTypeMember Rewrite(CodeTypeMember e, ref bool didRewrite)
            {
                CodeMemberAutoImplementedProperty cmap = e as CodeMemberAutoImplementedProperty;
                if (cmap != null)
                {
                    didRewrite = true;
                    return this.RewriteMemberAutoProperty(cmap);
                }

                return base.Rewrite(e, ref didRewrite);
            }

            /// <summary>
            /// Rewrites CodeTypeReference
            /// </summary>
            /// <param name="e">A CodeTypeReference to be rewritten.</param>
            /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
            /// <returns>Rewritten CodeTypeReference.</returns>
            protected override CodeTypeReference Rewrite(CodeTypeReference e, ref bool didRewrite)
            {
                // replace CodeImplicitTypeReference with 'var'
                if (e is CodeImplicitTypeReference)
                {
                    didRewrite = true;
                    return new CodeTypeReference("var");
                }

                return base.Rewrite(e, ref didRewrite);
            }

            /// <summary>
            /// Rewrites CodeExpression
            /// </summary>
            /// <param name="e">A CodeExpression to be rewritten.</param>
            /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
            /// <returns>Rewritten CodeExpression.</returns>
            protected override CodeExpression Rewrite(CodeExpression e, ref bool didRewrite)
            {
                CodeLambdaExpression lambdaExpression = e as CodeLambdaExpression;
                if (lambdaExpression != null)
                {
                    didRewrite = true;
                    return this.RewriteLambdaExpression(lambdaExpression);
                }

                CodeQueryExpression queryExpression = e as CodeQueryExpression;
                if (queryExpression != null)
                {
                    didRewrite = true;
                    return this.RewriteComprehensionExpression(queryExpression);
                }

                CodeCreateAndInitializeObjectExpression objectCreateExpression = e as CodeCreateAndInitializeObjectExpression;
                if (objectCreateExpression != null)
                {
                    didRewrite = true;
                    return this.RewriteCreateAndInitializeObjectExpression(objectCreateExpression);
                }

                CodeArrayInitializerExpression arrayInitializer = e as CodeArrayInitializerExpression;
                if (arrayInitializer != null)
                {
                    didRewrite = true;
                    return this.RewriteArrayInitializerExpression(arrayInitializer);
                }

                CodeExclusiveOrExpression exclusiveOrExpression = e as CodeExclusiveOrExpression;
                if (exclusiveOrExpression != null)
                {
                    didRewrite = true;
                    return this.RewriteExclusiveOrExpression(exclusiveOrExpression);
                }

                CodeAsExpression asExpression = e as CodeAsExpression;
                if (asExpression != null)
                {
                    didRewrite = true;
                    return this.RewriteAsExpression(asExpression);
                }

                CodeIsExpression isExpression = e as CodeIsExpression;
                if (isExpression != null)
                {
                    didRewrite = true;
                    return this.RewriteIsExpression(isExpression);
                }

                CodePrimitiveExpression primitiveExpression = e as CodePrimitiveExpression;
                if (primitiveExpression != null)
                {
                    return this.RewritePrimitiveExpression(primitiveExpression, ref didRewrite);
                }

                var ternaryExpression = e as CodeTernaryExpression;
                if (ternaryExpression != null)
                {
                    didRewrite = true;
                    return this.RewriteTernaryExpression(ternaryExpression);
                }

                var anonymousArray = e as CodeAnonymousArrayExpression;
                if (anonymousArray != null)
                {
                    didRewrite = true;
                    return this.RewriteAnonymousArrayExpression(anonymousArray);
                }

                return base.Rewrite(e, ref didRewrite);
            }

            private CodeExpression RewritePrimitiveExpression(CodePrimitiveExpression source, ref bool didRewrite)
            {
                if (source.Value != null)
                {
                    if (source.Value.GetType() == typeof(float))
                    {
                        var floatValue = (float)source.Value;
                        var stringValue = Convert.ToString(floatValue, CultureInfo.InvariantCulture);
                        if (Convert.ToSingle(stringValue, CultureInfo.InvariantCulture) != floatValue)
                        {
                            didRewrite = true;
                            return new CodeSnippetExpression(((double)floatValue).ToString(CultureInfo.InvariantCulture) + "F");
                        }
                    }
                }

                return base.Rewrite(source, ref didRewrite);
            }

            private CodeSnippetTypeMember RewriteMemberAutoProperty(CodeMemberAutoImplementedProperty autoProperty)
            {
                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    this.OutputCustomAttributes(sw, autoProperty.CustomAttributes);
                    this.WriteIndentation(sw);
                    this.OutputMemberAccessModifier(sw, autoProperty.Attributes);
                    this.OutputVTableModifier(sw, autoProperty.Attributes);
                    this.OutputMemberScopeModifier(sw, autoProperty.Attributes);
                    this.generator.GenerateCodeFromExpression(new CodeTypeReferenceExpression(autoProperty.Type), sw, null);
                    sw.Write(" ");
                    sw.Write(autoProperty.Name);
                    sw.Write(" { ");

                    if ((autoProperty.GetAttributes & MemberAttributes.AccessMask) != MemberAttributes.Public)
                    {
                        this.OutputMemberAccessModifier(sw, autoProperty.GetAttributes);
                    }

                    sw.Write("get; ");

                    if ((autoProperty.SetAttributes & MemberAttributes.AccessMask) != MemberAttributes.Public)
                    {
                        this.OutputMemberAccessModifier(sw, autoProperty.SetAttributes);
                    }

                    sw.Write("set; }");
                    return new CodeSnippetTypeMember(sw.ToString());
                }
            }

            private CodeSnippetExpression RewriteArrayInitializerExpression(CodeArrayInitializerExpression arrayInitializer)
            {
                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    sw.Write("{ ");
                    sw.WriteLine();
                    this.nestingLevel++;
                    try
                    {
                        foreach (CodeExpression expr in arrayInitializer.Values)
                        {
                            this.WriteIndentation(sw);
                            this.generator.GenerateCodeFromExpression(expr, sw, null);
                            sw.WriteLine(", ");
                        }
                    }
                    finally
                    {
                        this.nestingLevel--;
                    }

                    this.WriteIndentation(sw);
                    sw.Write(" }");
                    return new CodeSnippetExpression(sw.ToString());
                }
            }

            private CodeSnippetExpression RewriteCreateAndInitializeObjectExpression(CodeCreateAndInitializeObjectExpression objectCreateExpression)
            {
                string separator = string.Empty;

                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    sw.Write("new");
                    if (objectCreateExpression.ObjectType != null)
                    {
                        sw.Write(" ");
                        sw.Write(this.generator.GetTypeOutput(objectCreateExpression.ObjectType));
                        sw.Write("(");

                        separator = string.Empty;
                        foreach (CodeExpression expr in objectCreateExpression.ConstructorParameters)
                        {
                            sw.Write(separator);
                            separator = ", ";

                            this.generator.GenerateCodeFromExpression(expr, sw, null);
                        }

                        sw.Write(")");
                    }

                    sw.WriteLine();
                    this.WriteIndentation(sw);

                    // We shouldn't write out the '{ }' marker if no property initializers are passed in to the object creation expression
                    if (objectCreateExpression.PropertyInitializers.Any())
                    {
                        sw.WriteLine("{");
                        this.nestingLevel++;
                        try
                        {
                            foreach (var kvp in objectCreateExpression.PropertyInitializers)
                            {
                                this.WriteIndentation(sw);
                                if (kvp.Key != null)
                                {
                                    sw.Write(kvp.Key);
                                    sw.Write(" = ");
                                }

                                this.generator.GenerateCodeFromExpression(kvp.Value, sw, null);
                                sw.WriteLine(", ");
                            }
                        }
                        finally
                        {
                            this.nestingLevel--;
                        }

                        this.WriteIndentation(sw);
                        sw.Write("}");
                    }

                    return new CodeSnippetExpression(sw.ToString());
                }
            }

            /// <summary>
            /// Rewrites the lambda expression.
            /// </summary>
            /// <param name="lambdaExpression">The lambda expression to rewrite.</param>
            /// <returns>Expression rewritten as code snippet</returns>
            private CodeSnippetExpression RewriteLambdaExpression(CodeLambdaExpression lambdaExpression)
            {
                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    if (lambdaExpression.Parameters.Count == 1 && lambdaExpression.Parameters[0].Type is CodeImplicitTypeReference)
                    {
                        sw.Write(this.generator.CreateEscapedIdentifier(lambdaExpression.Parameters[0].Name));
                    }
                    else
                    {
                        sw.Write("(");
                        string separator = string.Empty;
                        foreach (CodeParameterDeclarationExpression cpde in lambdaExpression.Parameters)
                        {
                            sw.Write(separator);
                            separator = ", ";
                            if (!(cpde.Type is CodeImplicitTypeReference))
                            {
                                this.generator.GenerateCodeFromExpression(cpde, sw, null);
                            }
                            else
                            {
                                sw.Write(this.generator.CreateEscapedIdentifier(cpde.Name));
                            }
                        }

                        sw.Write(")");
                    }

                    sw.Write(" => ");
                    bool didRewrite = false;
                    if (lambdaExpression.BodyStatements.Count > 0)
                    {
                        sw.WriteLine();
                        this.WriteIndentation(sw);
                        sw.WriteLine("{");
                        this.nestingLevel++;
                        foreach (CodeStatement stat in lambdaExpression.BodyStatements)
                        {
                            this.WriteIndentation(sw);
                            this.generator.GenerateCodeFromStatement(this.Rewrite(stat, ref didRewrite), sw, null);
                        }

                        this.nestingLevel--;
                        this.WriteIndentation(sw);
                        sw.Write("}");
                        didRewrite = true;
                    }
                    else
                    {
                        this.generator.GenerateCodeFromExpression(this.Rewrite(lambdaExpression.Body, ref didRewrite), sw, null);
                    }

                    return new CodeSnippetExpression(sw.ToString());
                }
            }

            private CodeSnippetExpression RewriteComprehensionExpression(CodeQueryExpression queryExpression)
            {
                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    if (queryExpression.From != null)
                    {
                        sw.Write("(");
                        sw.Write("from " + queryExpression.InputParameterName + " in ");
                        this.generator.GenerateCodeFromExpression(queryExpression.From, sw, null);
                        sw.WriteLine();
                    }
                    
                    if (queryExpression.OrderByKeySelectors.Count != 0)
                    {
                        sw.Write("orderby ");
                        for (int i = 0; i < queryExpression.OrderByKeySelectors.Count; i++)
                        {
                            var keySelector = queryExpression.OrderByKeySelectors[i];
                            this.generator.GenerateCodeFromExpression(keySelector, sw, null);
                            if (queryExpression.AreDescending[i])
                            {
                                sw.Write(" descending");
                            }
                            
                            if (i != queryExpression.OrderByKeySelectors.Count - 1)
                            {
                                sw.Write(", ");
                            }
                        }
                        
                        sw.WriteLine();
                    }
                    
                    if (queryExpression.Where != null)
                    {
                        sw.Write("where ");
                        this.generator.GenerateCodeFromExpression(queryExpression.Where, sw, null);
                        sw.WriteLine();
                    }

                    if (queryExpression.GroupByKeySelector != null)
                    {
                        sw.Write("group " + queryExpression.InputParameterName + " by ");
                        this.generator.GenerateCodeFromExpression(queryExpression.GroupByKeySelector, sw, null);
                        sw.Write(" into " + queryExpression.GroupParameterName);
                        sw.WriteLine();
                    }

                    if (queryExpression.Select != null)
                    {
                        sw.Write("select ");
                        this.generator.GenerateCodeFromExpression(queryExpression.Select, sw, null);
                        sw.Write(")");
                        sw.WriteLine();
                    }
                    else
                    {
                        if (queryExpression.GroupByKeySelector != null)
                        {
                            sw.Write("select " + queryExpression.GroupParameterName);
                            sw.Write(")");
                        }
                        else
                        {
                            sw.Write("select " + queryExpression.InputParameterName);
                            sw.Write(")");
                        }
                        
                        sw.WriteLine();
                    }

                    return new CodeSnippetExpression(sw.ToString());
                }
            }

            /// <summary>
            /// Rewrites exclusive or expression.
            /// </summary>
            /// <param name="exclusiveOrExpression">Expression to rewrite.</param>
            /// <returns>Expression rewritten as code snippet.</returns>
            private CodeSnippetExpression RewriteExclusiveOrExpression(CodeExclusiveOrExpression exclusiveOrExpression)
            {
                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    sw.Write("(");
                    this.generator.GenerateCodeFromExpression(exclusiveOrExpression.Left, sw, null);
                    sw.Write(" ^ ");
                    this.generator.GenerateCodeFromExpression(exclusiveOrExpression.Right, sw, null);
                    sw.Write(")");

                    return new CodeSnippetExpression(sw.ToString());
                }
            }

            /// <summary>
            /// Rewrites as expression.
            /// </summary>
            /// <param name="asExpression">Expression to rewrite.</param>
            /// <returns>Expression rewritten as code snippet.</returns>
            private CodeSnippetExpression RewriteAsExpression(CodeAsExpression asExpression)
            {
                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    sw.Write("(");
                    this.generator.GenerateCodeFromExpression(asExpression.Source, sw, null);
                    sw.Write(" as ");
                    this.generator.GenerateCodeFromExpression(asExpression.TargetType, sw, null);
                    sw.Write(")");

                    return new CodeSnippetExpression(sw.ToString());
                }
            }

            /// <summary>
            /// Rewrites Is expression.
            /// </summary>
            /// <param name="asExpression">Expression to rewrite.</param>
            /// <returns>Expression rewritten as code snippet.</returns>
            private CodeSnippetExpression RewriteIsExpression(CodeIsExpression asExpression)
            {
                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    sw.Write("(");
                    this.generator.GenerateCodeFromExpression(asExpression.Source, sw, null);
                    sw.Write(" is ");
                    this.generator.GenerateCodeFromExpression(asExpression.TargetType, sw, null);
                    sw.Write(")");

                    return new CodeSnippetExpression(sw.ToString());
                }
            }

            private CodeSnippetExpression RewriteTernaryExpression(CodeTernaryExpression ternaryExpression)
            {
                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    sw.Write("(");
                    this.generator.GenerateCodeFromExpression(ternaryExpression.Condition, sw, null);
                    sw.Write(" ? ");
                    this.generator.GenerateCodeFromExpression(ternaryExpression.IfTrue, sw, null);
                    sw.Write(" : ");
                    this.generator.GenerateCodeFromExpression(ternaryExpression.IfFalse, sw, null);
                    sw.Write(")");

                    return new CodeSnippetExpression(sw.ToString());
                }
            }

            private CodeExpression RewriteAnonymousArrayExpression(CodeAnonymousArrayExpression expression)
            {
                // empty string will go through original C# generator to produce something like
                // new  [] { }
                return new CodeArrayCreateExpression(new CodeTypeReference(" "), expression.Elements.ToArray());
            }

            private void OutputVTableModifier(TextWriter output, MemberAttributes attributes)
            {
                switch (attributes & MemberAttributes.VTableMask)
                {
                    case MemberAttributes.New:
                        output.Write("new ");
                        break;
                }
            }

            /// <summary>
            /// Generates code for the specified member access modifier.
            /// </summary>
            /// <param name="output">The output.</param>
            /// <param name="attributes">The attributes.</param>
            private void OutputMemberAccessModifier(TextWriter output, MemberAttributes attributes)
            {
                switch (attributes & MemberAttributes.AccessMask)
                {
                    case MemberAttributes.Assembly:
                        output.Write("internal ");
                        break;
                    case MemberAttributes.FamilyAndAssembly:
                        output.Write("internal ");  /*FamANDAssem*/
                        break;
                    case MemberAttributes.Family:
                        output.Write("protected ");
                        break;
                    case MemberAttributes.FamilyOrAssembly:
                        output.Write("protected internal ");
                        break;
                    case MemberAttributes.Private:
                        output.Write("private ");
                        break;
                    case MemberAttributes.Public:
                        output.Write("public ");
                        break;
                }
            }

            /// <summary>
            /// Generates output for Attributes on customProperties. 
            /// </summary>
            /// <param name="output">Textwriter instance to which it is written</param>
            /// <param name="attributes">Collection of CodeAttributeDeclaration</param>
            private void OutputCustomAttributes(TextWriter output, CodeAttributeDeclarationCollection attributes)
            {
                if (attributes.Count != 0)
                {
                    foreach (CodeAttributeDeclaration current in attributes)
                    {
                        this.WriteIndentation(output);
                        output.Write("[");
                        output.Write(current.AttributeType.BaseType);

                        if (current.Arguments.Count > 0)
                        {
                            output.Write("(");
                            int argumentCount = 0;
                            foreach (CodeAttributeArgument argument in current.Arguments)
                            {
                                if (argumentCount > 0)
                                {
                                    output.Write(", ");
                                }

                                if (!string.IsNullOrEmpty(argument.Name))
                                {
                                    output.Write(argument.Name + " = ");
                                }

                                this.generator.GenerateCodeFromExpression(argument.Value, output, null);
                                argumentCount++;
                            }

                            output.Write(")");
                        }

                        output.Write("]");
                        output.WriteLine();
                    }
                }
            }

            private void OutputMemberScopeModifier(TextWriter output, MemberAttributes attributes)
            {
                switch (attributes & MemberAttributes.ScopeMask)
                {
                    case MemberAttributes.Abstract:
                        output.Write("abstract ");
                        break;

                    case MemberAttributes.Final:
                        output.Write(string.Empty);
                        break;

                    case MemberAttributes.Static:
                        output.Write("static ");
                        break;

                    case MemberAttributes.Override:
                        output.Write("override ");
                        break;

                    default:
                        switch (attributes & MemberAttributes.AccessMask)
                        {
                            case MemberAttributes.Family:
                            case MemberAttributes.Public:
                            case MemberAttributes.Assembly:
                                output.Write("virtual ");
                                break;
                        }

                        break;
                }
            }

            private void WriteIndentation(TextWriter output)
            {
                output.Write(new string(' ', 4 * this.nestingLevel));
            }
        }
    }
}
