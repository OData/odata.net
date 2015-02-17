//---------------------------------------------------------------------
// <copyright file="ClientQueryCodeGenHelperMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;

    /// <summary>
    /// Methods to help generate code for client expression
    /// </summary>
    internal class ClientQueryCodeGenHelperMethods
    {
        internal static CodeExpression BuildClientExpectedErrorExpression(ExpectedClientErrorBaseline clientExpectedErrorExpression)
        {
            if (clientExpectedErrorExpression == null)
            {
                return Code.Null();
            }

            CodeExpression expectedExceptionMessageExpression = BuildExpectedErrorMessage(clientExpectedErrorExpression.ExpectedExceptionMessage);

            return Code.New(
                Code.TypeRef(typeof(ExpectedClientErrorBaseline)),
                Code.TypeOf(Code.TypeRef(clientExpectedErrorExpression.ExpectedExceptionType)),
                Code.Primitive(clientExpectedErrorExpression.HasServerSpecificExpectedMessage),
                expectedExceptionMessageExpression);
        }

        internal static CodeExpression BuildExpectedErrorMessage(ExpectedErrorMessage expectedErrorMessage)
        {
            List<CodeExpression> codeExpressions = new List<CodeExpression>();
            codeExpressions.Add(Code.Primitive(expectedErrorMessage.ResourceIdentifier));
            codeExpressions.AddRange(expectedErrorMessage.Arguments.ToArray().Select(r => Code.Primitive(r)));
            CodeExpression codeExpression = Code.New(Code.TypeRef(typeof(ExpectedErrorMessage)), codeExpressions.ToArray());

            if (expectedErrorMessage.InnerError != null)
            {
                codeExpression = BuildExpectedErrorMessage(expectedErrorMessage.InnerError).Call("AddParent", codeExpression);
            }

            return codeExpression;
        }
    }
}