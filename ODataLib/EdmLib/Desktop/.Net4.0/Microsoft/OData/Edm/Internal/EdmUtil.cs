//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.Internal.CsdlSemantics;

namespace Microsoft.OData.Edm
{
    internal static class EdmUtil
    {
        // this is what we should be doing for CDM schemas
        // the RegEx for valid identifiers are taken from the C# Language Specification (2.4.2 Identifiers)
        // (except that we exclude _ as a valid starting character).
        // This results in a somewhat smaller set of identifier from what System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier
        // allows. Not all identifiers allowed by IsValidLanguageIndependentIdentifier are valid in C#.IsValidLanguageIndependentIdentifier allows:
        //    Mn, Mc, and Pc as a leading character (which the spec and C# (at least for some Mn and Mc characters) do not allow)
        //    characters that Char.GetUnicodeCategory says are in Nl and Cf but which the RegEx does not accept (and which C# does allow).
        //
        // we could create the StartCharacterExp and OtherCharacterExp dynamically to force inclusion of the missing Nl and Cf characters...
        private const string StartCharacterExp = @"[\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}]";
        private const string OtherCharacterExp = @"[\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]";
        private const string NameExp = StartCharacterExp + OtherCharacterExp + "{0,}";

        // private static Regex ValidDottedName=new Regex(@"^"+NameExp+@"(\."+NameExp+@"){0,}$",RegexOptions.Singleline);
        private static Regex UndottedNameValidator = PlatformHelper.CreateCompiled(@"^" + NameExp + @"$", RegexOptions.Singleline);
        
        public static bool IsNullOrWhiteSpaceInternal(String value)
        {
            return value == null || value.ToCharArray().All(Char.IsWhiteSpace);
        }

        public static String JoinInternal<T>(String separator, IEnumerable<T> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            if (separator == null)
            {
                separator = String.Empty;
            }

            using (IEnumerator<T> en = values.GetEnumerator())
            {
                if (!en.MoveNext())
                {
                    return String.Empty;
                }

                StringBuilder result = new StringBuilder();
                if (en.Current != null)
                {
                    // handle the case that the enumeration has null entries
                    // and the case where their ToString() override is broken
                    string value = en.Current.ToString();
                    if (value != null)
                    {
                        result.Append(value);
                    }
                }

                while (en.MoveNext())
                {
                    result.Append(separator);
                    if (en.Current != null)
                    {
                        // handle the case that the enumeration has null entries
                        // and the case where their ToString() override is broken
                        string value = en.Current.ToString();
                        if (value != null)
                        {
                            result.Append(value);
                        }
                    }
                }

                return result.ToString();
            }
        }

