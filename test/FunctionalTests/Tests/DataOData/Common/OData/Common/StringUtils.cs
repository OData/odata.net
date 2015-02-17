//---------------------------------------------------------------------
// <copyright file="StringUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Helper methods to work with strings
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Resolves variables in form of $(variablename)
        /// </summary>
        /// <param name="input">The input string to resolve variables in.</param>
        /// <param name="variables">Dictionary of key (variable name) and value (variable value) to use for resolving</param>
        /// <returns>The input string with all variables resolved.</returns>
        public static string ResolveVariables(string input, Dictionary<string, string> variables)
        {
            return ResolveVariables(
                input,
                variableName =>
                {
                    string variableValue = null;
                    variables.TryGetValue(variableName, out variableValue);
                    return variableValue;
                });
        }

        /// <summary>
        /// Resolves variables in form of $(variablename)
        /// </summary>
        /// <param name="input">The input string to resolve variables in.</param>
        /// <param name="resolver">The resolver which gets the variable name and returns the value to resolve it with.</param>
        /// <returns>The input string with all variables resolved.</returns>
        /// <remarks>The resolver should return null if the variable was not recognized.</remarks>
        public static string ResolveVariables(string input, Func<string, string> resolver)
        {
            ExceptionUtilities.CheckArgumentNotNull(resolver, "resolver");

            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            int startIndex = 0;
            StringBuilder result = null;
            while (startIndex >= 0 && startIndex < input.Length)
            {
                int variableReferenceStart = input.IndexOf("$(", startIndex);
                if (variableReferenceStart >= 0)
                {
                    int variableReferenceEnd = variableReferenceStart + 2;
                    while (variableReferenceEnd < input.Length)
                    {
                        if (input[variableReferenceEnd] == ')')
                        {
                            if (variableReferenceEnd + 1 < input.Length && input[variableReferenceEnd + 1] == ')')
                            {
                                // Double )) - escape sequence
                                variableReferenceEnd += 2;
                            }
                            else
                            {
                                // Found the end
                                break;
                            }
                        }
                        else
                        {
                            variableReferenceEnd++;
                        }
                    }

                    if (variableReferenceEnd < input.Length)
                    {
                        string variableName = input.Substring(variableReferenceStart + 2, variableReferenceEnd - variableReferenceStart - 2);
                        if (result == null)
                        {
                            result = new StringBuilder();
                        }

                        result.Append(input.Substring(startIndex, variableReferenceStart - startIndex));
                        string variableValue = resolver(variableName);
                        if (variableValue == null)
                        {
                            throw new InvalidOperationException(string.Format(
                                "Unrecognized variable '{0}' in string: {1}.", variableName, input));
                        }

                        // Resolve the variableValue recursively - supports variables which resolve to other variables
                        variableValue = ResolveVariables(variableValue, resolver);

                        result.Append(variableValue);
                        startIndex = variableReferenceEnd + 1;
                        continue;
                    }
                }

                break;
            }

            if (result != null)
            {
                if (startIndex < input.Length)
                {
                    result.Append(input.Substring(startIndex));
                }

                return result.ToString();
            }
            else
            {
                return input;
            }
        }

        /// <summary>
        /// Concatenates all strings in an enumerable of strings.
        /// </summary>
        /// <param name="source">Enumerable of strings to concatenate.</param>
        /// <returns>The resulting string.</returns>
        public static string Concatenate(this IEnumerable<string> source)
        {
            return source.Concatenate("");
        }

        /// <summary>
        /// Concatenates all string in an enumerable of string.
        /// </summary>
        /// <param name="source">Enumerable of string to concatenate.</param>
        /// <param name="separator">The seprator to put between each pair of strings.</param>
        /// <returns>The resulting string.</returns>
        public static string Concatenate(this IEnumerable<string> source, string separator)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (string s in source)
            {
                if (!first)
                {
                    sb.Append(separator);
                }

                sb.Append(s);
                first = false;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Given a sequence of strings and string arrays flattens them into a single string array.
        /// </summary>
        /// <param name="inputs">The sequence of input strings and/or string arrays.</param>
        /// <returns>The flattened list of strings as array.</returns>
        public static string[] Flatten(params object[] inputs)
        {
            if (inputs == null)
            {
                return null;
            }

            List<string> flattenedStrings = new List<string>();

            foreach (object o in inputs)
            {
                if (o == null)
                {
                    flattenedStrings.Add(null);
                }
                else
                {
                    string asString = o as string;
                    if (asString != null)
                    {
                        flattenedStrings.Add(asString);
                    }
                    else
                    {
                        string[] asStringArray = o as string[];
                        if (asStringArray != null)
                        {
                            flattenedStrings.AddRange(asStringArray);
                        }
                        else
                        {
                            object[] asObjectArray = o as object[];
                            if (asObjectArray != null)
                            {
                                flattenedStrings.AddRange(Flatten(asObjectArray));
                            }
                            else
                            {
                                throw new NotSupportedException();
                            }
                        }
                    }
                }
            }

            return flattenedStrings.ToArray();
        }

        /// <summary>
        /// Returns the string representation of the line feed characters.
        /// </summary>
        /// <param name="lineFeedChars">The line feed characters to escape.</param>
        /// <returns>The string representation of the line feed characters.</returns>
        public static string LineFeedString(char[] lineFeedChars)
        {
            ExceptionUtilities.CheckArgumentNotNull(lineFeedChars, "lineFeedChars");

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < lineFeedChars.Length; ++i)
            {
                switch (lineFeedChars[i])
                {
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    default:
                        throw new NotSupportedException("Line feed char '" + lineFeedChars[i] + "' (" + (int)lineFeedChars[i] + ") not supported.");
                }
            }

            return builder.ToString();
        }
    }
}
