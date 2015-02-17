//---------------------------------------------------------------------
// <copyright file="TypeFunctionHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.BuiltInFunctionHelper
{
    using System;
    using Microsoft.OData.Edm;

    public class TypeFunctionHelper
    {
        public static TOut TypeCastFunction<TOut, TIn>(TIn instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("Cast input");
            }

            try
            {
                if (typeof(IConvertible).IsAssignableFrom(instance.GetType()) &&
                    typeof(IConvertible).IsAssignableFrom(typeof(TOut)) &&
                    !typeof(TOut).IsEnum)
                {
                    return (TOut)Convert.ChangeType(instance, typeof(TOut));
                }

                else if (typeof(TOut).IsEnum && typeof(TIn) == typeof(string))
                {
                    //Convert string to enum;
                    var enumValue = Enum.Parse(typeof(TOut), instance.ToString(), true);
                    return (TOut)enumValue;
                }
                else
                {
                    return (TOut)(object)instance;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(string.Format("Can't convert type {0} to type {1}", instance.GetType(), typeof(TOut)), ex);
            }
        }
    }
}