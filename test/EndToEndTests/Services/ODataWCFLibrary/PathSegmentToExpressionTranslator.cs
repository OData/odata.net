//---------------------------------------------------------------------
// <copyright file="PathSegmentToExpressionTranslator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.Services.ODataWCFService.DataSource;
using Microsoft.Test.OData.Services.ODataWCFService.UriHandlers;

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    internal class PathSegmentToExpressionTranslator : PathSegmentTranslator<Expression>
    {
        private readonly IEdmModel model;
        private readonly IODataDataSource dataSource;
        private readonly QueryContext queryContext;

        private const string WhereMethodName = "Where";
        private const string CountMethodName = "Count";
        private const string AsQueryableMethod = "AsQueryable";
        private const string CastMethodName = "Cast";
        private static readonly Expression NullLiteralExpression = Expression.Constant(null);

        public Expression[] ActionInvokeParameters { get; set; }

        /// <summary>
        /// The resulting expression from this translator.
        /// </summary>
        public Expression ResultExpression { get; private set; }

        /// <summary>
        /// The last processed ODataPathSegment.
        /// </summary>
        private ODataPathSegment LastProcessedSegment { get; set; }

        private QueryTarget LastQueryTarget
        {
            get
            {
                // TODO: cache
                return QueryTarget.Resolve(new ODataPath(this.LastProcessedSegment));
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="queryProvider">The query provider</param>
        /// <param name="model">The model</param>
        public PathSegmentToExpressionTranslator(IODataDataSource dataSource, QueryContext queryContext, IEdmModel model)
        {
            this.dataSource = dataSource;
            this.queryContext = queryContext;
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
            if (this.LastQueryTarget.IsEntitySet)
            {
                Type baseType = EdmClrTypeUtils.GetInstanceType(this.LastQueryTarget.ElementType.FullTypeName());
                Type instanceType = EdmClrTypeUtils.GetInstanceType(((IEdmCollectionType)segment.EdmType).ElementType);

                // A set return now is a IEnumable, try to convert to IQueryable.
                this.ResultExpression = Expression.Call(typeof(Queryable), AsQueryableMethod, new Type[] { baseType }, this.ResultExpression);

                // Convert to .Where(it=> it is Type)
                ParameterExpression parameter = Expression.Parameter(baseType, "it");
                Expression body = Expression.TypeIs(parameter, instanceType);
                this.ResultExpression = Expression.Call(typeof(Queryable), WhereMethodName, new Type[] { baseType }, this.ResultExpression, Expression.Quote(Expression.Lambda(body, parameter)));
                this.ResultExpression = Expression.Call(typeof(Queryable), CastMethodName, new[] { instanceType }, this.ResultExpression);
            }
            else if (this.LastQueryTarget.TypeKind == EdmTypeKind.Entity)
            {
                Type instanceType = EdmClrTypeUtils.GetInstanceType(segment.EdmType.FullTypeName());
                this.ResultExpression = Expression.Convert(this.ResultExpression, instanceType);
            }
            else if (this.LastQueryTarget.Property != null)
            {
                if (this.LastQueryTarget.TypeKind == EdmTypeKind.Complex)
                {
                    Type instanceType = EdmClrTypeUtils.GetInstanceType(segment.EdmType.FullTypeName());
                    this.ResultExpression = Expression.Convert(this.ResultExpression, instanceType);
                }
                else
                {
                    // This code should not be hit. It should be prevented by UriParser.
                    throw new ApplicationException(
                        string.Format("PropertySegment with TypeKind '{0}' following by TypeSegment is invalid", this.LastQueryTarget.TypeKind));
                }
            }
            else
            {
                throw Utility.BuildException(HttpStatusCode.NotImplemented, "Unsupported URI segment before TypeSegment", null);
            }

            this.LastProcessedSegment = segment;
            return this.ResultExpression;
        }

        /// <summary>
        /// Translate NavigationPropertySegment to linq expression.
        /// </summary>
        /// <param name="segment">The NavigationPropertySegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(NavigationPropertySegment segment)
        {
            if (!(this.LastProcessedSegment is KeySegment
                || this.LastProcessedSegment is SingletonSegment
                || this.LastProcessedSegment is TypeSegment
                || this.LastProcessedSegment is NavigationPropertySegment))
            {
                throw new InvalidOperationException("Unsupported URI segment before NavigationPropertySegment");
            }

            IEdmEntityType lastSegmentEntityType = this.LastProcessedSegment.EdmType as IEdmEntityType;
            IEdmNavigationProperty navigationProperty = segment.NavigationProperty;

            // <context>/PropertyName will be translated to <context>.PropertyName
            Type sourceInstanceType = EdmClrTypeUtils.GetInstanceType(lastSegmentEntityType.FullTypeName());

            this.ResultExpression = Expression.Property(this.ResultExpression, sourceInstanceType, navigationProperty.Name);
            this.LastProcessedSegment = segment;

            return this.ResultExpression;
        }

        /// <summary>
        /// Translate EntitySetSegment to linq expression.
        /// </summary>
        /// <param name="segment">The EntitySetSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(EntitySetSegment segment)
        {
            if (this.LastProcessedSegment != null)
            {
                throw new InvalidOperationException("Unsupported URI segment before EntitySetSegment");
            }

            var rootSet = Utility.GetRootQuery(this.dataSource, segment.EntitySet);
            this.ResultExpression = Expression.Constant(rootSet);
            this.LastProcessedSegment = segment;
            return this.ResultExpression;
        }

        /// <summary>
        /// Translate SingletonSegment to linq expression.
        /// </summary>
        /// <param name="segment">The SingletonSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(SingletonSegment segment)
        {
            if (this.LastProcessedSegment != null)
            {
                throw new InvalidOperationException("Unsupported URI segment before SingletonSegment");
            }

            var singleton = Utility.GetRootQuery(this.dataSource, segment.Singleton);
            this.ResultExpression = Expression.Constant(singleton);
            this.LastProcessedSegment = segment;
            return this.ResultExpression;
        }

        /// <summary>
        /// Translate KeySegment to linq expression.
        /// </summary>
        /// <param name="segment">The KeySegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(KeySegment segment)
        {
            if (!(this.LastProcessedSegment is EntitySetSegment || this.LastProcessedSegment is NavigationPropertySegment || this.LastProcessedSegment is NavigationPropertyLinkSegment))
            {
                throw new InvalidOperationException("Unsupported URI segment before KeySegment");
            }

            // translate to be ResultExpressionFromEntitySetSegment.Where(id -> id == keyVale).Single()
            IEdmEntityType entityType = segment.EdmType as IEdmEntityType;
            Type instanceType = EdmClrTypeUtils.GetInstanceType(entityType.FullTypeName());

            // Convert to .Where(it=> it is Type)
            ParameterExpression parameter = Expression.Parameter(instanceType, "it");
            Expression body = null;
            foreach (var key in segment.Keys)
            {
                Expression keyPredicate = null;
                var propertyAccessExpression = Expression.Property(parameter, instanceType, key.Key);

                var keyType = entityType.Key().Single(k => k.Name == key.Key).Type;
                if (keyType.IsTypeDefinition())
                {
                    var typeDefinition = keyType.AsTypeDefinition();
                    var underlyingType = EdmClrTypeUtils.GetInstanceType(typeDefinition.TypeDefinition().UnderlyingType.FullTypeName());
                    keyPredicate = Expression.Equal(Expression.Convert(propertyAccessExpression, underlyingType), Expression.Constant(key.Value));
                }
                else
                {
                    keyPredicate = Expression.Equal(propertyAccessExpression, Expression.Constant(key.Value));
                }

                if (body == null)
                {
                    body = keyPredicate;
                }
                else
                {
                    body = Expression.AndAlso(body, keyPredicate);
                }
            }

            this.ResultExpression = Expression.Call(
                this.ResultExpression,
                typeof(EntityCollection<>).MakeGenericType(instanceType).GetMethod("GetEntity"),
                Expression.Lambda(body, parameter));

            this.LastProcessedSegment = segment;
            return this.ResultExpression;
        }

        /// <summary>
        /// Translate PropertySegment to linq expression.
        /// </summary>
        /// <param name="segment">The PropertySegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(PropertySegment segment)
        {
            if (!(this.LastProcessedSegment is KeySegment
                || this.LastProcessedSegment is SingletonSegment
                || this.LastProcessedSegment is NavigationPropertySegment
                || this.LastProcessedSegment is PropertySegment   // For addressing primitive property under complex property
                || this.LastProcessedSegment is TypeSegment       // For derived type cast
                || this.LastProcessedSegment is OperationSegment))// Context Uri Contains Bugs When Query Property Segment after Opertaion Segment
            {
                throw new InvalidOperationException("Unsupported URI segment before PropertySegment");
            }

            // translate to be ResultExpressionFromKeySegment.PropertyName
            this.ResultExpression = Expression.Property(this.ResultExpression, segment.Property.Name);
            this.LastProcessedSegment = segment;
            return this.ResultExpression;
        }

        /// <summary>
        /// Translate OperationImportSegment to linq expression.
        /// </summary>
        /// <param name="segment">The OperationImportSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(OperationImportSegment segment)
        {
            IEdmOperationImport operationImport = segment.OperationImports.First();
            IEdmOperation operation = operationImport.Operation;

            List<Expression> arguments = new List<Expression>();
            if (operation.IsBound)
            {
                arguments.Add(this.ResultExpression);
            }
            if (operation is IEdmFunction)
            {
                NodeToExpressionTranslator nodeToExpressionTranslator = new NodeToExpressionTranslator()
                {
                    UriParser = this.queryContext.UriParser,
                    DataSource = this.dataSource,
                };
                arguments.AddRange(segment.Parameters.Select(p => nodeToExpressionTranslator.TranslateNode((QueryNode)p.Value)));
                var operationProvider = this.dataSource.OperationProvider;
                operationProvider.QueryContext = this.queryContext;
                this.ResultExpression = Expression.Call(Expression.Constant(operationProvider), operation.Name, new Type[] { }, arguments.ToArray());
            }
            else if (operation is IEdmAction)
            {
                arguments.AddRange(this.ActionInvokeParameters);
                var operationProvider = this.dataSource.OperationProvider;
                operationProvider.QueryContext = this.queryContext;
                this.ResultExpression = Expression.Call(Expression.Constant(operationProvider), operation.Name, new Type[] { }, arguments.ToArray());
            }
            else
            {
                throw new NotImplementedException("Unsupported operation type.");
            }

            this.LastProcessedSegment = segment;
            return this.ResultExpression;
        }

        /// <summary>
        /// Translate OperationSegment to linq expression.
        /// </summary>
        /// <param name="segment">The OperationSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(OperationSegment segment)
        {
            IEdmOperation operation = segment.Operations.First();

            List<Expression> arguments = new List<Expression>();
            if (operation.IsBound)
            {
                arguments.Add(this.ResultExpression);
            }
            if (operation is IEdmFunction)
            {
                NodeToExpressionTranslator nodeToExpressionTranslator = new NodeToExpressionTranslator()
                {
                    UriParser = this.queryContext.UriParser,
                    DataSource = this.dataSource,
                };

                arguments.AddRange(segment.Parameters.Select(p => nodeToExpressionTranslator.TranslateNode((QueryNode)p.Value)));
                var operationProvider = this.dataSource.OperationProvider;
                operationProvider.QueryContext = this.queryContext;
                this.ResultExpression = Expression.Call(Expression.Constant(operationProvider), operation.Name, new Type[] { }, arguments.ToArray());
            }
            else if (operation is IEdmAction)
            {
                arguments.AddRange(this.ActionInvokeParameters);
                var operationProvider = this.dataSource.OperationProvider;
                operationProvider.QueryContext = this.queryContext;
                this.ResultExpression = Expression.Call(Expression.Constant(operationProvider), operation.Name, new Type[] { }, arguments.ToArray());
            }
            else
            {
                throw new NotImplementedException("Unsupported operation type.");
            }

            this.LastProcessedSegment = segment;
            return this.ResultExpression;
        }

        /// <summary>
        /// Translate OpenPropertySegment to linq expression.
        /// </summary>
        /// <param name="segment">The OpenPropertySegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(DynamicPathSegment segment)
        {
            if (!(this.LastProcessedSegment is KeySegment
               || this.LastProcessedSegment is SingletonSegment
               || this.LastProcessedSegment is NavigationPropertySegment
               || this.LastProcessedSegment is PropertySegment
               || this.LastProcessedSegment is TypeSegment))
            {
                throw new InvalidOperationException("Unsupported URI segment before PropertySegment");
            }

            // get OpenProperties
            var propertyAccessExpression = Expression.Property(this.ResultExpression, "OpenProperties");
            // key
            var key = Expression.Constant(segment.Identifier, typeof(string));
            // OpenProperties.ContainsKey(segment.PropertyName)
            MethodInfo containsKeyMethod = typeof(Dictionary<string, object>).GetMethod("ContainsKey", new[] { typeof(string) });
            var containsExpression = Expression.Call(propertyAccessExpression, containsKeyMethod, key);
            //OpenProperties[segment.PropertyName]
            var queryOpenPropertyExpression = Expression.Property(propertyAccessExpression, "Item", key);

            this.ResultExpression = Expression.Condition(containsExpression, queryOpenPropertyExpression, Expression.Constant(null));
            this.LastProcessedSegment = segment;
            return this.ResultExpression;
        }

        /// <summary>
        /// Translate CountSegment to linq expression.
        /// </summary>
        /// <param name="segment">The CountSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(CountSegment segment)
        {
            if (!(this.LastProcessedSegment is EntitySetSegment || this.LastProcessedSegment is NavigationPropertySegment))
            {
                throw new InvalidOperationException("Unsupported URI segment before CountSegment");
            }

            Type instanceType = EdmClrTypeUtils.GetInstanceType((this.LastProcessedSegment.EdmType as EdmCollectionType).ElementType);

            // A set return now is a IEnumable, try to convert to IQueryable.
            this.ResultExpression = Expression.Call(typeof(Queryable), AsQueryableMethod, new Type[] { instanceType }, this.ResultExpression);

            this.ResultExpression = Expression.Call(typeof(Queryable), CountMethodName, new Type[] { instanceType }, this.ResultExpression);
            this.LastProcessedSegment = segment;
            return this.ResultExpression;
        }

        /// <summary>
        /// Translate NavigationPropertyLinkSegment to linq expression.
        /// </summary>
        /// <param name="segment">The NavigationPropertyLinkSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(NavigationPropertyLinkSegment segment)
        {
            if (!(this.LastProcessedSegment is KeySegment
                || this.LastProcessedSegment is SingletonSegment
                || this.LastProcessedSegment is TypeSegment))
            {
                throw new InvalidOperationException("Unsupported URI segment before NavigationPropertyLinkSegment");
            }

            var lastSegmentEntityType = this.LastProcessedSegment.EdmType as EdmEntityType;

            // <context>/PropertyName will be translated to <context>.PropertyName
            this.ResultExpression = Expression.Property(
                this.ResultExpression,
                EdmClrTypeUtils.GetInstanceType(lastSegmentEntityType.FullTypeName()),
                segment.NavigationProperty.Name);

            this.LastProcessedSegment = segment;
            return this.ResultExpression;
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
            // The last segment could be one of the following types
            //  1. PropertySegment type: query a property
            //  2. KeySegment type: query a stream. E.g. Photo(1)/$value
            //  3. NavigationPropertySegment: People('someone')/Photo/$value
            if (this.LastProcessedSegment is PropertySegment || this.LastProcessedSegment is KeySegment || this.LastProcessedSegment is NavigationPropertySegment)
            {
                this.LastProcessedSegment = segment;
                return this.ResultExpression;
            }

            throw new InvalidOperationException("Unsupported URI segment before ValueSegment");
        }

        /// <summary>
        /// Translate MetadataSegment to linq expression.
        /// </summary>
        /// <param name="segment">The MetadataSegment</param>
        /// <returns>The linq expression</returns>
        public override Expression Translate(MetadataSegment segment)
        {
            throw new InvalidOperationException("Unexpected $metadata in Uri.");
        }
    }
}
