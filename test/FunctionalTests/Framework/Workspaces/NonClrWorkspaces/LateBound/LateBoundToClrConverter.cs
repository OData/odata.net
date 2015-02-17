//---------------------------------------------------------------------
// <copyright file="LateBoundToClrConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Service;
using Microsoft.OData.Service.Providers;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Test.Astoria.LateBound
{
    public static class LateBoundToClrConverter
    {
        public static OpenTypeMethodsImplementation OpenTypeMethodsImplementation
        {
            get;
            set;
        }

        public static DSPMethodsImplementation DSPMethodsImplementation
        {
            get;
            set;
        }

        /// <summary>
        /// Convert Late Bound Expression to CLR Expression
        /// </summary>
        /// <param name="ex">The Expression</param>
        /// <param name="provider">The IDSQP provider instance</param>
        /// <returns>The converted expression with no latebound methods</returns>
        public static Expression ToClrExpression(Expression ex, IDataServiceQueryProvider provider)
        {
            if (OpenTypeMethodsImplementation == null)
            {
                OpenTypeMethodsImplementation = new DefaultOpenTypeMethodsImplementation();
            }

            if (DSPMethodsImplementation == null)
            {
                DSPMethodsImplementation = new DefaultDSPMethodsImplementation();
            }

            OpenTypeMethodsImplementation.QueryProvider = provider;
            DSPMethodsImplementation.QueryProvider = provider;
            LateBoundExpressionVisitor visitor = new LateBoundExpressionVisitor(OpenTypeMethodsImplementation, DSPMethodsImplementation);
            return visitor.Visit(ex);
        }
    }
}
