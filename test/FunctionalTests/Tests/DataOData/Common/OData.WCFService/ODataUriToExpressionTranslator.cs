//---------------------------------------------------------------------
// <copyright file="ODataUriToExpressionTranslator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    #region Namespaces
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.OData.Contracts;
    #endregion Namespaces

    internal class ODataUriToExpressionTranslator : PathSegmentTranslator<Expression>
    {
        private readonly IEdmModel model;
        private readonly IODataQueryProvider queryProvider;
        
        private const string WhereMethodName = "Where";
        private const string SingleMethodName = "Single";
        private static readonly Expression NullLiteralExpression = Expression.Constant(null);
        private static readonly Expression TrueLiteralExpression = Expression.Constant(true, typeof(bool));
        private static readonly ConstantExpression FalseLiteralExpression = Expression.Constant(false);

        /// <summary>
        /// The resulting expression from this translator.
        /// </summary>
        public Expression ResultExpression { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="queryProvider">The query provider</param>
        /// <param name="model">The model</param>
        public ODataUriToExpressionTranslator(IODataQueryProvider queryProvider, IEdmModel model)
        {
            this.queryProvider = queryProvider;
            this.model = model;
            this.ResultExpression = NullLiteralExpression;
        }

        /// <summary>
        /// Translate TypeSegment to linq expression.
        /// </summary>
        /// <param name="segment">The TypeSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(TypeSegment segment)
        {
            throw new NotImplementedException("TypeSegment translation is not supported");
        }

        /// <summary>
        /// Translate NavigationPropertySegment to linq expression.
        /// </summary>
        /// <param name="segment">The NavigationPropertySegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(NavigationPropertySegment segment)
        {
            throw new NotImplementedException("NavigationPropertySegment translation is not supported");
        }

        /// <summary>
        /// Translate EntitySetSegment to linq expression.
        /// </summary>
        /// <param name="segment">The EntitySetSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(EntitySetSegment segment)
        {
            IQueryable rootSetQueryable = queryProvider.GetQueryRootForEntitySet(segment.EntitySet);
            this.ResultExpression = rootSetQueryable.Expression;
            return this.ResultExpression;
        }

        /// <summary>
        /// Translate KeySegment to linq expression.
        /// </summary>
        /// <param name="segment">The KeySegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(KeySegment segment)
        {
            // translate to be ResultExpressionFromEntitySetSegment.Where(id -> id == keyVale).Single()

            Type instanceType = null;
            if (segment.EdmType.TypeKind == EdmTypeKind.Primitive)
            {
                instanceType = EdmLibraryExtensions.GetPrimitiveClrType((IEdmPrimitiveTypeReference)segment.EdmType.ToTypeReference(false));
            }

            ParameterExpression parameter = Expression.Parameter(instanceType, "it");
            Expression body = null;
            foreach (var key in segment.Keys)
            {
                var propertyAccessExpression = Expression.Property(parameter, instanceType, key.Key);
                Expression keyPredicate = Expression.Equal(propertyAccessExpression, Expression.Constant(key.Value));
                if (body == null)
                {
                    body = keyPredicate;
                }
                else
                {
                    body = Expression.AndAlso(body, keyPredicate);
                }
            }

            Expression whereResult = Expression.Call(typeof(Queryable), WhereMethodName, new Type[] { instanceType }, this.ResultExpression, Expression.Quote(Expression.Lambda(body, parameter)));
            this.ResultExpression = Expression.Call(typeof(Queryable), SingleMethodName, new Type[] { instanceType }, whereResult);
            return this.ResultExpression;
        }

        /// <summary>
        /// Translate PropertySegment to linq expression.
        /// </summary>
        /// <param name="segment">The PropertySegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(PropertySegment segment)
        {
            // translate to be ResultExpressionFromKeySegment.PropertyName
            this.ResultExpression = Expression.Property(this.ResultExpression, segment.Property.Name);     
            return this.ResultExpression;
        }

        /// <summary>
        /// Translate OperationSegment to linq expression.
        /// </summary>
        /// <param name="segment">The OperationSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(OperationSegment segment)
        {
            throw new NotImplementedException("OperationSegment translation is not supported");
        }

        /// <summary>
        /// Translate OpenPropertySegment to linq expression.
        /// </summary>
        /// <param name="segment">The OpenPropertySegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(DynamicPathSegment segment)
        {
            throw new NotImplementedException("OpenPropertySegment translation is not supported");
        }

        /// <summary>
        /// Translate CountSegment to linq expression.
        /// </summary>
        /// <param name="segment">The CountSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(CountSegment segment)
        {
            throw new NotImplementedException("CountSegment translation is not supported");
        }

        /// <summary>
        /// Translate NavigationPropertyLinkSegment to linq expression.
        /// </summary>
        /// <param name="segment">The NavigationPropertyLinkSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(NavigationPropertyLinkSegment segment)
        {
            throw new NotImplementedException("NavigationPropertyLinkSegment translation is not supported");
        }

        /// <summary>
        /// Translate BatchSegment to linq expression.
        /// </summary>
        /// <param name="segment">The BatchSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(BatchSegment segment)
        {
            throw new NotImplementedException("BatchSegment translation is not supported");
        }

        /// <summary>
        /// Translate BatchReferenceSegment to linq expression.
        /// </summary>
        /// <param name="segment">The BatchReferenceSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(BatchReferenceSegment segment)
        {
            throw new NotImplementedException("BatchReferenceSegment translation is not supported");
        }

        /// <summary>
        /// Translate ValueSegment to linq expression.
        /// </summary>
        /// <param name="segment">The ValueSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(ValueSegment segment)
        {
            throw new NotImplementedException("ValueSegment translation is not supported");
        }

        /// <summary>
        /// Translate MetadataSegment to linq expression.
        /// </summary>
        /// <param name="segment">The MetadataSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(MetadataSegment segment)
        {
            throw new NotImplementedException("MetadataSegment translation is not supported");
        }
    }
}