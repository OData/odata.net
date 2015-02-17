//---------------------------------------------------------------------
// <copyright file="ClientQueryExpressionCodeExpressionConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System.CodeDom;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Linq;

    /// <summary>
    /// Contract for generating non Astoria specific code expressions
    /// </summary>
    [ImplementationName(typeof(IClientQueryExpressionCodeExpressionConverter), "Default")]
    public class ClientQueryExpressionCodeExpressionConverter : LinqMethodSyntaxCodeGenerationVisitor, IClientQueryExpressionCodeExpressionConverter
    {
        /// <summary>
        /// Initializes a new instance of the ClientQueryExpressionCodeExpressionConverter class.
        /// </summary>
        public ClientQueryExpressionCodeExpressionConverter() 
            : base(null, null)
        {
        }

        /// <summary>
        /// Generates code for a QueryExpression
        /// NOTE:do not use for expression that contain Astoria specific expressions
        /// </summary>
        /// <param name="queryExpression">Query Expression</param>
        /// <returns>Code expression</returns>
        public CodeExpression Convert(QueryExpression queryExpression)
        {
            return this.GenerateCode(queryExpression);
        }

        /// <summary>
        /// Resolves Property for the Property Initializer
        /// </summary>
        /// <param name="newExpression">Object being created</param>
        /// <param name="memberName">Name of member being resolved</param>
        /// <param name="memberQueryExpression">Member Query expression</param>
        /// <param name="member">Already generated member code expression</param>
        /// <returns>Code Expression</returns>
        protected override CodeExpression FixupPropertyForPropertyInitializer(QueryExpression newExpression, string memberName, QueryExpression memberQueryExpression, CodeExpression member)
        {
            var queryComplexType = newExpression.ExpressionType as QueryComplexType;
            var queryCollectionType = memberQueryExpression.ExpressionType as QueryCollectionType;

            if (queryCollectionType != null)
            {
                ExceptionUtilities.CheckObjectNotNull(queryComplexType, "Expected a valid queryComplexType");
                var propertyInfo = queryComplexType.ClrType.GetProperty(memberName);

                ExceptionUtilities.CheckObjectNotNull(propertyInfo, "Cannot find property '{0}' on ClrType '{1}'", memberName, queryComplexType.ClrType.Name);

                return Code.New(Code.TypeRef(propertyInfo.PropertyType), member);
            }

            return member;
        }
    }
}
