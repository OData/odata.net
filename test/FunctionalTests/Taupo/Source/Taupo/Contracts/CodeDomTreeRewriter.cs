//---------------------------------------------------------------------
// <copyright file="CodeDomTreeRewriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

// this code has been generated from T4 template 'CodeDomTreeRewriter.tt' file. 
// Do not modify the file, modify template instead.
namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using System.CodeDom;
    using System.Collections;
    using System.Collections.Specialized;

    /// <summary>
    /// Rewriting visitor for CodeDom trees.
    /// </summary>
    /// <remarks>
    /// Current implementation always copies the input.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Code is generated")]
    public class CodeDomTreeRewriter
    {
        /// <summary>
        /// Initializes a new instance of the CodeDomTreeRewriter class.
        /// </summary>
        public CodeDomTreeRewriter()
        {
        }

        /// <summary>
        /// Rewrites CodeExpression
        /// </summary>
        /// <param name="source">A CodeExpression to be rewritten.</param>
        /// <returns>Rewritten CodeExpression.</returns>
        public CodeExpression Rewrite(CodeExpression source)
        {
            bool didRewrite = false;
            return this.Rewrite(source, ref didRewrite);
        }

        /// <summary>
        /// Rewrites CodeCompileUnit
        /// </summary>
        /// <param name="source">A CodeCompileUnit to be rewritten.</param>
        /// <returns>Rewritten CodeCompileUnit.</returns>
        public CodeCompileUnit Rewrite(CodeCompileUnit source)
        {
            bool didRewrite = false;
            return this.Rewrite(source, ref didRewrite);
        }

        /// <summary>
        /// Rewrites CodeNamespace
        /// </summary>
        /// <param name="source">A CodeNamespace to be rewritten.</param>
        /// <returns>Rewritten CodeNamespace.</returns>
        public CodeNamespace Rewrite(CodeNamespace source)
        {
            bool didRewrite = false;
            return this.Rewrite(source, ref didRewrite);
        }

        /// <summary>
        /// Rewrites CodeTypeDeclaration
        /// </summary>
        /// <param name="source">A CodeTypeDeclaration to be rewritten.</param>
        /// <returns>Rewritten CodeTypeDeclaration.</returns>
        public CodeTypeDeclaration Rewrite(CodeTypeDeclaration source)
        {
            bool didRewrite = false;
            return this.Rewrite(source, ref didRewrite);
        }

        /// <summary>
        /// Rewrites CodeStatement
        /// </summary>
        /// <param name="source">A CodeStatement to be rewritten.</param>
        /// <returns>Rewritten CodeStatement.</returns>
        public CodeStatement Rewrite(CodeStatement source)
        {
            bool didRewrite = false;
            return this.Rewrite(source, ref didRewrite);
        }

        /// <summary>
        /// Rewrites CodeTypeReference
        /// </summary>
        /// <param name="source">A CodeTypeReference to be rewritten.</param>
        /// <returns>Rewritten CodeTypeReference.</returns>
        public CodeTypeReference Rewrite(CodeTypeReference source)
        {
            bool didRewrite = false;
            return this.Rewrite(source, ref didRewrite);
        }

        /// <summary>
        /// Rewrites CodeArgumentReferenceExpression
        /// </summary>
        /// <param name="source">A CodeArgumentReferenceExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeArgumentReferenceExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeArgumentReferenceExpression Rewrite(CodeArgumentReferenceExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeArgumentReferenceExpression result = new CodeArgumentReferenceExpression();
            result.ParameterName = source.ParameterName;
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeArrayCreateExpression
        /// </summary>
        /// <param name="source">A CodeArrayCreateExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeArrayCreateExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeArrayCreateExpression Rewrite(CodeArrayCreateExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeArrayCreateExpression result = new CodeArrayCreateExpression();
            result.CreateType = this.Rewrite(source.CreateType, ref didChildRewrite);
            this.Rewrite(result.Initializers, source.Initializers, ref didChildRewrite);
            result.Size = source.Size;
            result.SizeExpression = this.Rewrite(source.SizeExpression, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeArrayIndexerExpression
        /// </summary>
        /// <param name="source">A CodeArrayIndexerExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeArrayIndexerExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeArrayIndexerExpression Rewrite(CodeArrayIndexerExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeArrayIndexerExpression result = new CodeArrayIndexerExpression();
            result.TargetObject = this.Rewrite(source.TargetObject, ref didChildRewrite);
            this.Rewrite(result.Indices, source.Indices, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeAssignStatement
        /// </summary>
        /// <param name="source">A CodeAssignStatement to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeAssignStatement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeAssignStatement Rewrite(CodeAssignStatement source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeAssignStatement result = new CodeAssignStatement();
            result.Left = this.Rewrite(source.Left, ref didChildRewrite);
            result.Right = this.Rewrite(source.Right, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeAttachEventStatement
        /// </summary>
        /// <param name="source">A CodeAttachEventStatement to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeAttachEventStatement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeAttachEventStatement Rewrite(CodeAttachEventStatement source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeAttachEventStatement result = new CodeAttachEventStatement();
            result.Event = this.Rewrite(source.Event, ref didChildRewrite);
            result.Listener = this.Rewrite(source.Listener, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeAttributeArgument
        /// </summary>
        /// <param name="source">A CodeAttributeArgument to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeAttributeArgument.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeAttributeArgument Rewrite(CodeAttributeArgument source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeAttributeArgument result = new CodeAttributeArgument();
            result.Name = source.Name;
            result.Value = this.Rewrite(source.Value, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeAttributeArgumentCollection
        /// </summary>
        /// <param name="target">Target collection.</param>
        /// <param name="source">Source collection.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(CodeAttributeArgumentCollection target, CodeAttributeArgumentCollection source, ref bool didRewrite)
        {
            foreach (CodeAttributeArgument item in source)
            {
                target.Add(this.Rewrite(item, ref didRewrite));
            }
        }

        /// <summary>
        /// Rewrites CodeAttributeDeclarationCollection
        /// </summary>
        /// <param name="target">Target collection.</param>
        /// <param name="source">Source collection.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(CodeAttributeDeclarationCollection target, CodeAttributeDeclarationCollection source, ref bool didRewrite)
        {
            foreach (CodeAttributeDeclaration item in source)
            {
                target.Add(this.Rewrite(item, ref didRewrite));
            }
        }

        /// <summary>
        /// Rewrites CodeBaseReferenceExpression
        /// </summary>
        /// <param name="source">A CodeBaseReferenceExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeBaseReferenceExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeBaseReferenceExpression Rewrite(CodeBaseReferenceExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeBaseReferenceExpression result = new CodeBaseReferenceExpression();
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeBinaryOperatorExpression
        /// </summary>
        /// <param name="source">A CodeBinaryOperatorExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeBinaryOperatorExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeBinaryOperatorExpression Rewrite(CodeBinaryOperatorExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeBinaryOperatorExpression result = new CodeBinaryOperatorExpression();
            result.Right = this.Rewrite(source.Right, ref didChildRewrite);
            result.Left = this.Rewrite(source.Left, ref didChildRewrite);
            result.Operator = this.Rewrite(source.Operator, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeBinaryOperatorType
        /// </summary>
        /// <param name="source">A CodeBinaryOperatorType to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeBinaryOperatorType.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeBinaryOperatorType Rewrite(CodeBinaryOperatorType source, ref bool didRewrite)
        {
            bool didChildRewrite = false;
            CodeBinaryOperatorType result = new CodeBinaryOperatorType();
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeCastExpression
        /// </summary>
        /// <param name="source">A CodeCastExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeCastExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeCastExpression Rewrite(CodeCastExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeCastExpression result = new CodeCastExpression();
            result.TargetType = this.Rewrite(source.TargetType, ref didChildRewrite);
            result.Expression = this.Rewrite(source.Expression, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeCatchClause
        /// </summary>
        /// <param name="source">A CodeCatchClause to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeCatchClause.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeCatchClause Rewrite(CodeCatchClause source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeCatchClause result = new CodeCatchClause();
            result.LocalName = source.LocalName;
            result.CatchExceptionType = this.Rewrite(source.CatchExceptionType, ref didChildRewrite);
            this.Rewrite(result.Statements, source.Statements, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeCatchClauseCollection
        /// </summary>
        /// <param name="target">Target collection.</param>
        /// <param name="source">Source collection.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(CodeCatchClauseCollection target, CodeCatchClauseCollection source, ref bool didRewrite)
        {
            foreach (CodeCatchClause item in source)
            {
                target.Add(this.Rewrite(item, ref didRewrite));
            }
        }

        /// <summary>
        /// Rewrites CodeChecksumPragma
        /// </summary>
        /// <param name="source">A CodeChecksumPragma to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeChecksumPragma.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeChecksumPragma Rewrite(CodeChecksumPragma source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeChecksumPragma result = new CodeChecksumPragma();
            result.FileName = source.FileName;
            result.ChecksumAlgorithmId = source.ChecksumAlgorithmId;
            result.ChecksumData = source.ChecksumData;
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeComment
        /// </summary>
        /// <param name="source">A CodeComment to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeComment.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeComment Rewrite(CodeComment source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeComment result = new CodeComment();
            result.DocComment = source.DocComment;
            result.Text = source.Text;
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeCommentStatement
        /// </summary>
        /// <param name="source">A CodeCommentStatement to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeCommentStatement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeCommentStatement Rewrite(CodeCommentStatement source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeCommentStatement result = new CodeCommentStatement();
            result.Comment = this.Rewrite(source.Comment, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeCommentStatementCollection
        /// </summary>
        /// <param name="target">Target collection.</param>
        /// <param name="source">Source collection.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(CodeCommentStatementCollection target, CodeCommentStatementCollection source, ref bool didRewrite)
        {
            foreach (CodeCommentStatement item in source)
            {
                target.Add(this.Rewrite(item, ref didRewrite));
            }
        }

        /// <summary>
        /// Rewrites CodeCompileUnit
        /// </summary>
        /// <param name="source">A CodeCompileUnit to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeCompileUnit.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeCompileUnit Rewrite(CodeCompileUnit source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            if (source is CodeSnippetCompileUnit)
            {
                return this.Rewrite((CodeSnippetCompileUnit)source, ref didRewrite);
            }

            bool didChildRewrite = false;
            CodeCompileUnit result = new CodeCompileUnit();
            this.Rewrite(result.Namespaces, source.Namespaces, ref didChildRewrite);
            this.Rewrite(result.ReferencedAssemblies, source.ReferencedAssemblies, ref didChildRewrite);
            this.Rewrite(result.AssemblyCustomAttributes, source.AssemblyCustomAttributes, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeConditionStatement
        /// </summary>
        /// <param name="source">A CodeConditionStatement to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeConditionStatement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeConditionStatement Rewrite(CodeConditionStatement source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeConditionStatement result = new CodeConditionStatement();
            result.Condition = this.Rewrite(source.Condition, ref didChildRewrite);
            this.Rewrite(result.TrueStatements, source.TrueStatements, ref didChildRewrite);
            this.Rewrite(result.FalseStatements, source.FalseStatements, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeConstructor
        /// </summary>
        /// <param name="source">A CodeConstructor to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeConstructor.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeConstructor Rewrite(CodeConstructor source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeConstructor result = new CodeConstructor();
            this.Rewrite(result.BaseConstructorArgs, source.BaseConstructorArgs, ref didChildRewrite);
            this.Rewrite(result.ChainedConstructorArgs, source.ChainedConstructorArgs, ref didChildRewrite);
            result.ReturnType = this.Rewrite(source.ReturnType, ref didChildRewrite);
            this.Rewrite(result.Statements, source.Statements, ref didChildRewrite);
            this.Rewrite(result.Parameters, source.Parameters, ref didChildRewrite);
            result.PrivateImplementationType = this.Rewrite(source.PrivateImplementationType, ref didChildRewrite);
            this.Rewrite(result.ImplementationTypes, source.ImplementationTypes, ref didChildRewrite);
            this.Rewrite(result.ReturnTypeCustomAttributes, source.ReturnTypeCustomAttributes, ref didChildRewrite);
            this.Rewrite(result.TypeParameters, source.TypeParameters, ref didChildRewrite);
            result.Name = source.Name;
            result.Attributes = this.Rewrite(source.Attributes, ref didChildRewrite);
            this.Rewrite(result.CustomAttributes, source.CustomAttributes, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.Comments, source.Comments, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeDefaultValueExpression
        /// </summary>
        /// <param name="source">A CodeDefaultValueExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeDefaultValueExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeDefaultValueExpression Rewrite(CodeDefaultValueExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeDefaultValueExpression result = new CodeDefaultValueExpression();
            result.Type = this.Rewrite(source.Type, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeDelegateCreateExpression
        /// </summary>
        /// <param name="source">A CodeDelegateCreateExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeDelegateCreateExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeDelegateCreateExpression Rewrite(CodeDelegateCreateExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeDelegateCreateExpression result = new CodeDelegateCreateExpression();
            result.DelegateType = this.Rewrite(source.DelegateType, ref didChildRewrite);
            result.TargetObject = this.Rewrite(source.TargetObject, ref didChildRewrite);
            result.MethodName = source.MethodName;
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeDelegateInvokeExpression
        /// </summary>
        /// <param name="source">A CodeDelegateInvokeExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeDelegateInvokeExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeDelegateInvokeExpression Rewrite(CodeDelegateInvokeExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeDelegateInvokeExpression result = new CodeDelegateInvokeExpression();
            result.TargetObject = this.Rewrite(source.TargetObject, ref didChildRewrite);
            this.Rewrite(result.Parameters, source.Parameters, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeDirectionExpression
        /// </summary>
        /// <param name="source">A CodeDirectionExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeDirectionExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeDirectionExpression Rewrite(CodeDirectionExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeDirectionExpression result = new CodeDirectionExpression();
            result.Expression = this.Rewrite(source.Expression, ref didChildRewrite);
            result.Direction = this.Rewrite(source.Direction, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeDirective
        /// </summary>
        /// <param name="source">A CodeDirective to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeDirective.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeDirective Rewrite(CodeDirective source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            if (source is CodeChecksumPragma)
            {
                return this.Rewrite((CodeChecksumPragma)source, ref didRewrite);
            }

            if (source is CodeRegionDirective)
            {
                return this.Rewrite((CodeRegionDirective)source, ref didRewrite);
            }

            bool didChildRewrite = false;
            CodeDirective result = new CodeDirective();
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeDirectiveCollection
        /// </summary>
        /// <param name="target">Target collection.</param>
        /// <param name="source">Source collection.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(CodeDirectiveCollection target, CodeDirectiveCollection source, ref bool didRewrite)
        {
            foreach (CodeDirective item in source)
            {
                target.Add(this.Rewrite(item, ref didRewrite));
            }
        }

        /// <summary>
        /// Rewrites CodeEntryPointMethod
        /// </summary>
        /// <param name="source">A CodeEntryPointMethod to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeEntryPointMethod.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeEntryPointMethod Rewrite(CodeEntryPointMethod source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeEntryPointMethod result = new CodeEntryPointMethod();
            result.ReturnType = this.Rewrite(source.ReturnType, ref didChildRewrite);
            this.Rewrite(result.Statements, source.Statements, ref didChildRewrite);
            this.Rewrite(result.Parameters, source.Parameters, ref didChildRewrite);
            result.PrivateImplementationType = this.Rewrite(source.PrivateImplementationType, ref didChildRewrite);
            this.Rewrite(result.ImplementationTypes, source.ImplementationTypes, ref didChildRewrite);
            this.Rewrite(result.ReturnTypeCustomAttributes, source.ReturnTypeCustomAttributes, ref didChildRewrite);
            this.Rewrite(result.TypeParameters, source.TypeParameters, ref didChildRewrite);
            result.Name = source.Name;
            result.Attributes = this.Rewrite(source.Attributes, ref didChildRewrite);
            this.Rewrite(result.CustomAttributes, source.CustomAttributes, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.Comments, source.Comments, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeEventReferenceExpression
        /// </summary>
        /// <param name="source">A CodeEventReferenceExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeEventReferenceExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeEventReferenceExpression Rewrite(CodeEventReferenceExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeEventReferenceExpression result = new CodeEventReferenceExpression();
            result.TargetObject = this.Rewrite(source.TargetObject, ref didChildRewrite);
            result.EventName = source.EventName;
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeExpression
        /// </summary>
        /// <param name="source">A CodeExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeExpression Rewrite(CodeExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            if (source is CodeArgumentReferenceExpression)
            {
                return this.Rewrite((CodeArgumentReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeArrayCreateExpression)
            {
                return this.Rewrite((CodeArrayCreateExpression)source, ref didRewrite);
            }

            if (source is CodeArrayIndexerExpression)
            {
                return this.Rewrite((CodeArrayIndexerExpression)source, ref didRewrite);
            }

            if (source is CodeBaseReferenceExpression)
            {
                return this.Rewrite((CodeBaseReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeBinaryOperatorExpression)
            {
                return this.Rewrite((CodeBinaryOperatorExpression)source, ref didRewrite);
            }

            if (source is CodeCastExpression)
            {
                return this.Rewrite((CodeCastExpression)source, ref didRewrite);
            }

            if (source is CodeDefaultValueExpression)
            {
                return this.Rewrite((CodeDefaultValueExpression)source, ref didRewrite);
            }

            if (source is CodeDelegateCreateExpression)
            {
                return this.Rewrite((CodeDelegateCreateExpression)source, ref didRewrite);
            }

            if (source is CodeDelegateInvokeExpression)
            {
                return this.Rewrite((CodeDelegateInvokeExpression)source, ref didRewrite);
            }

            if (source is CodeDirectionExpression)
            {
                return this.Rewrite((CodeDirectionExpression)source, ref didRewrite);
            }

            if (source is CodeEventReferenceExpression)
            {
                return this.Rewrite((CodeEventReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeFieldReferenceExpression)
            {
                return this.Rewrite((CodeFieldReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeIndexerExpression)
            {
                return this.Rewrite((CodeIndexerExpression)source, ref didRewrite);
            }

            if (source is CodeMethodInvokeExpression)
            {
                return this.Rewrite((CodeMethodInvokeExpression)source, ref didRewrite);
            }

            if (source is CodeMethodReferenceExpression)
            {
                return this.Rewrite((CodeMethodReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeObjectCreateExpression)
            {
                return this.Rewrite((CodeObjectCreateExpression)source, ref didRewrite);
            }

            if (source is CodeParameterDeclarationExpression)
            {
                return this.Rewrite((CodeParameterDeclarationExpression)source, ref didRewrite);
            }

            if (source is CodePrimitiveExpression)
            {
                return this.Rewrite((CodePrimitiveExpression)source, ref didRewrite);
            }

            if (source is CodePropertyReferenceExpression)
            {
                return this.Rewrite((CodePropertyReferenceExpression)source, ref didRewrite);
            }

            if (source is CodePropertySetValueReferenceExpression)
            {
                return this.Rewrite((CodePropertySetValueReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeSnippetExpression)
            {
                return this.Rewrite((CodeSnippetExpression)source, ref didRewrite);
            }

            if (source is CodeThisReferenceExpression)
            {
                return this.Rewrite((CodeThisReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeTypeOfExpression)
            {
                return this.Rewrite((CodeTypeOfExpression)source, ref didRewrite);
            }

            if (source is CodeTypeReferenceExpression)
            {
                return this.Rewrite((CodeTypeReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeVariableReferenceExpression)
            {
                return this.Rewrite((CodeVariableReferenceExpression)source, ref didRewrite);
            }

            bool didChildRewrite = false;
            CodeExpression result = new CodeExpression();
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeExpressionCollection
        /// </summary>
        /// <param name="target">Target collection.</param>
        /// <param name="source">Source collection.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(CodeExpressionCollection target, CodeExpressionCollection source, ref bool didRewrite)
        {
            foreach (CodeExpression item in source)
            {
                target.Add(this.Rewrite(item, ref didRewrite));
            }
        }

        /// <summary>
        /// Rewrites CodeExpressionStatement
        /// </summary>
        /// <param name="source">A CodeExpressionStatement to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeExpressionStatement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeExpressionStatement Rewrite(CodeExpressionStatement source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeExpressionStatement result = new CodeExpressionStatement();
            result.Expression = this.Rewrite(source.Expression, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeFieldReferenceExpression
        /// </summary>
        /// <param name="source">A CodeFieldReferenceExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeFieldReferenceExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeFieldReferenceExpression Rewrite(CodeFieldReferenceExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeFieldReferenceExpression result = new CodeFieldReferenceExpression();
            result.TargetObject = this.Rewrite(source.TargetObject, ref didChildRewrite);
            result.FieldName = source.FieldName;
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeGotoStatement
        /// </summary>
        /// <param name="source">A CodeGotoStatement to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeGotoStatement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeGotoStatement Rewrite(CodeGotoStatement source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeGotoStatement result = new CodeGotoStatement();
            result.Label = source.Label;
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeIndexerExpression
        /// </summary>
        /// <param name="source">A CodeIndexerExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeIndexerExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeIndexerExpression Rewrite(CodeIndexerExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeIndexerExpression result = new CodeIndexerExpression();
            result.TargetObject = this.Rewrite(source.TargetObject, ref didChildRewrite);
            this.Rewrite(result.Indices, source.Indices, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeIterationStatement
        /// </summary>
        /// <param name="source">A CodeIterationStatement to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeIterationStatement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeIterationStatement Rewrite(CodeIterationStatement source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeIterationStatement result = new CodeIterationStatement();
            result.InitStatement = this.Rewrite(source.InitStatement, ref didChildRewrite);
            result.TestExpression = this.Rewrite(source.TestExpression, ref didChildRewrite);
            result.IncrementStatement = this.Rewrite(source.IncrementStatement, ref didChildRewrite);
            this.Rewrite(result.Statements, source.Statements, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeLabeledStatement
        /// </summary>
        /// <param name="source">A CodeLabeledStatement to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeLabeledStatement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeLabeledStatement Rewrite(CodeLabeledStatement source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeLabeledStatement result = new CodeLabeledStatement();
            result.Label = source.Label;
            result.Statement = this.Rewrite(source.Statement, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeLinePragma
        /// </summary>
        /// <param name="source">A CodeLinePragma to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeLinePragma.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeLinePragma Rewrite(CodeLinePragma source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeLinePragma result = new CodeLinePragma();
            result.FileName = source.FileName;
            result.LineNumber = source.LineNumber;
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeMemberEvent
        /// </summary>
        /// <param name="source">A CodeMemberEvent to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeMemberEvent.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeMemberEvent Rewrite(CodeMemberEvent source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeMemberEvent result = new CodeMemberEvent();
            result.Type = this.Rewrite(source.Type, ref didChildRewrite);
            result.PrivateImplementationType = this.Rewrite(source.PrivateImplementationType, ref didChildRewrite);
            this.Rewrite(result.ImplementationTypes, source.ImplementationTypes, ref didChildRewrite);
            result.Name = source.Name;
            result.Attributes = this.Rewrite(source.Attributes, ref didChildRewrite);
            this.Rewrite(result.CustomAttributes, source.CustomAttributes, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.Comments, source.Comments, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeMemberField
        /// </summary>
        /// <param name="source">A CodeMemberField to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeMemberField.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeMemberField Rewrite(CodeMemberField source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeMemberField result = new CodeMemberField();
            result.Type = this.Rewrite(source.Type, ref didChildRewrite);
            result.InitExpression = this.Rewrite(source.InitExpression, ref didChildRewrite);
            result.Name = source.Name;
            result.Attributes = this.Rewrite(source.Attributes, ref didChildRewrite);
            this.Rewrite(result.CustomAttributes, source.CustomAttributes, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.Comments, source.Comments, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeMemberMethod
        /// </summary>
        /// <param name="source">A CodeMemberMethod to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeMemberMethod.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeMemberMethod Rewrite(CodeMemberMethod source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            if (source is CodeConstructor)
            {
                return this.Rewrite((CodeConstructor)source, ref didRewrite);
            }

            if (source is CodeEntryPointMethod)
            {
                return this.Rewrite((CodeEntryPointMethod)source, ref didRewrite);
            }

            if (source is CodeTypeConstructor)
            {
                return this.Rewrite((CodeTypeConstructor)source, ref didRewrite);
            }

            bool didChildRewrite = false;
            CodeMemberMethod result = new CodeMemberMethod();
            result.ReturnType = this.Rewrite(source.ReturnType, ref didChildRewrite);
            this.Rewrite(result.Statements, source.Statements, ref didChildRewrite);
            this.Rewrite(result.Parameters, source.Parameters, ref didChildRewrite);
            result.PrivateImplementationType = this.Rewrite(source.PrivateImplementationType, ref didChildRewrite);
            this.Rewrite(result.ImplementationTypes, source.ImplementationTypes, ref didChildRewrite);
            this.Rewrite(result.ReturnTypeCustomAttributes, source.ReturnTypeCustomAttributes, ref didChildRewrite);
            this.Rewrite(result.TypeParameters, source.TypeParameters, ref didChildRewrite);
            result.Name = source.Name;
            result.Attributes = this.Rewrite(source.Attributes, ref didChildRewrite);
            this.Rewrite(result.CustomAttributes, source.CustomAttributes, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.Comments, source.Comments, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeMemberProperty
        /// </summary>
        /// <param name="source">A CodeMemberProperty to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeMemberProperty.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeMemberProperty Rewrite(CodeMemberProperty source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeMemberProperty result = new CodeMemberProperty();
            result.PrivateImplementationType = this.Rewrite(source.PrivateImplementationType, ref didChildRewrite);
            this.Rewrite(result.ImplementationTypes, source.ImplementationTypes, ref didChildRewrite);
            result.Type = this.Rewrite(source.Type, ref didChildRewrite);
            result.HasGet = source.HasGet;
            result.HasSet = source.HasSet;
            this.Rewrite(result.GetStatements, source.GetStatements, ref didChildRewrite);
            this.Rewrite(result.SetStatements, source.SetStatements, ref didChildRewrite);
            this.Rewrite(result.Parameters, source.Parameters, ref didChildRewrite);
            result.Name = source.Name;
            result.Attributes = this.Rewrite(source.Attributes, ref didChildRewrite);
            this.Rewrite(result.CustomAttributes, source.CustomAttributes, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.Comments, source.Comments, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeMethodInvokeExpression
        /// </summary>
        /// <param name="source">A CodeMethodInvokeExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeMethodInvokeExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeMethodInvokeExpression Rewrite(CodeMethodInvokeExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeMethodInvokeExpression result = new CodeMethodInvokeExpression();
            result.Method = this.Rewrite(source.Method, ref didChildRewrite);
            this.Rewrite(result.Parameters, source.Parameters, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeMethodReferenceExpression
        /// </summary>
        /// <param name="source">A CodeMethodReferenceExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeMethodReferenceExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeMethodReferenceExpression Rewrite(CodeMethodReferenceExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeMethodReferenceExpression result = new CodeMethodReferenceExpression();
            result.TargetObject = this.Rewrite(source.TargetObject, ref didChildRewrite);
            result.MethodName = source.MethodName;
            this.Rewrite(result.TypeArguments, source.TypeArguments, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeMethodReturnStatement
        /// </summary>
        /// <param name="source">A CodeMethodReturnStatement to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeMethodReturnStatement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeMethodReturnStatement Rewrite(CodeMethodReturnStatement source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeMethodReturnStatement result = new CodeMethodReturnStatement();
            result.Expression = this.Rewrite(source.Expression, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeNamespace
        /// </summary>
        /// <param name="source">A CodeNamespace to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeNamespace.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeNamespace Rewrite(CodeNamespace source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeNamespace result = new CodeNamespace();
            this.Rewrite(result.Types, source.Types, ref didChildRewrite);
            this.Rewrite(result.Imports, source.Imports, ref didChildRewrite);
            result.Name = source.Name;
            this.Rewrite(result.Comments, source.Comments, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeNamespaceCollection
        /// </summary>
        /// <param name="target">Target collection.</param>
        /// <param name="source">Source collection.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(CodeNamespaceCollection target, CodeNamespaceCollection source, ref bool didRewrite)
        {
            foreach (CodeNamespace item in source)
            {
                target.Add(this.Rewrite(item, ref didRewrite));
            }
        }

        /// <summary>
        /// Rewrites CodeNamespaceImport
        /// </summary>
        /// <param name="source">A CodeNamespaceImport to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeNamespaceImport.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeNamespaceImport Rewrite(CodeNamespaceImport source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeNamespaceImport result = new CodeNamespaceImport();
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            result.Namespace = source.Namespace;
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeNamespaceImportCollection
        /// </summary>
        /// <param name="target">Target collection.</param>
        /// <param name="source">Source collection.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(CodeNamespaceImportCollection target, CodeNamespaceImportCollection source, ref bool didRewrite)
        {
            foreach (CodeNamespaceImport item in source)
            {
                target.Add(this.Rewrite(item, ref didRewrite));
            }
        }

        /// <summary>
        /// Rewrites CodeObject
        /// </summary>
        /// <param name="source">A CodeObject to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeObject.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeObject Rewrite(CodeObject source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            if (source is CodeConstructor)
            {
                return this.Rewrite((CodeConstructor)source, ref didRewrite);
            }

            if (source is CodeEntryPointMethod)
            {
                return this.Rewrite((CodeEntryPointMethod)source, ref didRewrite);
            }

            if (source is CodeTypeConstructor)
            {
                return this.Rewrite((CodeTypeConstructor)source, ref didRewrite);
            }

            if (source is CodeTypeDelegate)
            {
                return this.Rewrite((CodeTypeDelegate)source, ref didRewrite);
            }

            if (source is CodeArgumentReferenceExpression)
            {
                return this.Rewrite((CodeArgumentReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeArrayCreateExpression)
            {
                return this.Rewrite((CodeArrayCreateExpression)source, ref didRewrite);
            }

            if (source is CodeArrayIndexerExpression)
            {
                return this.Rewrite((CodeArrayIndexerExpression)source, ref didRewrite);
            }

            if (source is CodeAssignStatement)
            {
                return this.Rewrite((CodeAssignStatement)source, ref didRewrite);
            }

            if (source is CodeAttachEventStatement)
            {
                return this.Rewrite((CodeAttachEventStatement)source, ref didRewrite);
            }

            if (source is CodeBaseReferenceExpression)
            {
                return this.Rewrite((CodeBaseReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeBinaryOperatorExpression)
            {
                return this.Rewrite((CodeBinaryOperatorExpression)source, ref didRewrite);
            }

            if (source is CodeCastExpression)
            {
                return this.Rewrite((CodeCastExpression)source, ref didRewrite);
            }

            if (source is CodeChecksumPragma)
            {
                return this.Rewrite((CodeChecksumPragma)source, ref didRewrite);
            }

            if (source is CodeCommentStatement)
            {
                return this.Rewrite((CodeCommentStatement)source, ref didRewrite);
            }

            if (source is CodeConditionStatement)
            {
                return this.Rewrite((CodeConditionStatement)source, ref didRewrite);
            }

            if (source is CodeDefaultValueExpression)
            {
                return this.Rewrite((CodeDefaultValueExpression)source, ref didRewrite);
            }

            if (source is CodeDelegateCreateExpression)
            {
                return this.Rewrite((CodeDelegateCreateExpression)source, ref didRewrite);
            }

            if (source is CodeDelegateInvokeExpression)
            {
                return this.Rewrite((CodeDelegateInvokeExpression)source, ref didRewrite);
            }

            if (source is CodeDirectionExpression)
            {
                return this.Rewrite((CodeDirectionExpression)source, ref didRewrite);
            }

            if (source is CodeEventReferenceExpression)
            {
                return this.Rewrite((CodeEventReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeExpressionStatement)
            {
                return this.Rewrite((CodeExpressionStatement)source, ref didRewrite);
            }

            if (source is CodeFieldReferenceExpression)
            {
                return this.Rewrite((CodeFieldReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeGotoStatement)
            {
                return this.Rewrite((CodeGotoStatement)source, ref didRewrite);
            }

            if (source is CodeIndexerExpression)
            {
                return this.Rewrite((CodeIndexerExpression)source, ref didRewrite);
            }

            if (source is CodeIterationStatement)
            {
                return this.Rewrite((CodeIterationStatement)source, ref didRewrite);
            }

            if (source is CodeLabeledStatement)
            {
                return this.Rewrite((CodeLabeledStatement)source, ref didRewrite);
            }

            if (source is CodeMemberEvent)
            {
                return this.Rewrite((CodeMemberEvent)source, ref didRewrite);
            }

            if (source is CodeMemberField)
            {
                return this.Rewrite((CodeMemberField)source, ref didRewrite);
            }

            if (source is CodeMemberMethod)
            {
                return this.Rewrite((CodeMemberMethod)source, ref didRewrite);
            }

            if (source is CodeMemberProperty)
            {
                return this.Rewrite((CodeMemberProperty)source, ref didRewrite);
            }

            if (source is CodeMethodInvokeExpression)
            {
                return this.Rewrite((CodeMethodInvokeExpression)source, ref didRewrite);
            }

            if (source is CodeMethodReferenceExpression)
            {
                return this.Rewrite((CodeMethodReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeMethodReturnStatement)
            {
                return this.Rewrite((CodeMethodReturnStatement)source, ref didRewrite);
            }

            if (source is CodeObjectCreateExpression)
            {
                return this.Rewrite((CodeObjectCreateExpression)source, ref didRewrite);
            }

            if (source is CodeParameterDeclarationExpression)
            {
                return this.Rewrite((CodeParameterDeclarationExpression)source, ref didRewrite);
            }

            if (source is CodePrimitiveExpression)
            {
                return this.Rewrite((CodePrimitiveExpression)source, ref didRewrite);
            }

            if (source is CodePropertyReferenceExpression)
            {
                return this.Rewrite((CodePropertyReferenceExpression)source, ref didRewrite);
            }

            if (source is CodePropertySetValueReferenceExpression)
            {
                return this.Rewrite((CodePropertySetValueReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeRegionDirective)
            {
                return this.Rewrite((CodeRegionDirective)source, ref didRewrite);
            }

            if (source is CodeRemoveEventStatement)
            {
                return this.Rewrite((CodeRemoveEventStatement)source, ref didRewrite);
            }

            if (source is CodeSnippetCompileUnit)
            {
                return this.Rewrite((CodeSnippetCompileUnit)source, ref didRewrite);
            }

            if (source is CodeSnippetExpression)
            {
                return this.Rewrite((CodeSnippetExpression)source, ref didRewrite);
            }

            if (source is CodeSnippetStatement)
            {
                return this.Rewrite((CodeSnippetStatement)source, ref didRewrite);
            }

            if (source is CodeSnippetTypeMember)
            {
                return this.Rewrite((CodeSnippetTypeMember)source, ref didRewrite);
            }

            if (source is CodeThisReferenceExpression)
            {
                return this.Rewrite((CodeThisReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeThrowExceptionStatement)
            {
                return this.Rewrite((CodeThrowExceptionStatement)source, ref didRewrite);
            }

            if (source is CodeTryCatchFinallyStatement)
            {
                return this.Rewrite((CodeTryCatchFinallyStatement)source, ref didRewrite);
            }

            if (source is CodeTypeDeclaration)
            {
                return this.Rewrite((CodeTypeDeclaration)source, ref didRewrite);
            }

            if (source is CodeTypeOfExpression)
            {
                return this.Rewrite((CodeTypeOfExpression)source, ref didRewrite);
            }

            if (source is CodeTypeReferenceExpression)
            {
                return this.Rewrite((CodeTypeReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeVariableDeclarationStatement)
            {
                return this.Rewrite((CodeVariableDeclarationStatement)source, ref didRewrite);
            }

            if (source is CodeVariableReferenceExpression)
            {
                return this.Rewrite((CodeVariableReferenceExpression)source, ref didRewrite);
            }

            if (source is CodeComment)
            {
                return this.Rewrite((CodeComment)source, ref didRewrite);
            }

            if (source is CodeCompileUnit)
            {
                return this.Rewrite((CodeCompileUnit)source, ref didRewrite);
            }

            if (source is CodeDirective)
            {
                return this.Rewrite((CodeDirective)source, ref didRewrite);
            }

            if (source is CodeExpression)
            {
                return this.Rewrite((CodeExpression)source, ref didRewrite);
            }

            if (source is CodeNamespace)
            {
                return this.Rewrite((CodeNamespace)source, ref didRewrite);
            }

            if (source is CodeNamespaceImport)
            {
                return this.Rewrite((CodeNamespaceImport)source, ref didRewrite);
            }

            if (source is CodeStatement)
            {
                return this.Rewrite((CodeStatement)source, ref didRewrite);
            }

            if (source is CodeTypeMember)
            {
                return this.Rewrite((CodeTypeMember)source, ref didRewrite);
            }

            if (source is CodeTypeParameter)
            {
                return this.Rewrite((CodeTypeParameter)source, ref didRewrite);
            }

            if (source is CodeTypeReference)
            {
                return this.Rewrite((CodeTypeReference)source, ref didRewrite);
            }

            bool didChildRewrite = false;
            CodeObject result = new CodeObject();
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeObjectCreateExpression
        /// </summary>
        /// <param name="source">A CodeObjectCreateExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeObjectCreateExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeObjectCreateExpression Rewrite(CodeObjectCreateExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeObjectCreateExpression result = new CodeObjectCreateExpression();
            result.CreateType = this.Rewrite(source.CreateType, ref didChildRewrite);
            this.Rewrite(result.Parameters, source.Parameters, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeParameterDeclarationExpression
        /// </summary>
        /// <param name="source">A CodeParameterDeclarationExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeParameterDeclarationExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeParameterDeclarationExpression Rewrite(CodeParameterDeclarationExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeParameterDeclarationExpression result = new CodeParameterDeclarationExpression();
            this.Rewrite(result.CustomAttributes, source.CustomAttributes, ref didChildRewrite);
            result.Direction = this.Rewrite(source.Direction, ref didChildRewrite);
            result.Type = this.Rewrite(source.Type, ref didChildRewrite);
            result.Name = source.Name;
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeParameterDeclarationExpressionCollection
        /// </summary>
        /// <param name="target">Target collection.</param>
        /// <param name="source">Source collection.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(CodeParameterDeclarationExpressionCollection target, CodeParameterDeclarationExpressionCollection source, ref bool didRewrite)
        {
            foreach (CodeParameterDeclarationExpression item in source)
            {
                target.Add(this.Rewrite(item, ref didRewrite));
            }
        }

        /// <summary>
        /// Rewrites CodePrimitiveExpression
        /// </summary>
        /// <param name="source">A CodePrimitiveExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodePrimitiveExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodePrimitiveExpression Rewrite(CodePrimitiveExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodePrimitiveExpression result = new CodePrimitiveExpression();
            result.Value = source.Value;
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodePropertyReferenceExpression
        /// </summary>
        /// <param name="source">A CodePropertyReferenceExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodePropertyReferenceExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodePropertyReferenceExpression Rewrite(CodePropertyReferenceExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodePropertyReferenceExpression result = new CodePropertyReferenceExpression();
            result.TargetObject = this.Rewrite(source.TargetObject, ref didChildRewrite);
            result.PropertyName = source.PropertyName;
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodePropertySetValueReferenceExpression
        /// </summary>
        /// <param name="source">A CodePropertySetValueReferenceExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodePropertySetValueReferenceExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodePropertySetValueReferenceExpression Rewrite(CodePropertySetValueReferenceExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodePropertySetValueReferenceExpression result = new CodePropertySetValueReferenceExpression();
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeRegionDirective
        /// </summary>
        /// <param name="source">A CodeRegionDirective to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeRegionDirective.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeRegionDirective Rewrite(CodeRegionDirective source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeRegionDirective result = new CodeRegionDirective();
            result.RegionText = source.RegionText;
            result.RegionMode = this.Rewrite(source.RegionMode, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeRegionMode
        /// </summary>
        /// <param name="source">A CodeRegionMode to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeRegionMode.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeRegionMode Rewrite(CodeRegionMode source, ref bool didRewrite)
        {
            bool didChildRewrite = false;
            CodeRegionMode result = new CodeRegionMode();
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeRemoveEventStatement
        /// </summary>
        /// <param name="source">A CodeRemoveEventStatement to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeRemoveEventStatement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeRemoveEventStatement Rewrite(CodeRemoveEventStatement source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeRemoveEventStatement result = new CodeRemoveEventStatement();
            result.Event = this.Rewrite(source.Event, ref didChildRewrite);
            result.Listener = this.Rewrite(source.Listener, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeSnippetCompileUnit
        /// </summary>
        /// <param name="source">A CodeSnippetCompileUnit to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeSnippetCompileUnit.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeSnippetCompileUnit Rewrite(CodeSnippetCompileUnit source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeSnippetCompileUnit result = new CodeSnippetCompileUnit();
            result.Value = source.Value;
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.Namespaces, source.Namespaces, ref didChildRewrite);
            this.Rewrite(result.ReferencedAssemblies, source.ReferencedAssemblies, ref didChildRewrite);
            this.Rewrite(result.AssemblyCustomAttributes, source.AssemblyCustomAttributes, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeSnippetExpression
        /// </summary>
        /// <param name="source">A CodeSnippetExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeSnippetExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeSnippetExpression Rewrite(CodeSnippetExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeSnippetExpression result = new CodeSnippetExpression();
            result.Value = source.Value;
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeSnippetStatement
        /// </summary>
        /// <param name="source">A CodeSnippetStatement to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeSnippetStatement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeSnippetStatement Rewrite(CodeSnippetStatement source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeSnippetStatement result = new CodeSnippetStatement();
            result.Value = source.Value;
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeSnippetTypeMember
        /// </summary>
        /// <param name="source">A CodeSnippetTypeMember to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeSnippetTypeMember.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeSnippetTypeMember Rewrite(CodeSnippetTypeMember source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeSnippetTypeMember result = new CodeSnippetTypeMember();
            result.Text = source.Text;
            result.Name = source.Name;
            result.Attributes = this.Rewrite(source.Attributes, ref didChildRewrite);
            this.Rewrite(result.CustomAttributes, source.CustomAttributes, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.Comments, source.Comments, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeStatement
        /// </summary>
        /// <param name="source">A CodeStatement to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeStatement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeStatement Rewrite(CodeStatement source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            if (source is CodeAssignStatement)
            {
                return this.Rewrite((CodeAssignStatement)source, ref didRewrite);
            }

            if (source is CodeAttachEventStatement)
            {
                return this.Rewrite((CodeAttachEventStatement)source, ref didRewrite);
            }

            if (source is CodeCommentStatement)
            {
                return this.Rewrite((CodeCommentStatement)source, ref didRewrite);
            }

            if (source is CodeConditionStatement)
            {
                return this.Rewrite((CodeConditionStatement)source, ref didRewrite);
            }

            if (source is CodeExpressionStatement)
            {
                return this.Rewrite((CodeExpressionStatement)source, ref didRewrite);
            }

            if (source is CodeGotoStatement)
            {
                return this.Rewrite((CodeGotoStatement)source, ref didRewrite);
            }

            if (source is CodeIterationStatement)
            {
                return this.Rewrite((CodeIterationStatement)source, ref didRewrite);
            }

            if (source is CodeLabeledStatement)
            {
                return this.Rewrite((CodeLabeledStatement)source, ref didRewrite);
            }

            if (source is CodeMethodReturnStatement)
            {
                return this.Rewrite((CodeMethodReturnStatement)source, ref didRewrite);
            }

            if (source is CodeRemoveEventStatement)
            {
                return this.Rewrite((CodeRemoveEventStatement)source, ref didRewrite);
            }

            if (source is CodeSnippetStatement)
            {
                return this.Rewrite((CodeSnippetStatement)source, ref didRewrite);
            }

            if (source is CodeThrowExceptionStatement)
            {
                return this.Rewrite((CodeThrowExceptionStatement)source, ref didRewrite);
            }

            if (source is CodeTryCatchFinallyStatement)
            {
                return this.Rewrite((CodeTryCatchFinallyStatement)source, ref didRewrite);
            }

            if (source is CodeVariableDeclarationStatement)
            {
                return this.Rewrite((CodeVariableDeclarationStatement)source, ref didRewrite);
            }

            bool didChildRewrite = false;
            CodeStatement result = new CodeStatement();
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeStatementCollection
        /// </summary>
        /// <param name="target">Target collection.</param>
        /// <param name="source">Source collection.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(CodeStatementCollection target, CodeStatementCollection source, ref bool didRewrite)
        {
            foreach (CodeStatement item in source)
            {
                target.Add(this.Rewrite(item, ref didRewrite));
            }
        }

        /// <summary>
        /// Rewrites CodeThisReferenceExpression
        /// </summary>
        /// <param name="source">A CodeThisReferenceExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeThisReferenceExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeThisReferenceExpression Rewrite(CodeThisReferenceExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeThisReferenceExpression result = new CodeThisReferenceExpression();
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeThrowExceptionStatement
        /// </summary>
        /// <param name="source">A CodeThrowExceptionStatement to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeThrowExceptionStatement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeThrowExceptionStatement Rewrite(CodeThrowExceptionStatement source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeThrowExceptionStatement result = new CodeThrowExceptionStatement();
            result.ToThrow = this.Rewrite(source.ToThrow, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeTryCatchFinallyStatement
        /// </summary>
        /// <param name="source">A CodeTryCatchFinallyStatement to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeTryCatchFinallyStatement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeTryCatchFinallyStatement Rewrite(CodeTryCatchFinallyStatement source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeTryCatchFinallyStatement result = new CodeTryCatchFinallyStatement();
            this.Rewrite(result.TryStatements, source.TryStatements, ref didChildRewrite);
            this.Rewrite(result.CatchClauses, source.CatchClauses, ref didChildRewrite);
            this.Rewrite(result.FinallyStatements, source.FinallyStatements, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeTypeConstructor
        /// </summary>
        /// <param name="source">A CodeTypeConstructor to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeTypeConstructor.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeTypeConstructor Rewrite(CodeTypeConstructor source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeTypeConstructor result = new CodeTypeConstructor();
            result.ReturnType = this.Rewrite(source.ReturnType, ref didChildRewrite);
            this.Rewrite(result.Statements, source.Statements, ref didChildRewrite);
            this.Rewrite(result.Parameters, source.Parameters, ref didChildRewrite);
            result.PrivateImplementationType = this.Rewrite(source.PrivateImplementationType, ref didChildRewrite);
            this.Rewrite(result.ImplementationTypes, source.ImplementationTypes, ref didChildRewrite);
            this.Rewrite(result.ReturnTypeCustomAttributes, source.ReturnTypeCustomAttributes, ref didChildRewrite);
            this.Rewrite(result.TypeParameters, source.TypeParameters, ref didChildRewrite);
            result.Name = source.Name;
            result.Attributes = this.Rewrite(source.Attributes, ref didChildRewrite);
            this.Rewrite(result.CustomAttributes, source.CustomAttributes, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.Comments, source.Comments, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeTypeDeclaration
        /// </summary>
        /// <param name="source">A CodeTypeDeclaration to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeTypeDeclaration.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeTypeDeclaration Rewrite(CodeTypeDeclaration source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            if (source is CodeTypeDelegate)
            {
                return this.Rewrite((CodeTypeDelegate)source, ref didRewrite);
            }

            bool didChildRewrite = false;
            CodeTypeDeclaration result = new CodeTypeDeclaration();
            result.TypeAttributes = source.TypeAttributes;
            this.Rewrite(result.BaseTypes, source.BaseTypes, ref didChildRewrite);
            result.IsClass = source.IsClass;
            result.IsStruct = source.IsStruct;
            result.IsEnum = source.IsEnum;
            result.IsInterface = source.IsInterface;
            result.IsPartial = source.IsPartial;
            this.Rewrite(result.Members, source.Members, ref didChildRewrite);
            this.Rewrite(result.TypeParameters, source.TypeParameters, ref didChildRewrite);
            result.Name = source.Name;
            result.Attributes = this.Rewrite(source.Attributes, ref didChildRewrite);
            this.Rewrite(result.CustomAttributes, source.CustomAttributes, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.Comments, source.Comments, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeTypeDeclarationCollection
        /// </summary>
        /// <param name="target">Target collection.</param>
        /// <param name="source">Source collection.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(CodeTypeDeclarationCollection target, CodeTypeDeclarationCollection source, ref bool didRewrite)
        {
            foreach (CodeTypeDeclaration item in source)
            {
                target.Add(this.Rewrite(item, ref didRewrite));
            }
        }

        /// <summary>
        /// Rewrites CodeTypeDelegate
        /// </summary>
        /// <param name="source">A CodeTypeDelegate to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeTypeDelegate.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeTypeDelegate Rewrite(CodeTypeDelegate source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeTypeDelegate result = new CodeTypeDelegate();
            result.ReturnType = this.Rewrite(source.ReturnType, ref didChildRewrite);
            this.Rewrite(result.Parameters, source.Parameters, ref didChildRewrite);
            result.TypeAttributes = source.TypeAttributes;
            this.Rewrite(result.BaseTypes, source.BaseTypes, ref didChildRewrite);
            result.IsClass = source.IsClass;
            result.IsStruct = source.IsStruct;
            result.IsEnum = source.IsEnum;
            result.IsInterface = source.IsInterface;
            result.IsPartial = source.IsPartial;
            this.Rewrite(result.Members, source.Members, ref didChildRewrite);
            this.Rewrite(result.TypeParameters, source.TypeParameters, ref didChildRewrite);
            result.Name = source.Name;
            result.Attributes = this.Rewrite(source.Attributes, ref didChildRewrite);
            this.Rewrite(result.CustomAttributes, source.CustomAttributes, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.Comments, source.Comments, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeTypeMember
        /// </summary>
        /// <param name="source">A CodeTypeMember to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeTypeMember.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeTypeMember Rewrite(CodeTypeMember source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            if (source is CodeConstructor)
            {
                return this.Rewrite((CodeConstructor)source, ref didRewrite);
            }

            if (source is CodeEntryPointMethod)
            {
                return this.Rewrite((CodeEntryPointMethod)source, ref didRewrite);
            }

            if (source is CodeTypeConstructor)
            {
                return this.Rewrite((CodeTypeConstructor)source, ref didRewrite);
            }

            if (source is CodeTypeDelegate)
            {
                return this.Rewrite((CodeTypeDelegate)source, ref didRewrite);
            }

            if (source is CodeMemberEvent)
            {
                return this.Rewrite((CodeMemberEvent)source, ref didRewrite);
            }

            if (source is CodeMemberField)
            {
                return this.Rewrite((CodeMemberField)source, ref didRewrite);
            }

            if (source is CodeMemberMethod)
            {
                return this.Rewrite((CodeMemberMethod)source, ref didRewrite);
            }

            if (source is CodeMemberProperty)
            {
                return this.Rewrite((CodeMemberProperty)source, ref didRewrite);
            }

            if (source is CodeSnippetTypeMember)
            {
                return this.Rewrite((CodeSnippetTypeMember)source, ref didRewrite);
            }

            if (source is CodeTypeDeclaration)
            {
                return this.Rewrite((CodeTypeDeclaration)source, ref didRewrite);
            }

            bool didChildRewrite = false;
            CodeTypeMember result = new CodeTypeMember();
            result.Name = source.Name;
            result.Attributes = this.Rewrite(source.Attributes, ref didChildRewrite);
            this.Rewrite(result.CustomAttributes, source.CustomAttributes, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.Comments, source.Comments, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeTypeMemberCollection
        /// </summary>
        /// <param name="target">Target collection.</param>
        /// <param name="source">Source collection.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(CodeTypeMemberCollection target, CodeTypeMemberCollection source, ref bool didRewrite)
        {
            foreach (CodeTypeMember item in source)
            {
                target.Add(this.Rewrite(item, ref didRewrite));
            }
        }

        /// <summary>
        /// Rewrites CodeTypeOfExpression
        /// </summary>
        /// <param name="source">A CodeTypeOfExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeTypeOfExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeTypeOfExpression Rewrite(CodeTypeOfExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeTypeOfExpression result = new CodeTypeOfExpression();
            result.Type = this.Rewrite(source.Type, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeTypeParameter
        /// </summary>
        /// <param name="source">A CodeTypeParameter to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeTypeParameter.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeTypeParameter Rewrite(CodeTypeParameter source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeTypeParameter result = new CodeTypeParameter();
            result.Name = source.Name;
            this.Rewrite(result.Constraints, source.Constraints, ref didChildRewrite);
            this.Rewrite(result.CustomAttributes, source.CustomAttributes, ref didChildRewrite);
            result.HasConstructorConstraint = source.HasConstructorConstraint;
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeTypeParameterCollection
        /// </summary>
        /// <param name="target">Target collection.</param>
        /// <param name="source">Source collection.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(CodeTypeParameterCollection target, CodeTypeParameterCollection source, ref bool didRewrite)
        {
            foreach (CodeTypeParameter item in source)
            {
                target.Add(this.Rewrite(item, ref didRewrite));
            }
        }

        /// <summary>
        /// Rewrites CodeTypeReferenceOptions
        /// </summary>
        /// <param name="source">A CodeTypeReferenceOptions to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeTypeReferenceOptions.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeTypeReferenceOptions Rewrite(CodeTypeReferenceOptions source, ref bool didRewrite)
        {
            bool didChildRewrite = false;
            CodeTypeReferenceOptions result = new CodeTypeReferenceOptions();
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeTypeReference
        /// </summary>
        /// <param name="source">A CodeTypeReference to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeTypeReference.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeTypeReference Rewrite(CodeTypeReference source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeTypeReference result = new CodeTypeReference();
            result.ArrayElementType = this.Rewrite(source.ArrayElementType, ref didChildRewrite);
            result.ArrayRank = source.ArrayRank;
            result.BaseType = source.BaseType;
            result.Options = this.Rewrite(source.Options, ref didChildRewrite);
            this.Rewrite(result.TypeArguments, source.TypeArguments, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeTypeReferenceCollection
        /// </summary>
        /// <param name="target">Target collection.</param>
        /// <param name="source">Source collection.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(CodeTypeReferenceCollection target, CodeTypeReferenceCollection source, ref bool didRewrite)
        {
            foreach (CodeTypeReference item in source)
            {
                target.Add(this.Rewrite(item, ref didRewrite));
            }
        }

        /// <summary>
        /// Rewrites CodeTypeReferenceExpression
        /// </summary>
        /// <param name="source">A CodeTypeReferenceExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeTypeReferenceExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeTypeReferenceExpression Rewrite(CodeTypeReferenceExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeTypeReferenceExpression result = new CodeTypeReferenceExpression();
            result.Type = this.Rewrite(source.Type, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeVariableDeclarationStatement
        /// </summary>
        /// <param name="source">A CodeVariableDeclarationStatement to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeVariableDeclarationStatement.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeVariableDeclarationStatement Rewrite(CodeVariableDeclarationStatement source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeVariableDeclarationStatement result = new CodeVariableDeclarationStatement();
            result.InitExpression = this.Rewrite(source.InitExpression, ref didChildRewrite);
            result.Name = source.Name;
            result.Type = this.Rewrite(source.Type, ref didChildRewrite);
            result.LinePragma = this.Rewrite(source.LinePragma, ref didChildRewrite);
            this.Rewrite(result.StartDirectives, source.StartDirectives, ref didChildRewrite);
            this.Rewrite(result.EndDirectives, source.EndDirectives, ref didChildRewrite);
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeVariableReferenceExpression
        /// </summary>
        /// <param name="source">A CodeVariableReferenceExpression to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeVariableReferenceExpression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual CodeVariableReferenceExpression Rewrite(CodeVariableReferenceExpression source, ref bool didRewrite)
        {
            if (source == null)
            {
                return source;
            }

            bool didChildRewrite = false;
            CodeVariableReferenceExpression result = new CodeVariableReferenceExpression();
            result.VariableName = source.VariableName;
            this.Rewrite(result.UserData, source.UserData, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites FieldDirection
        /// </summary>
        /// <param name="source">A FieldDirection to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten FieldDirection.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual FieldDirection Rewrite(FieldDirection source, ref bool didRewrite)
        {
            bool didChildRewrite = false;
            FieldDirection result = new FieldDirection();
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites MemberAttributes
        /// </summary>
        /// <param name="source">A MemberAttributes to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten MemberAttributes.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Code is generated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursive code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Makes the code more readable, avoids too many variables.")]
        protected virtual MemberAttributes Rewrite(MemberAttributes source, ref bool didRewrite)
        {
            bool didChildRewrite = false;
            MemberAttributes result = new MemberAttributes();
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Rewrites CodeAttributeDeclaration.
        /// </summary>
        /// <param name="source">A CodeAttributeDeclaration to be rewritten.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        /// <returns>Rewritten CodeAttributeDeclaration.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Need this to simplify recursion.")]
        protected virtual CodeAttributeDeclaration Rewrite(CodeAttributeDeclaration source, ref bool didRewrite)
        {
            bool didChildRewrite = false;
            CodeAttributeDeclaration result = new CodeAttributeDeclaration(this.Rewrite(source.AttributeType, ref didChildRewrite));
            result.Name = source.Name;
            this.Rewrite(result.Arguments, source.Arguments, ref didChildRewrite);
            if (didChildRewrite)
            {
                didRewrite = true;
                return result;
            }
            else
            {
                return source;
            }
        }
        
        /// <summary>
        /// Copies contents of one list into another.
        /// </summary>
        /// <param name="target">Target collection.</param>
        /// <param name="source">Source collection.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(IList target, IList source, ref bool didRewrite)
        {
            didRewrite = true;
            foreach (var str in source)
            {
                target.Add(str);
            }
        }

        /// <summary>
        /// Copies contents of one dictionary into another.
        /// </summary>
        /// <param name="target">Target dictionary.</param>
        /// <param name="source">Source dictionary.</param>
        /// <param name="didRewrite">A value which will be set to true if the rewriting returned a new object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Need this to simplify recursion.")]
        protected void Rewrite(IDictionary target, IDictionary source, ref bool didRewrite)
        {
            didRewrite = true;
            foreach (var k in source.Keys)
            {
                target.Add(k, source[k]);
            }
        }
    }
}