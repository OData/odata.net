//---------------------------------------------------------------------
// <copyright file="ExtendedVBCodeGenerator.cs" company="Microsoft">
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
    using System.Reflection;
    using System.Security;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.VisualBasic;

    /// <summary>
    /// Extended VB Code Generator based on CodeDOM with extensions.
    /// </summary>
    public class ExtendedVBCodeGenerator : ExtendedCodeGenerator
    {
        /// <summary>
        /// Initializes a new instance of the ExtendedVBCodeGenerator class.
        /// </summary>
        public ExtendedVBCodeGenerator()
        {
        }

        /// <summary>
        /// Generates code for the specified Code Document Object Model (CodeDOM) compilation unit and
        /// outputs it to the specified text writer using the specified options.
        /// </summary>
        /// <param name="compileUnit">A <see cref="CodeCompileUnit"/> to generate code for.</param>
        /// <param name="writer">The <see cref="TextWriter"/> to output code to.</param>
        /// <param name="options">A <see cref="SafeCodeGeneratorOptions"/> that indicates the options to use for generating code.</param>
        public override void GenerateCodeFromCompileUnit(CodeCompileUnit compileUnit, TextWriter writer, SafeCodeGeneratorOptions options)
        {
            writer.WriteLine("Option Infer");
            base.GenerateCodeFromCompileUnit(compileUnit, writer, options);
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
            return new VBCodeProvider().CreateGenerator();
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
            return new ExtendedVBTreeRewriter(this);
        }

        /// <summary>
        /// CodeDom tree this.rewriter capable of handling extended CodeDOM nodes (lambdas, anonymous types, etc).
        /// </summary>
        private class ExtendedVBTreeRewriter : CodeDomTreeRewriter
        {
            private ExtendedCodeGenerator generator;
            private int nestingLevel = 0;

            internal ExtendedVBTreeRewriter(ExtendedCodeGenerator generator)
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
                    // if the type doesn't already inherit from a base type but implements interfaces , we will need to 
                    // add an Inherits from Object so that VB outputs the right Implements statement.
                    // MSDN Link for this behavior : http://msdn.microsoft.com/en-us/library/system.codedom.codetypedeclaration.basetypes.aspx
                    // "To generate a class in Visual Basic that does not inherit from a base type, but that does implement one or more interfaces, 
                    //  you must include Object as the first item in the BaseTypes collection. "
                    if (e.BaseTypes.Count > 0)
                    {
                        bool hasConcreteBaseType = false;
                        foreach (CodeTypeReference codeTypeRef in e.BaseTypes)
                        {
                            Type baseType = this.GetType(codeTypeRef.BaseType);

                            // If at least one of the basetypes is a concrete type, we dont need to inject the dummy Object base type
                            if (baseType != null && !baseType.IsInterface())
                            {
                                hasConcreteBaseType = true;
                                break;
                            }

                            // If we failed to find the type via Type.GetType() we will assume that the type is a concrete base type
                            if (!codeTypeRef.BaseType.StartsWith("System", StringComparison.Ordinal) && baseType == null)
                            {
                                hasConcreteBaseType = true;
                                break;
                            }
                        }

                        if (!hasConcreteBaseType)
                        {
                            e.BaseTypes.Insert(0, Code.TypeRef<object>());
                        }
                    }

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
            /// Rewrites CodeStatement
            /// </summary>
            /// <param name="source">A CodeStatement to be rewritten.</param>
            /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
            /// <returns>Rewritten CodeStatement.</returns>
            protected override CodeStatement Rewrite(CodeStatement source, ref bool didRewrite)
            {
                CodeVariableDeclarationStatement varDecl = source as CodeVariableDeclarationStatement;
                if (varDecl != null)
                {
                    if (varDecl.Type is CodeImplicitTypeReference)
                    {
                        using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                        {
                            this.WriteIndentation(sw);
                            sw.Write("Dim ");
                            sw.Write(varDecl.Name);
                            sw.Write(" = ");
                            this.generator.GenerateCodeFromExpression(varDecl.InitExpression, sw, null);

                            didRewrite = true;
                            return new CodeSnippetStatement(sw.ToString());
                        }
                    }
                }

                return base.Rewrite(source, ref didRewrite);
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

                CodeBinaryOperatorExpression binaryExpression = e as CodeBinaryOperatorExpression;
                if (binaryExpression != null && this.ShouldRewriteToIdentityComparision(binaryExpression) && binaryExpression.Operator == CodeBinaryOperatorType.IdentityInequality)
                {
                    return this.RewriteIdentityInequalityExpression(binaryExpression, ref didRewrite);
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
            
            /// <summary>
            /// Rewrites CodeBinaryOperatorExpression
            /// </summary>
            /// <param name="source">A CodeBinaryOperatorExpression to be rewritten.</param>
            /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
            /// <returns>Rewritten CodeBinaryOperatorExpression.</returns>
            protected override CodeBinaryOperatorExpression Rewrite(CodeBinaryOperatorExpression source, ref bool didRewrite)
            {
                if (this.ShouldRewriteToIdentityComparision(source) && (source.Operator == CodeBinaryOperatorType.ValueEquality || source.Operator == CodeBinaryOperatorType.IdentityEquality))
                {
                    // if an expression is of the type 
                    // c.ID = Nothing or c.Array  = New Array() 
                    // we rewrite it to be : 
                    // c.ID IS NOTHING or c.Array Is New Array()
                    CodeBinaryOperatorExpression binaryComparisionExpression = this.ConvertToIdentityEquality(source);
                    didRewrite = true;
                    return binaryComparisionExpression;
                }
                else
                {
                    return base.Rewrite(source, ref didRewrite);
                }
            }

            private CodeExpression RewriteIdentityInequalityExpression(CodeBinaryOperatorExpression source, ref bool didRewrite)
            {
                // given an IdentiyInequality Expression which looks like this : 
                // c.Array <> New Array() or c.Array <> Nothing
                // we convert the expression for byte[] types to :
                // NOT ( c.Array Is New Array() ) or NOT ( c.Array Is Nothing ) 
                // For the rest of the types, the expresion remains the same
                string rewrittenExpression;

                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    if (((this.IsNullValue(source.Right) || this.IsNullValue(source.Left)) && (source.Left is CodeCastExpression) && !this.IsPropertyReferenceSystemByte((CodeCastExpression)source.Left))
                        || (source.Right is CodeCastExpression && !this.IsPropertyReferenceSystemByte((CodeCastExpression)source.Right)))
                    {
                        this.generator.GenerateCodeFromExpression(source.Left, sw, null);
                        sw.Write(" <> ");
                        this.generator.GenerateCodeFromExpression(source.Right, sw, null);
                        didRewrite = true;
                    }
                    else
                    {
                        sw.Write("NOT (");
                        this.generator.GenerateCodeFromExpression(source.Left, sw, null);
                        sw.Write(" IS ");
                        this.generator.GenerateCodeFromExpression(source.Right, sw, null);
                        sw.Write(")");
                        didRewrite = true;
                    }

                    rewrittenExpression = sw.ToString();
                }

                return new CodeSnippetExpression(rewrittenExpression);
            }

            private CodeBinaryOperatorExpression ConvertToIdentityEquality(CodeBinaryOperatorExpression source)
            {
                CodeBinaryOperatorExpression binaryComparisionExpression = new CodeBinaryOperatorExpression();
                binaryComparisionExpression.Operator = CodeBinaryOperatorType.IdentityEquality;
                binaryComparisionExpression.Left = this.Rewrite(source.Left);
                binaryComparisionExpression.Right = this.Rewrite(source.Right);
                return binaryComparisionExpression;
            }

            private bool IsPropertyReferenceSystemByte(CodeExpression codeExpression)
            {
                CodeCastExpression castExpression = codeExpression as CodeCastExpression;

                if (castExpression != null)
                {
                    if (castExpression.TargetType.BaseType == "System.Byte")
                    {
                        return true;
                    }
                }

                return false;
            }

            private bool ShouldRewriteToIdentityComparision(CodeBinaryOperatorExpression expression)
            {
                CodeExpression leftHandSide = expression.Left;
                CodeExpression rightHandSide = expression.Right;

                if (this.IsNullValue(rightHandSide) || this.IsNullValue(leftHandSide))
                {
                    return true;
                }
                else
                {
                    return rightHandSide is CodeArrayCreateExpression || leftHandSide is CodeArrayCreateExpression;
                }
            }

            private bool IsNullValue(CodeExpression rightHandSide)
            {
                // if the RHS is Nothing, the statement will be evaluated at compile time and the 
                // condition will always be true, we can delay the comparision until 
                // enumeration of the query by changing the comparision to be Identity Equality
                CodePrimitiveExpression primitiveExpression = rightHandSide as CodePrimitiveExpression;
                return primitiveExpression != null && primitiveExpression.Value == null;
            }

            private CodeSnippetTypeMember RewriteMemberAutoProperty(CodeMemberAutoImplementedProperty autoProperty)
            {
                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    if (autoProperty.GetAttributes != autoProperty.SetAttributes)
                    {
                        // VB doesn't support auto-implemented properties with different access modifiers, so we have
                        // to expand it into a field and a property that gets and sets that field.

                        // Private _PropertyName As Type
                        var fieldName = "_" + autoProperty.Name;
                        this.WriteIndentation(sw);
                        sw.Write("Private " + fieldName + " As ");
                        this.generator.GenerateCodeFromExpression(new CodeTypeReferenceExpression(autoProperty.Type), sw, null);
                        sw.WriteLine();

                        // Public Virtual Property PropetyName As Type
                        this.OutputCustomAttributes(sw, autoProperty.CustomAttributes);
                        this.WriteIndentation(sw);
                        this.OutputMemberAccessModifier(sw, autoProperty.Attributes);
                        this.OutputVTableModifier(sw, autoProperty.Attributes);
                        this.OutputMemberScopeModifier(sw, autoProperty.Attributes);
                        sw.Write("Property ");
                        sw.Write(autoProperty.Name);
                        sw.Write(" As ");
                        this.generator.GenerateCodeFromExpression(new CodeTypeReferenceExpression(autoProperty.Type), sw, null);
                        sw.WriteLine(this.GetArrayPostfix(autoProperty.Type));

                        // Protected Get
                        this.nestingLevel++;
                        this.WriteIndentation(sw);
                        if ((autoProperty.GetAttributes & MemberAttributes.AccessMask) != MemberAttributes.Public)
                        {
                            this.OutputMemberAccessModifier(sw, autoProperty.GetAttributes);
                        }

                        sw.WriteLine("Get");

                        // Return _PropertyName
                        this.nestingLevel++;
                        this.WriteIndentation(sw);
                        sw.Write("Return ");
                        sw.WriteLine(fieldName);

                        // End Get
                        this.nestingLevel--;
                        this.WriteIndentation(sw);
                        sw.WriteLine("End Get");

                        // Private Set(ByVal value As Type)
                        this.WriteIndentation(sw);
                        if ((autoProperty.SetAttributes & MemberAttributes.AccessMask) != MemberAttributes.Public)
                        {
                            this.OutputMemberAccessModifier(sw, autoProperty.SetAttributes);
                        }

                        sw.Write("Set(ByVal value As ");
                        this.generator.GenerateCodeFromExpression(new CodeTypeReferenceExpression(autoProperty.Type), sw, null);
                        sw.Write(this.GetArrayPostfix(autoProperty.Type));
                        sw.WriteLine(")");

                        // Me._PropertyName = value
                        this.nestingLevel++;
                        this.WriteIndentation(sw);
                        sw.Write("Me.");
                        sw.Write(fieldName);
                        sw.WriteLine(" = value");

                        // End Set
                        this.nestingLevel--;
                        this.WriteIndentation(sw);
                        sw.WriteLine("End Set");

                        // End Property
                        this.nestingLevel--;
                        this.WriteIndentation(sw);
                        sw.Write("End Property");
                    }
                    else
                    {
                        this.OutputCustomAttributes(sw, autoProperty.CustomAttributes);
                        this.WriteIndentation(sw);
                        this.OutputMemberAccessModifier(sw, autoProperty.Attributes);
                        this.OutputVTableModifier(sw, autoProperty.Attributes);
                        this.OutputMemberScopeModifier(sw, autoProperty.Attributes);
                        sw.Write("Property ");
                        sw.Write(autoProperty.Name);
                        sw.Write(" As ");
                        this.generator.GenerateCodeFromExpression(new CodeTypeReferenceExpression(autoProperty.Type), sw, null);
                        sw.Write(this.GetArrayPostfix(autoProperty.Type));
                    }

                    return new CodeSnippetTypeMember(sw.ToString());
                }
            }
            
            private string GetArrayPostfix(CodeTypeReference typeRef)
            {
                string s = string.Empty;

                if (typeRef.ArrayElementType != null)
                {
                    s = this.GetArrayPostfix(typeRef.ArrayElementType);
                }

                if (typeRef.ArrayRank > 0)
                {
                    char[] results = new char[typeRef.ArrayRank + 1];
                    results[0] = '(';
                    results[typeRef.ArrayRank] = ')';
                    for (int i = 1; i < typeRef.ArrayRank; i++)
                    {
                        results[i] = ',';
                    }

                    s = new string(results) + s;
                }

                return s;
            }

            private CodeSnippetExpression RewriteArrayInitializerExpression(CodeArrayInitializerExpression arrayInitializer)
            {
                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    sw.Write("{ ");
                    for (int i = 0; i < arrayInitializer.Values.Count; i++)
                    {
                        var expr = arrayInitializer.Values[i];
                        this.generator.GenerateCodeFromExpression(expr, sw, null);
                        if (i < arrayInitializer.Values.Count - 1)
                        {
                            sw.Write(", ");
                        }
                    }
                    
                    sw.Write(" }");
                    return new CodeSnippetExpression(sw.ToString());
                }
            }

            private CodeSnippetExpression RewriteCreateAndInitializeObjectExpression(CodeCreateAndInitializeObjectExpression objectCreateExpression)
            {
                string separator = string.Empty;

                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    sw.Write("New");
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

                    // we shouldn't write out the 'With {' marker if no property initializers are passed in to the object creation expression
                    if (objectCreateExpression.PropertyInitializers.Any())
                    {
                        sw.WriteLine(" With {");
                        this.nestingLevel++;
                        try
                        {
                            int count = 0;

                            foreach (var kvp in objectCreateExpression.PropertyInitializers)
                            {
                                this.WriteIndentation(sw);
                                if (kvp.Key != null)
                                {
                                    sw.Write(".");
                                    sw.Write(kvp.Key);
                                    sw.Write(" = ");
                                }

                                this.generator.GenerateCodeFromExpression(kvp.Value, sw, null);
                                count++;
                                sw.WriteLine(count < objectCreateExpression.PropertyInitializers.Count ? ", " : string.Empty);
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
                    sw.Write(lambdaExpression.HasReturnValue ? "Function (" : "Sub (");

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

                    sw.Write(") ");
                    bool didRewrite = false;
                    if (lambdaExpression.BodyStatements.Count > 0)
                    {
                        sw.WriteLine();
                        this.WriteIndentation(sw);
                        this.nestingLevel++;
                        foreach (CodeStatement stat in lambdaExpression.BodyStatements)
                        {
                            this.WriteIndentation(sw);
                            this.generator.GenerateCodeFromStatement(this.Rewrite(stat, ref didRewrite), sw, null);
                        }

                        this.nestingLevel--;
                        this.WriteIndentation(sw);
                        sw.Write(lambdaExpression.HasReturnValue ? "End Function" : "End Sub");
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
                        sw.Write("From " + queryExpression.InputParameterName + " In ");
                        this.generator.GenerateCodeFromExpression(queryExpression.From, sw, null);
                        sw.Write(" _");
                        sw.WriteLine();
                    }

                    if (queryExpression.OrderByKeySelectors.Count != 0)
                    {
                        sw.Write("Order By ");
                        for (int i = 0; i < queryExpression.OrderByKeySelectors.Count; i++)
                        {
                            var keySelector = queryExpression.OrderByKeySelectors[i];
                            this.generator.GenerateCodeFromExpression(keySelector, sw, null);
                            if (queryExpression.AreDescending[i])
                            {
                                sw.Write(" Descending");
                            }

                            if (i != queryExpression.OrderByKeySelectors.Count - 1)
                            {
                                sw.Write(", ");
                            }
                        }

                        sw.Write(" _");
                        sw.WriteLine();
                    }

                    if (queryExpression.Where != null)
                    {
                        sw.Write("Where ");
                        this.generator.GenerateCodeFromExpression(queryExpression.Where, sw, null);
                        sw.Write(" _");
                        sw.WriteLine();
                    }

                    if (queryExpression.GroupByKeySelector != null)
                    {
                        sw.Write("Group " + queryExpression.InputParameterName + " By ");
                        this.generator.GenerateCodeFromExpression(queryExpression.GroupByKeySelector, sw, null);
                        sw.Write(" Into " + queryExpression.GroupParameterName + " = Group");
                        sw.Write(" _");
                        sw.WriteLine();
                    }

                    if (queryExpression.Select != null)
                    {
                        sw.Write("Select ");
                        this.generator.GenerateCodeFromExpression(queryExpression.Select, sw, null);
                        sw.Write(")");
                    }
                    else
                    {
                        if (queryExpression.GroupByKeySelector != null)
                        {
                            sw.Write("Select " + queryExpression.GroupParameterName);
                            sw.Write(")");
                        }
                        else
                        {
                            sw.Write("Select " + queryExpression.InputParameterName);
                            sw.Write(")");
                        }
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
                    sw.Write(" Xor ");
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
                    sw.Write("TryCast(");
                    this.generator.GenerateCodeFromExpression(asExpression.Source, sw, null);
                    sw.Write(", ");
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

                    // VB.net doesn't allow direct comparision of types such as 
                    // c.Id Is Integer
                    // this needs to be rewritten to :
                    // Typeof DirectCast(c.Id,Object) Is Integer
                    // the type comparision operator can only be used on reference types,
                    // hence the cast to Object before the TypeOf operator is invoked.
                    sw.Write("TypeOf DirectCast(");
                    this.generator.GenerateCodeFromExpression(asExpression.Source, sw, null);
                    sw.Write(" , Object )");
                    sw.Write(" Is ");

                    // Handle the CodeTypeReference issue with Byte[]
                    if (asExpression.TargetType.Type.BaseType == "System.Byte")
                    {
                        sw.Write("Byte()");
                    }
                    else
                    {
                        this.generator.GenerateCodeFromExpression(asExpression.TargetType, sw, null);
                    }

                    sw.Write(")");

                    return new CodeSnippetExpression(sw.ToString());
                }
            }

            private CodeExpression RewritePrimitiveExpression(CodePrimitiveExpression source, ref bool didRewrite)
            {
                if (source.Value != null)
                {
                    if (source.Value.GetType() == typeof(decimal))
                    {
                        didRewrite = true;
                        return this.RewritePrimitiveDecimal((decimal)source.Value);
                    }

                    if (source.Value.GetType() == typeof(long)
                        && long.MinValue == (long)source.Value)
                    {
                        // VB compiler generates "Overflow" error for the long.MinValue: -9223372036854775808
                        didRewrite = true;
                        return new CodeSnippetExpression("Long.MinValue");
                    }

                    if (source.Value.GetType() == typeof(int)
                        && int.MinValue == (int)source.Value)
                    {
                        // VB compiler generates "Overflow" error for the integer.MinValue: -2147483648
                        didRewrite = true;
                        return new CodeSnippetExpression("Integer.MinValue");
                    }

                    if (source.Value.GetType() == typeof(string))
                    {
                        didRewrite = true;
                        return this.RewritePrimitiveString((string)source.Value);
                    }

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

            private CodeSnippetExpression RewritePrimitiveString(string value)
            {
                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    // Escape quote literals
                    value = value.Replace("\"", "\"\"");

                    // Fix to replace the VB specific smart quotes 
                    if (value.IndexOf("“", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        value = value.Replace("“", "\" & Char.ConvertFromUtf32(8220) & \"");
                    }

                    sw.Write(value);

                    // Adding the extra quotes to escape the final string 
                    return new CodeSnippetExpression("\"" + sw.ToString() + "\"");
                }
            }

            private CodeSnippetExpression RewritePrimitiveDecimal(decimal value)
            {
                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    decimal d = value;
                    sw.Write(d);
                    sw.Write("D");
                    return new CodeSnippetExpression(sw.ToString());
                }
            }

            private CodeSnippetExpression RewriteTernaryExpression(CodeTernaryExpression ternaryExpression)
            {
                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                {
                    sw.Write("If(");
                    this.generator.GenerateCodeFromExpression(ternaryExpression.Condition, sw, null);
                    sw.Write(", ");
                    this.generator.GenerateCodeFromExpression(ternaryExpression.IfTrue, sw, null);
                    sw.Write(", ");
                    this.generator.GenerateCodeFromExpression(ternaryExpression.IfFalse, sw, null);
                    sw.Write(")");

                    return new CodeSnippetExpression(sw.ToString());
                }
            }

            private CodeExpression RewriteAnonymousArrayExpression(CodeAnonymousArrayExpression expression)
            {
                var converted = new CodeArrayInitializerExpression();
                foreach (var element in expression.Elements)
                {
                    converted.Add(element);
                }

                return this.RewriteArrayInitializerExpression(converted);
            }

            private void OutputMemberAccessModifier(TextWriter output, MemberAttributes attributes)
            {
                switch (attributes & MemberAttributes.AccessMask)
                {
                    case MemberAttributes.Assembly:
                        output.Write("Friend ");
                        break;
                    case MemberAttributes.FamilyAndAssembly:
                        output.Write("Friend ");
                        break;
                    case MemberAttributes.Family:
                        output.Write("Protected ");
                        break;
                    case MemberAttributes.FamilyOrAssembly:
                        output.Write("Protected Friend ");
                        break;
                    case MemberAttributes.Private:
                        output.Write("Private ");
                        break;
                    case MemberAttributes.Public:
                        output.Write("Public ");
                        break;
                }
            }

            private void OutputVTableModifier(TextWriter output, MemberAttributes attributes)
            {
                switch (attributes & MemberAttributes.VTableMask)
                {
                    case MemberAttributes.New:
                        output.Write("Shadows ");
                        break;
                }
            }

            private void OutputCustomAttributes(TextWriter output, CodeAttributeDeclarationCollection attributes)
            {
                if (attributes.Count != 0)
                {
                    foreach (CodeAttributeDeclaration current in attributes)
                    {
                        this.WriteIndentation(output);
                        output.Write("<");
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
                                    output.Write(argument.Name + " := ");
                                }

                                this.generator.GenerateCodeFromExpression(argument.Value, output, null);
                                argumentCount++;
                            }

                            output.Write(")");
                        }
                        else
                        {
                            output.Write("()");
                        }

                        output.Write(">");

                        output.WriteLine();
                    }
                }
            }

            private void OutputMemberScopeModifier(TextWriter output, MemberAttributes attributes)
            {
                switch (attributes & MemberAttributes.ScopeMask)
                {
                    case MemberAttributes.Abstract:
                        output.Write("MustOverride ");
                        break;
                    case MemberAttributes.Final:
                        output.Write(string.Empty);
                        break;
                    case MemberAttributes.Static:
                        output.Write("Shared ");
                        break;
                    case MemberAttributes.Override:
                        output.Write("Overrides ");
                        break;
                    default:
                        switch (attributes & MemberAttributes.AccessMask)
                        {
                            case MemberAttributes.Family:
                            case MemberAttributes.Public:
                            case MemberAttributes.Assembly:
                                output.Write("Overridable ");
                                break;
                        }

                        break;
                }
            }

            private void WriteIndentation(TextWriter output)
            {
                output.Write(new string(' ', 4 * this.nestingLevel));
            }

            private Type GetType(string clrTypeName)
            {
                Type clrType = Type.GetType(clrTypeName);
                if (clrType == null)
                {
                    foreach (Assembly assemblyInAppDomain in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        clrType = assemblyInAppDomain.GetType(clrTypeName, false, true);
                        if (clrType != null)
                        {
                            break;
                        }
                    }
                }

                return clrType;
            }
        }
    }
}
