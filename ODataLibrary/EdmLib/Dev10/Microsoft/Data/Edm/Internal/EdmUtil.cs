//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Linq;

namespace Microsoft.Data.Edm
{
    internal static class EdmUtil
    {
        internal static bool TryGetNamespaceNameFromQualifiedName(string qualifiedName, out string namespaceName, out string name)
        {
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

        public static bool IsNullOrWhiteSpaceInternal(String value)
        {
            return value == null || value.All(Char.IsWhiteSpace);
        }

        internal static void CheckArgumentNull<T>([ValidatedNotNullAttribute]T value, string parameterName) where T : class
        {
            if (null == value)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        // Hack to alert FXCop that we do check for null.
        private sealed class ValidatedNotNullAttribute : Attribute
        {
        }

        internal static bool EqualsOrdinal(this string string1, string string2)
        {
            return string.Equals(string1, string2, StringComparison.Ordinal);
        }

        internal static bool EqualsOrdinalIgnoreCase(this string string1, string string2)
        {
            return string.Equals(string1, string2, StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsValidDataModelItemName(string name)
        {
            return IsValidUndottedName(name);
        }

        internal static bool IsValidQualifiedItemNamespace(string name)
        {
            return IsValidDottedName(name);
        }

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
        //private static Regex ValidDottedName=new Regex(@"^"+NameExp+@"(\."+NameExp+@"){0,}$",RegexOptions.Singleline);
#if SILVERLIGHT || ORCAS
        // from : http://msdn.microsoft.com/en-us/library/system.text.regularexpressions.regexoptions(VS.95).aspx#1
        // RegexOptions.Compiled does not exists in Silverlight
        private static System.Text.RegularExpressions.Regex UndottedNameValidator = new System.Text.RegularExpressions.Regex(@"^" + NameExp + @"$", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.None);
#else
        private static System.Text.RegularExpressions.Regex UndottedNameValidator = new System.Text.RegularExpressions.Regex(@"^" + NameExp + @"$", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.Compiled);
#endif

        private static bool IsValidUndottedName(string name)
        {
            // CodeGenerator.IsValidLanguageIndependentIdentifier does demand a FullTrust Link
            // but this is safe since the function only walks over the string no risk is introduced
            return (!string.IsNullOrEmpty(name) && UndottedNameValidator.IsMatch(name));
        }

        private static bool IsValidDottedName(string name)
        {
            // each part of the dotted name needs to be a valid name
            return name.Split('.').All(IsValidUndottedName);
        }
    }
}