        // This is testing if the name can be parsed and serialized, not if it is valid.
        public static bool IsQualifiedName(string name)
        {
            string[] nameTokens = name.Split('.');
            if (nameTokens.Count() < 2)
            {
                return false;
            }

            foreach (string token in nameTokens)
            {
                if (IsNullOrWhiteSpaceInternal(token))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsValidUndottedName(string name)
        {
            return (!string.IsNullOrEmpty(name) && UndottedNameValidator.IsMatch(name));
        }

        public static bool IsValidDottedName(string name)
        {
            // Each part of the dotted name needs to be a valid name.
            return name.Split('.').All(IsValidUndottedName);
        }

        public static string ParameterizedName(IEdmFunctionBase function)
        {
            int index = 0;
            int parameterCount = function.Parameters.Count();
            StringBuilder sb = new StringBuilder();

            UnresolvedOperation unresolvedOperationImport = function as UnresolvedOperation;
            if (unresolvedOperationImport != null)
            {
                sb.Append(unresolvedOperationImport.Namespace);
                sb.Append("/");
                sb.Append(unresolvedOperationImport.Name);
                return sb.ToString();
            }

            // If we have a operation (rather than a operation import), we want the parameterized name to include the namespace
            IEdmSchemaElement schemaFunction = function as IEdmSchemaElement;
            if (schemaFunction != null)
            {
                sb.Append(schemaFunction.Namespace);
                sb.Append(".");
            }

            sb.Append(function.Name);
            sb.Append("(");
            foreach (IEdmOperationParameter parameter in function.Parameters)
            {
                string typeName = "";
                if (parameter.Type.IsCollection())
                {
                    typeName = CsdlConstants.Value_Collection + "(" + parameter.Type.AsCollection().ElementType().FullName() + ")";
                }
                else if (parameter.Type.IsEntityReference())
                {
                    typeName = CsdlConstants.Value_Ref + "(" + parameter.Type.AsEntityReference().EntityType().FullName() + ")";
                }
                else
                {
                    typeName = parameter.Type.FullName();
                }

                sb.Append(typeName);
                index++;
                if (index < parameterCount)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(")");
            return sb.ToString();
        }

        public static bool TryGetNamespaceNameFromQualifiedName(string qualifiedName, out string namespaceName, out string name)
        {
            // Qualified name can be a operation import name which is separated by '/'
            int lastSlash = qualifiedName.LastIndexOf('/');
            if (lastSlash < 0)
            {
                // Not a OperationImport
                int lastDot = qualifiedName.LastIndexOf('.');
                if (lastDot < 0)
                {
                    namespaceName = string.Empty;
                    name = qualifiedName;
                    return false;
                }

                namespaceName = qualifiedName.Substring(0, lastDot);
                name = qualifiedName.Substring(lastDot + 1);
                return true;
            }

            namespaceName = qualifiedName.Substring(0, lastSlash);
            name = qualifiedName.Substring(lastSlash + 1);
            return true;
        }

        public static string FullyQualifiedName(IEdmVocabularyAnnotatable element)
        {
            IEdmSchemaElement schemaElement = element as IEdmSchemaElement;
            if (schemaElement != null)
            {
                IEdmOperation operation = schemaElement as IEdmOperation;
                if (operation != null)
                {
                    return ParameterizedName(operation);
                }
                else
                {
                    return schemaElement.FullName();
                }
            }
            else
            {
                IEdmEntityContainerElement containerElement = element as IEdmEntityContainerElement;
                if (containerElement != null)
                {
                    IEdmOperationImport operationImport = containerElement as IEdmOperationImport;
                    if (operationImport != null)
                    {
                        return operationImport.Container.FullName() + "/" + ParameterizedName(operationImport);
                    }
                    else
                    {
                        return containerElement.Container.FullName() + "/" + containerElement.Name;
                    }
                }
                else
                {
                    IEdmProperty property = element as IEdmProperty;
                    if (property != null)
                    {
                        IEdmSchemaType declaringSchemaType = property.DeclaringType as IEdmSchemaType;
                        if (declaringSchemaType != null)
                        {
                            string propertyOwnerName = FullyQualifiedName(declaringSchemaType);
                            if (propertyOwnerName != null)
                            {
                                return propertyOwnerName + "/" + property.Name;
                            }
                        }
                    }
                    else
                    {
                        IEdmOperationParameter parameter = element as IEdmOperationParameter;
                        if (parameter != null)
                        {
                            string parameterOwnerName = FullyQualifiedName(parameter.DeclaringFunction);
                            if (parameterOwnerName != null)
                            {
                                return parameterOwnerName + "/" + parameter.Name;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static T CheckArgumentNull<T>([ValidatedNotNullAttribute]T value, string parameterName) where T : class
        {
            if (null == value)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static bool EqualsOrdinal(this string string1, string string2)
        {
            return string.Equals(string1, string2, StringComparison.Ordinal);
        }

        public static bool EqualsOrdinalIgnoreCase(this string string1, string string2)
        {
            return string.Equals(string1, string2, StringComparison.OrdinalIgnoreCase);
        }

        // Hack to alert FXCop that we do check for null.
        private sealed class ValidatedNotNullAttribute : Attribute
        {
        }
    }
}
