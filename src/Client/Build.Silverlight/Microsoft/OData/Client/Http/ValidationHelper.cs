//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Http
{
    using System;
    using System.Diagnostics;

    /// <summary>Use this class to perform basic string validation.</summary>
    internal static class ValidationHelper
    {
        /// <summary>Whitespace characters that can be trimmed on HTTP values.</summary>
        private static readonly char[] HttpTrimCharacters = new char[] { '\t', '\n', '\v', '\f', '\r', ' ' };

        /// <summary>Characters which are not allowed on HTTP tokens.</summary>
        private static readonly char[] InvalidParamChars = new char[]
        { 
            '(', ')', '<', '>', '@', ',', ';', ':', '\\', '"', '\'', '/', '[', ']', '?', '=',  '{', '}', ' ', '\t', '\r', '\n'
        };

        /// <summary>Checks whether the specified <paramref name="name"/> has any disallowed characters.</summary>
        /// <param name="name">Name to check.</param>
        /// <param name="isHeaderValue">true if <paramref name="name"/> is a header value; false otherwise.</param>
        /// <returns>The name with no whitespace characters if valid; otherwise an exception is thrown.</returns>
        internal static string CheckBadChars(string name, bool isHeaderValue)
        {
            if (String.IsNullOrEmpty(name))
            {
                if (!isHeaderValue)
                {
                    if (name == null)
                    {
                        throw new ArgumentNullException("name");
                    }
                    else
                    {
                        // new ArgumentException("SR.GetString(SR.net_emptystringcall, name), name"));
                        throw new InvalidOperationException(
                            Microsoft.OData.Client.Strings.HttpWeb_InternalArgument("ValidationHelper.CheckBadChars.7", name));
                    }
                }
                
                return string.Empty;
            }
            
            if (isHeaderValue)
            {
                name = name.Trim(HttpTrimCharacters);
                int crlf = 0;
                for (int i = 0; i < name.Length; i++)
                {
                    char c = (char)('\x00ff' & name[i]);
                    switch (crlf)
                    {
                        case 0:
                        {
                            if (c != '\r')
                            {
                                break;
                            }

                            crlf = 1;
                            continue;
                        }
                        
                        case 1:
                        {
                            if (c != '\n')
                            {
                                // throw new ArgumentException("SR.GetString(SR.net_WebHeaderInvalidCRLFChars), value");
                                throw new InvalidOperationException(
                                    Microsoft.OData.Client.Strings.HttpWeb_InternalArgument("ValidationHelper.CheckBadChars", name));
                            }
                            
                            crlf = 2;
                            continue;
                        }
                        
                        case 2:
                        {
                            if ((c != ' ') && (c != '\t'))
                            {
                                // throw new ArgumentException("SR.GetString(SR.net_WebHeaderInvalidCRLFChars), value");
                                throw new InvalidOperationException(
                                    Microsoft.OData.Client.Strings.HttpWeb_InternalArgument("ValidationHelper.CheckBadChars.2", name));
                            }

                            crlf = 0;
                            continue;
                        }
                        
                        default:
                        {
                            continue;
                        }
                    }
                    
                    if (c == '\n')
                    {
                        crlf = 2;
                    }
                    else if ((c == '\x007f') || ((c < ' ') && (c != '\t')))
                    {
                        // throw new ArgumentException("SR.GetString(SR.net_WebHeaderInvalidControlChars), value");
                        throw new InvalidOperationException(
                            Microsoft.OData.Client.Strings.HttpWeb_InternalArgument("ValidationHelper.CheckBadChars.3", name));                        
                    }
                }
                
                if (crlf != 0)
                {
                    // throw new ArgumentException("SR.GetString(SR.net_WebHeaderInvalidCRLFChars), value");
                    throw new InvalidOperationException(
                        Microsoft.OData.Client.Strings.HttpWeb_InternalArgument("ValidationHelper.CheckBadChars.4", name));                    
                }
                
                return name;
            }
            
            if (name.IndexOfAny(InvalidParamChars) != -1)
            {
                // throw new ArgumentException("SR.GetString(SR.net_WebHeaderInvalidHeaderChars), name");
                throw new InvalidOperationException(
                    Microsoft.OData.Client.Strings.HttpWeb_InternalArgument("ValidationHelper.CheckBadChars.5", name));
            }

            if (ContainsNonAsciiChars(name))
            {
                // throw new ArgumentException("SR.GetString(SR.net_WebHeaderInvalidNonAsciiChars), name");
                throw new InvalidOperationException(
                    Microsoft.OData.Client.Strings.HttpWeb_InternalArgument("ValidationHelper.CheckBadChars.6", name));                
            }
            
            return name;
        }

        /// <summary>Checks whether the specified <paramref name="token"/> has any non-ASCII or control characters.</summary>
        /// <param name="token">Token to check.</param>
        /// <returns>true if <paramref name="token"/> has any non-ASCII characters or control characters; false otherwise.</returns>
        private static bool ContainsNonAsciiChars(string token)
        {
            Debug.Assert(token != null, "token != null");
            for (int i = 0; i < token.Length; i++)
            {
                if ((token[i] < ' ') || (token[i] > '~'))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
