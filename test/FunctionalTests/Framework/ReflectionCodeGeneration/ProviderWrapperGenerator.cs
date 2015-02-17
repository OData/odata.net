//---------------------------------------------------------------------
// <copyright file="ProviderWrapperGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace System.Data.Test.Astoria
{
    public static class ProviderWrapperGenerator
    {
        public static string TypeToCode(Type t)
        {
            if (t.IsGenericType)
            {
                string name = t.Name;
                if (name.Contains('`'))
                    name = name.Substring(0, name.IndexOf('`'));
                return t.Namespace + "." + name + "<" + string.Join(", ", t.GetGenericArguments().Select(g => TypeToCode(g)).ToArray()) + ">";
            }
            else
            {
                return t.FullName;
            }
        }

        public static string ParameterToCode(ParameterInfo info, bool includeType)
        {
            string asCode = string.Empty;
            if (info.IsOut)
                asCode += "out ";

            if (includeType)
            {
                string type = TypeToCode(info.ParameterType);
                if (type.EndsWith("&"))
                    type = type.Remove(type.Length - 1);
                asCode += type + " ";
            }

            asCode += info.Name;
            return asCode;
        }

        public static string Generate(string typeName, params Type[] toDeriveFromOrImplement)
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine("public class " + typeName + " : " + string.Join(", ", toDeriveFromOrImplement.Select(t => t.FullName).ToArray()));
            code.AppendLine("{");
            code.AppendLine("   private Dictionary<Type,object> underlyingProviders = new Dictionary<Type,object>();");
            code.AppendLine("   private T UnderlyingProvider<T>()");
            code.AppendLine("   {");
            code.AppendLine("       return (T)underlyingProviders[typeof(T)];");
            code.AppendLine("   }");

            code.AppendLine("   public " + typeName + "(params object[] providers)");
            code.AppendLine("   {");
            code.AppendLine("       List<object> list = providers.ToList();");
            // in case we derive from a real type, use 'this' as the first of the underlying providers
            code.AppendLine("       list.Insert(0,this);");
            code.AppendLine("       foreach(object p in list)");
            code.AppendLine("           foreach(Type t in p.GetType().GetInterfaces())");
            code.AppendLine("               underlyingProviders[t] = p;");
            code.AppendLine("   }");

            IEnumerable<Type> allInterfaces = toDeriveFromOrImplement.Where(t => t.IsInterface);
            allInterfaces = allInterfaces.Union(allInterfaces.SelectMany(i => i.GetInterfaces()));

            foreach (Type type in allInterfaces)
            {
                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    code.Append("   ");
                    code.Append(TypeToCode(property.PropertyType));
                    code.AppendLine(" " + type.FullName + "." + property.Name);
                    code.AppendLine("   {");
                    if (property.CanRead)
                        code.AppendLine("       get { return UnderlyingProvider<" + type.FullName + ">()." + property.Name + "; }");
                    if (property.CanWrite)
                        code.AppendLine("       set { UnderlyingProvider<" + type.FullName + ">()." + property.Name + " = value; }");
                    code.AppendLine("   }");
                    code.AppendLine();
                }

                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
                        continue;

                    code.Append("   ");
                    if (method.ReturnType == typeof(void))
                        code.Append("void");
                    else
                        code.Append(TypeToCode(method.ReturnType));

                    code.AppendLine(" " + type.FullName + "." + method.Name + "(" + string.Join(", ",
                        method.GetParameters().Select(p => ParameterToCode(p, true)).ToArray()) + ")");
                    code.AppendLine("   {");
                    code.Append("       ");
                    if (method.ReturnType != typeof(void))
                        code.Append("return ");
                    code.AppendLine("UnderlyingProvider<" + type.FullName + ">()." + method.Name + "(" + string.Join(", ", method.GetParameters().Select(p => ParameterToCode(p, false)).ToArray()) + ");");
                    code.AppendLine("   }");
                    code.AppendLine();
                }
            }
            code.AppendLine("}");

            return code.ToString();
        }

        public static string GenerateDerivedWrapper(string typeName, Type baseType, params Type[] interfaces)
        {
            StringBuilder code = new StringBuilder();

            code.Append("public class " + typeName + " : " + baseType.FullName);
            if (interfaces.Length > 0)
                code.Append(", " + string.Join(", ", interfaces.Select(t => t.FullName).ToArray()));
            code.AppendLine();
            code.AppendLine("{");


#if !ClientSKUFramework
           foreach (Type interfaceType in interfaces) 
           {
                foreach (MethodInfo method in interfaceType.GetMethods())
                {
                    code.Append("   public override ");
                    if (method.ReturnType == typeof(void))
                        code.Append("void");
                    else
                        code.Append(TypeToCode(method.ReturnType));

                    string methodCall = method.Name + "(" + string.Join(", ", method.GetParameters().Select(p => ParameterToCode(p, false)).ToArray()) + ");";

                    code.AppendLine(" " + method.Name + "(" + string.Join(", ",
                        method.GetParameters().Select(p => ParameterToCode(p, true)).ToArray()) + ")");
                    code.AppendLine("   {");

                    if (interfaceType == typeof(Microsoft.OData.Service.IUpdatable))
                        code.AppendLine("       System.Data.Test.Astoria.CallOrder.APICallLog.Current.Updatable." + methodCall);
                    else if (interfaceType == typeof(Microsoft.OData.Service.IExpandProvider))
                        code.AppendLine("       System.Data.Test.Astoria.CallOrder.APICallLog.Current.ExpandProvider." + methodCall);
                    else
                        AstoriaTestLog.FailAndThrow("Not sure how to build wrapper for type '" + interfaceType.FullName + "'");

                    code.AppendLine("       try");
                    code.AppendLine("       {");
                    code.Append("           ");
                    if (method.ReturnType != typeof(void))
                        code.Append("return ");
                    code.AppendLine("base." + method.Name + "(" + string.Join(", ", method.GetParameters().Select(p => ParameterToCode(p, false)).ToArray()) + ");");
                    code.AppendLine("       }");
                    code.AppendLine("       finally");
                    code.AppendLine("       {");
                    code.AppendLine("           System.Data.Test.Astoria.CallOrder.APICallLog.Current.Pop();");
                    code.AppendLine("       }");
                    code.AppendLine("   }");
                    code.AppendLine();
                }
            }
#endif
            code.AppendLine("}");
            return code.ToString();
        }
    }
}
