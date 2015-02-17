//---------------------------------------------------------------------
// <copyright file="CustomUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace System.Data.Test.Astoria.CustomData.Runtime
{
    // --- CODE SNIPPET START ---
    
    //
    // CustomUtils class
    //
    #region CustomUtils class

    public static class CustomUtils
    {
        #region Helper methods

        public static void CheckArgumentNotNull(object value, string argumentName)
        {
            if (null == value)
            {
                throw new ArgumentNullException(Format("'{0}' cannot be null.", argumentName));
            }
        }

        public static void CheckArgumentLengthLimit(int actualLength, int maxLength, string argumentName)
        {
            if (actualLength > maxLength)
            {
                throw new ArgumentOutOfRangeException(Format(
                                "Length of '{0}' must be less or equal to {1}. Actual length is {2}.",
                                argumentName, maxLength, actualLength
                            ));
            }
        }

        public static string FormatCsvString(IEnumerable<string> strings)
        {
            return FormatCsvString(strings, true);
        }

        public static string FormatCsvString(IEnumerable<string> strings, bool quoteNeeded)
        {
            StringBuilder sb = new StringBuilder();

            string separator = "";
            foreach (string str in strings)
            {
                sb.Append(separator);

                if (quoteNeeded)
                {
                    sb.Append('\'');
                }

                sb.Append(str);

                if (quoteNeeded)
                {
                    sb.Append('\'');
                }

                separator = ", ";
            }

            return sb.ToString();
        }

        private static string Format(string message, params object[] args)
        {
            CheckArgumentNotNull(message, "message");

            string formattedMessage = message;
            if (args != null && args.Length > 0)
            {
                formattedMessage = String.Format(message, args);
            }

            return formattedMessage;
        }

        #endregion
    }

    #endregion

    // --- CODE SNIPPET END ---
}
