//---------------------------------------------------------------------
// <copyright file="AlphabetFileParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Parses alphabet files
    /// </summary>
    internal class AlphabetFileParser
    {
        /// <summary>
        /// Parses the alphabet file.
        /// </summary>
        /// <param name="reader">The text reader with alphabet file contents.</param>
        /// <returns>List of strings in the alphabet file.</returns>
        public static string[] ParseAlphabetFile(TextReader reader)
        {
            List<string> entries = new List<string>();
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                string entry = ParseLine(line);
                if (entry != null)
                {
                    entries.Add(entry);
                }
            }

            return entries.ToArray();
        }

        private static string ParseLine(string line)
        {
            if (!line.StartsWith("'", StringComparison.Ordinal))
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 1; i < line.Length; ++i)
            {
                if (line[i] == '\'')
                {
                    break;
                }

                if (line[i] == '\\')
                {
                    if (line[i + 1] == 'u')
                    {
                        int codePoint = Convert.ToInt32(line.Substring(i + 2, 4), 16);
                        sb.Append((char)codePoint);

                        // skip 'uXXXX'
                        i += 5;
                    }
                    else
                    {
                        sb.Append(line[++i]);
                    }
                }
                else
                {
                    sb.Append(line[i]);
                }
            }

            return sb.ToString();
        }
    }
}
