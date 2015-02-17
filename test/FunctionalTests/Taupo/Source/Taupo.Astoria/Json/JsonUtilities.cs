//---------------------------------------------------------------------
// <copyright file="JsonUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Json
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Json utilities
    /// </summary>
    internal static class JsonUtilities
    {
        /// <summary>
        /// Escapes the given string so that it can be written into a javascript or JSON payload
        /// </summary>
        /// <param name="value">The value to escape</param>
        /// <returns>The escaped value</returns>
        internal static string BuildEscapedJavaScriptString(string value)
        {
            ExceptionUtilities.CheckArgumentNotNull(value, "value");

            // create a string builder with some extra space
            StringBuilder builder = new StringBuilder(value.Length + 6);

            if (value != null)
            {
                int lastWritePosition = 0;
                int skipped = 0;
                char[] chars = null;

                for (int i = 0; i < value.Length; i++)
                {
                    char c = value[i];
                    string escapedValue;

                    switch (c)
                    {
                        case '\t':
                            escapedValue = @"\t";
                            break;
                        case '\n':
                            escapedValue = @"\n";
                            break;
                        case '\r':
                            escapedValue = @"\r";
                            break;
                        case '\f':
                            escapedValue = @"\f";
                            break;
                        case '\b':
                            escapedValue = @"\b";
                            break;
                        case '\\':
                            escapedValue = @"\\";
                            break;
                        case '\u0085': // Next Line
                            escapedValue = @"\u0085";
                            break;
                        case '\u2028': // Line Separator
                            escapedValue = @"\u2028";
                            break;
                        case '\u2029': // Paragraph Separator
                            escapedValue = @"\u2029";
                            break;
                        case '"':
                            escapedValue = "\\\"";
                            break;
                        case '\'':
                            escapedValue = @"\'";
                            break;
                        default:
                            escapedValue = (c <= '\u001f') ? ToCharAsUnicode(c) : null;
                            break;
                    }

                    if (escapedValue != null)
                    {
                        if (chars == null)
                        {
                            chars = value.ToCharArray();
                        }

                        // write skipped text
                        if (skipped > 0)
                        {
                            builder.Append(chars, lastWritePosition, skipped);
                            skipped = 0;
                        }

                        // write escaped value and note position
                        builder.Append(escapedValue);
                        lastWritePosition = i + 1;
                    }
                    else
                    {
                        skipped++;
                    }
                }

                // write any remaining skipped text
                if (skipped > 0)
                {
                    if (lastWritePosition == 0)
                    {
                        builder.Append(value);
                    }
                    else
                    {
                        builder.Append(chars, lastWritePosition, skipped);
                    }
                }
            }

            return builder.ToString();
        }

        internal static string ToCharAsUnicode(char c)
        {
            return string.Format(CultureInfo.InvariantCulture, @"\u{0:x4}", (int)c);
        }
    }
}