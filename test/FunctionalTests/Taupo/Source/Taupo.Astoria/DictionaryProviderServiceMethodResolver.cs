//---------------------------------------------------------------------
// <copyright file="DictionaryProviderServiceMethodResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Dictionary provider specific class for resolving service method dependencies
    /// </summary>
    public class DictionaryProviderServiceMethodResolver : ServiceMethodResolver
    {
        private const string ContextInstancePropertyName = "contextInstance";

        /// <summary>
        /// Gets the context expression.
        /// </summary>
        /// <returns>
        /// The context expression
        /// </returns>
        protected override CodeExpression GetContextExpression()
        {
            // this.contextInstance;
            return Code.This().Property(ContextInstancePropertyName);
        }

        /// <summary>
        /// Rewrites the code if needed.
        /// </summary>
        /// <param name="toRewrite">The code expression to rewrite.</param>
        /// <param name="model">The model.</param>
        /// <returns>
        /// The rewritten expression
        /// </returns>
        protected override CodeExpression RewriteCodeIfNeeded(CodeExpression toRewrite, EntityModelSchema model)
        {
            var rw = new DictionaryProviderServiceMethodCodeRewriter(model);
            return rw.Rewrite(toRewrite);
        }

        /// <summary>
        /// Rewrites the service method code to suit the dictionary provider
        /// </summary>
        private class DictionaryProviderServiceMethodCodeRewriter : CodeDomTreeRewriter
        {
            private const string DefaultInstanceTypeName = "Microsoft.Test.Taupo.DataServices.Dictionary.ResourceInstance";
            private const string ResourceSetsPropertyName = "ResourceSets";
            private const string RootQueryMethodName = "GetQueryRootForResourceSet";

            private readonly EntityModelSchema model = null;
            private EntitySet rootEntitySet = null;
            
            internal DictionaryProviderServiceMethodCodeRewriter(EntityModelSchema model)
            {
                this.model = model;
            }

            /// <summary>
            /// Rewrites references to entities with dictionary provider specific CodeDom to do the correct lookup.
            /// </summary>
            /// <param name="source">the CodeDom Expression to rewrite</param>
            /// <param name="didRewrite">indicates whether or not the expression was rewritten</param>
            /// <returns>the CodeDom expression appropriate for the dictionary provider</returns>
            protected override CodeExpression Rewrite(CodeExpression source, ref bool didRewrite)
            {
                var propertyRef = source as CodePropertyReferenceExpression;
                if (propertyRef != null)
                {
                    return this.RewritePropertyReference(propertyRef, ref didRewrite);
                }

                var lambdaExpression = source as CodeLambdaExpression;
                if (lambdaExpression != null)
                {
                    return this.RewriteLambdaExpression(lambdaExpression, ref didRewrite);
                }

                var arrayExpression = source as CodeAnonymousArrayExpression;
                if (arrayExpression != null)
                {
                    return this.RewriteAnonymousArray(arrayExpression, ref didRewrite);
                }

                ExceptionUtilities.Assert(
                    source.GetType().Namespace == typeof(CodeObject).Namespace,
                    "Taupo-defined code expressions need to be explicitly handled or the base tree rewriter will lose them");

                return base.Rewrite(source, ref didRewrite);
            }

            private CodeExpression RewritePropertyReference(CodePropertyReferenceExpression propertyReference, ref bool didRewrite)
            {
                var targetRef = propertyReference.TargetObject as CodePropertyReferenceExpression;
                if (targetRef != null)
                {
                    //
                    // Replace this pattern :
                    //      context.entity
                    //
                    // With:
                    //      context.GetQueryRootForResourceSet(<entity reference name>).Cast<entity type>()
                    //
                    if (targetRef.PropertyName.Equals(DictionaryProviderServiceMethodResolver.ContextInstancePropertyName))
                    {
                        var entitySet = this.model.GetDefaultEntityContainer().EntitySets.SingleOrDefault(s => s.Name == propertyReference.PropertyName);
                        if (entitySet != null)
                        {
                            this.rootEntitySet = entitySet;

                            var setLambda = Code.Lambda().WithParameter("s").WithBody(Code.Argument("s").Property("Name").Equal(Code.Primitive(entitySet.Name), DataTypes.String));
                            var setReference = Code.This().Property(DictionaryProviderServiceMethodResolver.ContextInstancePropertyName).Property(ResourceSetsPropertyName).Call("Single", setLambda);
                            var setBaseTypeReference = this.GetClrTypeReference(entitySet.EntityType);

                            didRewrite = true;
                            return targetRef.Call(RootQueryMethodName, setReference).Call("Cast", new CodeTypeReference[] { setBaseTypeReference });
                        }
                    }
                }
                else if (propertyReference.TargetObject is CodeArgumentReferenceExpression)
                {
                    if (this.rootEntitySet != null)
                    {
                        // TODO: more derived types?
                        var entityType = this.rootEntitySet.EntityType;
                        var property = entityType.AllProperties.Where(p => p.Name == propertyReference.PropertyName && !p.IsTypeBacked()).SingleOrDefault();
                        if (property != null)
                        {
                            //
                            // Replace this pattern:
                            //      arg.EntityProperty    
                            // With:
                            //      (<property type>)arg[<PropertyName>]
                            //
                            var propertyType = property.PropertyType;

                            CodeTypeReference propertyTypeRef = Code.TypeRef(typeof(object));
                            if (propertyType != null)
                            {
                                var primitivePropertyType = propertyType as PrimitiveDataType;
                                if (primitivePropertyType != null)
                                {
                                    var clrType = primitivePropertyType.GetFacetValue<PrimitiveClrTypeFacet, Type>(typeof(object));

                                    // We need to make sure we have the right primitive type for the cast below. 
                                    // If its a nullable property, then the CLR type we cast to must be nullable as well
                                    bool clrTypeIsNullable = clrType.IsClass() || Nullable.GetUnderlyingType(clrType) != null;
                                    if (primitivePropertyType.IsNullable && !clrTypeIsNullable)
                                    {
                                        clrType = typeof(Nullable<>).MakeGenericType(clrType);
                                    }

                                    propertyTypeRef = Code.TypeRef(clrType);
                                }

                                var complexPropertyType = propertyType as ComplexDataType;
                                if (complexPropertyType != null)
                                {
                                    propertyTypeRef = this.GetClrTypeReference(complexPropertyType.Definition);
                                }
                            }

                            didRewrite = true;
                            return propertyReference.TargetObject.Index(Code.Primitive(propertyReference.PropertyName)).Cast(propertyTypeRef);
                        }
                    }
                }

                return base.Rewrite(propertyReference, ref didRewrite);
            }

            private CodeExpression RewriteLambdaExpression(CodeLambdaExpression lambdaExpression, ref bool didRewrite)
            {
                // This is a workaround for CodeDomTreeRewriter which does not handle CodeLambdaExpression in the case when
                // expressions further down the tree have been rewritten. (the lambda is rewritten as an empty CodeObject).
                var body = this.Rewrite(lambdaExpression.Body, ref didRewrite);
                if (didRewrite)
                {
                    var parameters = lambdaExpression.Parameters.Cast<CodeParameterDeclarationExpression>().Select(e => e.Name).ToArray();
                    var result = new CodeLambdaExpression().WithParameters(parameters).WithBody(body);
                    return result;
                }

                return lambdaExpression;
            }

            private CodeExpression RewriteAnonymousArray(CodeAnonymousArrayExpression arrayExpression, ref bool didRewrite)
            {
                var rewrittenElements = new List<CodeExpression>();
                bool elementRewritten = false;
                foreach (var element in arrayExpression.Elements)
                {
                    rewrittenElements.Add(this.Rewrite(element, ref elementRewritten));
                    didRewrite |= elementRewritten;
                }

                if (didRewrite)
                {
                    return Code.Array(rewrittenElements.ToArray());
                }

                return arrayExpression;
            }

            private CodeTypeReference GetClrTypeReference(NamedStructuralType type)
            {
                if (type.IsTypeBacked())
                {
                    return Code.TypeRef(type.FullName);
                }
                else
                {
                    return Code.TypeRef(DefaultInstanceTypeName);
                }
            }
        }
    }
}
