//---------------------------------------------------------------------
// <copyright file="DSPMethodsImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Reflection;
using Microsoft.OData.Service.Providers;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using DSP = Microsoft.OData.Service.Providers;
using System.Data.Test.Astoria.CallOrder;

namespace System.Data.Test.Astoria.LateBound
{
    #region base class
    public abstract class DSPMethodsImplementation
    {
        public IDataServiceQueryProvider QueryProvider
        {
            get;
            set;
        }

        public static MethodInfo GetImplementation(MethodInfo lateBoundMethod)
        {
            if (lateBoundMethod.ReflectedType != typeof(DataServiceProviderMethods))
                throw new ArgumentException("Given method is not a DataServiceProviderMethod: " + lateBoundMethod.ToString());

            return typeof(DSPMethodsImplementation).GetMethods().SingleOrDefault(m => m.Name == lateBoundMethod.Name && m.GetParameters().Length == lateBoundMethod.GetParameters().Length);
        }

        public static bool MethodHasImplementation(MethodInfo method)
        {
            return method.Name == "Compare" ||
                   method.Name == "AreByteArraysEqual" ||
                   method.Name == "AreByteArraysNotEqual";
        }

        public Expression ConvertMethodCall(MethodInfo lateBoundMethod, params Expression[] parameters)
        {
            return ConvertMethodCall(lateBoundMethod, parameters.AsEnumerable());
        }

        public Expression ConvertMethodCall(MethodInfo lateBoundMethod, IEnumerable<Expression> parameters)
        {
            MethodInfo implementation = GetImplementation(lateBoundMethod);
            if (implementation == null)
            {
                throw new ArgumentNullException("Could not find corresponding method for: " + lateBoundMethod.ToString());
            }

            if (lateBoundMethod.IsGenericMethod)
            {
                implementation = implementation.MakeGenericMethod(lateBoundMethod.GetGenericArguments());
            }

            Expression constant = Expression.Constant(this, typeof(DSPMethodsImplementation));
            return Expression.Call(constant, implementation, parameters);
        }

        public abstract object GetValue(object value, DSP.ResourceProperty property);

        public abstract object Convert(object value, DSP.ResourceType type);
        
        public abstract bool TypeIs(object value, DSP.ResourceType type);

        public abstract IEnumerable<T> GetSequenceValue<T>(object value, DSP.ResourceProperty property);

        public virtual IQueryable<TResult> OfType<TSource, TResult>(IQueryable<TSource> query, ResourceType resourceType)
        {
            throw new NotImplementedException();
        }

        public virtual T TypeAs<T>(object value, ResourceType type)
        {
            throw new NotImplementedException();
        }

        protected internal virtual bool ShouldReplaceMethod(MethodInfo method)
        {
            return !DSPMethodsImplementation.MethodHasImplementation(method);
        }
    }

    public class DefaultDSPMethodsImplementation : DSPMethodsImplementation
    {
        public override object GetValue(object value, DSP.ResourceProperty property)
        {
            APICallLog.Current.Push();
            try
            {
                return QueryProvider.GetPropertyValue(value, property);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public override object Convert(object value, DSP.ResourceType type)
        {
            return System.Convert.ChangeType(value, type.InstanceType);
        }

        public override bool TypeIs(object value, DSP.ResourceType type)
        {
            APICallLog.Current.Push();
            try
            {
                if (value == null)
                    return false;

                DSP.ResourceType instanceType = QueryProvider.GetResourceType(value);
                if (instanceType != null)
                {
                    IDataServiceMetadataProvider metadataProvider = QueryProvider as IDataServiceMetadataProvider;
                    if (metadataProvider != null && metadataProvider.HasDerivedTypes(type))
                    {
                        if (metadataProvider.GetDerivedTypes(type).Contains(instanceType))
                            return true;
                    }
                    return type == instanceType;
                }

                return type.InstanceType.IsAssignableFrom(value.GetType());
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public override IEnumerable<T> GetSequenceValue<T>(object value, DSP.ResourceProperty property)
        {
            APICallLog.Current.Push();
            try
            {
                return QueryProvider.GetPropertyValue(value, property) as IEnumerable<T>;
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }
    }
    #endregion
}
