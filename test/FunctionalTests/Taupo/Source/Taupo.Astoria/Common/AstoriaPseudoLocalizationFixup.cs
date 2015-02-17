//---------------------------------------------------------------------
// <copyright file="AstoriaPseudoLocalizationFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.EntityModel;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Linq;

    /// <summary>
    /// Modifies names of all types, members, containers and sets in the Astoria model by translating them using 
    /// a specified instance of <see cref="IPseudoLocalizer"/>.
    /// </summary>
    [ImplementationName(typeof(IAstoriaGlobalizationFixup), "AstoriaPLOC", HelpText = "Performs simple pseudo-localzation of all names in the Astoria model.")]
    public class AstoriaPseudoLocalizationFixup : PseudoLocalizationFixup, IAstoriaGlobalizationFixup
    {
        /// <summary>
        /// Modifies names of all types, members, containers and sets in the Astoria model by translating them using 
        /// a specified instance of <see cref="IPseudoLocalizer"/>.
        /// </summary>
        /// <param name="model">Model to perform fixup on</param>
        public override void Fixup(EntityModelSchema model)
        {
            base.Fixup(model);

            var functionBodyPseudoLocalizer = new QueryExpressionPseudoLocalizer(this.PseudoLocalize);
            foreach (var function in model.Functions)
            {
                function.Name = this.PseudoLocalize(function.Name);
                function.NamespaceName = this.PseudoLocalize(function.NamespaceName);
                var functionBodyAnnotation = (FunctionBodyAnnotation)function.GetAnnotation(typeof(FunctionBodyAnnotation));
                functionBodyAnnotation.FunctionBody = functionBodyPseudoLocalizer.ReplaceExpression(functionBodyAnnotation.FunctionBody);
            }

            foreach (var entityType in model.EntityTypes)
            {
                foreach (var propertyMappingAnnotation in entityType.Annotations.OfType<PropertyMappingAnnotation>())
                {
                    var localizedSourcePath = string.Join("/", propertyMappingAnnotation.SourcePath.Split(new string[] { "/" }, StringSplitOptions.None).Select(p => this.PseudoLocalize(p)));
                    propertyMappingAnnotation.SourcePath = localizedSourcePath;
                }
            }
        }

        private class QueryExpressionPseudoLocalizer : LinqExpressionReplacingVisitor
        {
            private Func<string, string> pseudoLocalizeFunc;

            public QueryExpressionPseudoLocalizer(Func<string, string> pseudoLocalizeFunc)
            {
                this.pseudoLocalizeFunc = pseudoLocalizeFunc;
            }

            public override QueryExpression Visit(QueryConstantExpression expression)
            {
                var oldValue = expression.ScalarValue.Value as string;
                if (oldValue != null)
                {
                    return CommonQueryBuilder.Constant(this.pseudoLocalizeFunc(oldValue), expression.ExpressionType);
                }
                else
                {
                    return expression;
                }
            }

            public override QueryExpression Visit(QueryPropertyExpression expression)
            {
                var pseudoLocalizedName = this.pseudoLocalizeFunc(expression.Name);
                var instance = this.ReplaceExpression(expression.Instance);

                return CommonQueryBuilder.Property(instance, pseudoLocalizedName);
            }

            public override QueryExpression Visit(QueryRootExpression expression)
            {
                var entitySetPropertyInfo = expression.GetType().GetProperties(false, false).Where(p => p.Name == "UnderlyingEntitySet").Single();
                var entitySet = (EntitySet)entitySetPropertyInfo.GetValue(expression, null);

                return CommonQueryBuilder.Root(entitySet);
            }
        }
    }
}
