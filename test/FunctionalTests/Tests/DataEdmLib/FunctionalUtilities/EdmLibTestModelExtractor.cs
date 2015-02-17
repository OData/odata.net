//---------------------------------------------------------------------
// <copyright file="EdmLibTestModelExtractor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.OData.Utils.Metadata;

    public class EdmLibTestModelExtractor
    {
        public Dictionary<string, T> GetModels<T>(Type type, EdmVersion edmVersion) where T : class
        {
            return GetModels<T>(type.GetMethods().Where(n => n.IsStatic == true && n.ReturnType == typeof(T)), edmVersion);
        }

        public Dictionary<string, T> GetModels<T>(Type type, EdmVersion edmVersion, Attribute testAttribute, bool isInclusiveAttribute) where T : class
        {
            Func<MethodInfo, bool> isTestApplicable = (method) => !(method.GetCustomAttributes(testAttribute.GetType(), false).Any() ^ isInclusiveAttribute);
            return GetModels<T>(type.GetMethods().Where(n => n.IsStatic == true && n.ReturnType == typeof(T) && isTestApplicable(n)), edmVersion);
        }

        private Dictionary<string, T> GetModels<T>(IEnumerable<MethodInfo> modelBuilderModethods, EdmVersion edmVersion) where T : class
        {
            var testModels = new Dictionary<string, T>();
            foreach (var method in modelBuilderModethods)
            {
                var parameters = new List<object>();
                if (method.GetParameters().Any())
                {
                    if (method.GetParameters().Single().ParameterType == typeof(EdmVersion))
                    {
                        parameters.Add(edmVersion);
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("The model builder method {0} can take no or only EdmVersion parameter.", method.Name));
                    }
                }
                var model = method.Invoke(null, parameters.ToArray<object>()) as T;
                if (null == model)
                {
                    throw new InvalidOperationException(string.Format("The extracted test model from {0} cannot be null.", method.Name));
                }
                testModels.Add(method.Name, model);
            }
            return testModels;
        }
    }
}