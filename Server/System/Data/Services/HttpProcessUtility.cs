//Copyright 2010 Microsoft Corporation
//
//Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 
//
//http://www.apache.org/licenses/LICENSE-2.0 
//
//Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and limitations under the License.


#if ASTORIA_CLIENT
namespace System.Data.Services.Client
#else
namespace System.Data.Services
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    internal static class HttpProcessUtility
    {
        internal static readonly UTF8Encoding EncodingUtf8NoPreamble = new UTF8Encoding(false, true);

        internal static Encoding FallbackEncoding
        {
            get
            {
                return EncodingUtf8NoPreamble;
            }
        }

        private static Encoding MissingEncoding
        {
            get
            {
#if ASTORIA_LIGHT                
                return Encoding.UTF8;
#else
                return Encoding.GetEncoding("ISO-8859-1", new EncoderExceptionFallback(), new DecoderExceptionFallback());
#endif
            }
        }

#if !ASTORIA_CLIENT
        
        internal static string BuildContentType(string mime, Encoding encoding)
        {
            Debug.Assert(mime != null, "mime != null");
            if (encoding == null)
            {
                return mime;
            }
            else
            {
                return mime + ";charset=" + encoding.WebName;
            }
        }

        internal static string SelectMimeType(string acceptTypesText, string[] availableTypes)
        {
            Debug.Assert(availableTypes != null, "acceptableTypes != null");
            string selectedContentType = null;
            int selectedMatchingParts = -1;
            int selectedQualityValue = 0;
            int selectedPreferenceIndex = Int32.MaxValue;
            bool acceptable = false;
            bool acceptTypesEmpty = true;
            if (!String.IsNullOrEmpty(acceptTypesText))
            {
                IEnumerable<MediaType> acceptTypes = MimeTypesFromAcceptHeader(acceptTypesText);
                foreach (MediaType acceptType in acceptTypes)
                {
                    acceptTypesEmpty = false;
                    for (int i = 0; i < availableTypes.Length; i++)
                    {
                        string availableType = availableTypes[i];
                        int matchingParts = acceptType.GetMatchingParts(availableType);
                        if (matchingParts < 0)
                        {
                            continue;
                        }

                        if (matchingParts > selectedMatchingParts)
                        {
                            selectedContentType = availableType;
                            selectedMatchingParts = matchingParts;
                            selectedQualityValue = acceptType.SelectQualityValue();
                            selectedPreferenceIndex = i;
                            acceptable = selectedQualityValue != 0;
                        }
                        else if (matchingParts == selectedMatchingParts)
                        {
                            int candidateQualityValue = acceptType.SelectQualityValue();
                            if (candidateQualityValue > selectedQualityValue)
                            {
                                selectedContentType = availableType;
                                selectedQualityValue = candidateQualityValue;
                                selectedPreferenceIndex = i;
                                acceptable = selectedQualityValue != 0;
                            }
                            else if (candidateQualityValue == selectedQualityValue)
                            {
                                if (i < selectedPreferenceIndex)
                                {
                                    selectedContentType = availableType;
                                    selectedPreferenceIndex = i;
                                }
                            }
                        }
                    }
                }
            }

            if (acceptTypesEmpty)
            {
                selectedContentType = availableTypes[0];
            }
            else if (!acceptable)
            {
                selectedContentType = null;
            }

            return selectedContentType;
        }

        internal static string SelectRequiredMimeType(
            string acceptTypesText,
            string[] exactContentType,
            string inexactContentType)
        {
            Debug.Assert(exactContentType != null && exactContentType.Length != 0, "exactContentType != null && exactContentType.Length != 0");
            Debug.Assert(inexactContentType != null, "inexactContentType != null");

            string selectedContentType = null;
            int selectedMatchingParts = -1;
            int selectedQualityValue = 0;
            bool acceptable = false;
            bool acceptTypesEmpty = true;
            bool foundExactMatch = false;

            if (!String.IsNullOrEmpty(acceptTypesText))
            {
                IEnumerable<MediaType> acceptTypes = MimeTypesFromAcceptHeader(acceptTypesText);
                foreach (MediaType acceptType in acceptTypes)
                {
                    acceptTypesEmpty = false;
                    for (int i = 0; i < exactContentType.Length; i++)
                    {
                        if (WebUtil.CompareMimeType(acceptType.MimeType, exactContentType[i]))
                        {
                            selectedContentType = exactContentType[i];
                            selectedQualityValue = acceptType.SelectQualityValue();
                            acceptable = selectedQualityValue != 0;
                            foundExactMatch = true;
                            break;
                        }
                    }

                    if (foundExactMatch)
                    {
                        break;
                    }

                    int matchingParts = acceptType.GetMatchingParts(inexactContentType);
                    if (matchingParts < 0)
                    {
                        continue;
                    }

                    if (matchingParts > selectedMatchingParts)
                    {
                        selectedContentType = inexactContentType;
                        selectedMatchingParts = matchingParts;
                        selectedQualityValue = acceptType.SelectQualityValue();
                        acceptable = selectedQualityValue != 0;
                    }
                    else if (matchingParts == selectedMatchingParts)
                    {
                        int candidateQualityValue = acceptType.SelectQualityValue();
                        if (candidateQualityValue > selectedQualityValue)
                        {
                            selectedContentType = inexactContentType;
                            selectedQualityValue = candidateQualityValue;
                            acceptable = selectedQualityValue != 0;
                        }
                    }
                }
            }

            if (!acceptable && !acceptTypesEmpty)
            {
                throw Error.HttpHeaderFailure(415, Strings.DataServiceException_UnsupportedMediaType);
            }

            if (acceptTypesEmpty)
            {
                Debug.Assert(selectedContentType == null, "selectedContentType == null - otherwise accept types were not empty");
                selectedContentType = inexactContentType;
            }

            Debug.Assert(selectedContentType != null, "selectedContentType != null - otherwise no selection was made");
            return selectedContentType;
        }

        internal static Encoding EncodingFromAcceptCharset(string acceptCharset)
        {
            Encoding result = null;
            if (!string.IsNullOrEmpty(acceptCharset))
            {
                List<CharsetPart> parts = new List<CharsetPart>(AcceptCharsetParts(acceptCharset));
                parts.Sort(delegate(CharsetPart x, CharsetPart y)
                {
                    return y.Quality - x.Quality;
                });

                var encoderFallback = new EncoderExceptionFallback();
                var decoderFallback = new DecoderExceptionFallback();
                foreach (CharsetPart part in parts)
                {
                    if (part.Quality > 0)
                    {
                        if (String.Compare("utf-8", part.Charset, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            result = FallbackEncoding;
                            break;
                        }
                        else
                        {
                            try
                            {
                                result = Encoding.GetEncoding(part.Charset, encoderFallback, decoderFallback);
                                break;
                            }
                            catch (ArgumentException)
                            {
                            }
                        }
                    }
                }
            }

            if (result == null)
            {
                result = FallbackEncoding;
            }

            return result;
        }
#endif

        internal static KeyValuePair<string, string>[] ReadContentType(string contentType, out string mime, out Encoding encoding)
        {
            if (String.IsNullOrEmpty(contentType))
            {
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_ContentTypeMissing);
            }

            MediaType mediaType = ReadMediaType(contentType);
            mime = mediaType.MimeType;
            encoding = mediaType.SelectEncoding();
            return mediaType.Parameters;
        }

#if !ASTORIA_CLIENT
        internal static string GetParameterValue(KeyValuePair<string, string>[] parameters, string parameterName)
        {
            if (parameters == null)
            {
                return null;
            }

            foreach (KeyValuePair<string, string> parameterInfo in parameters)
            {
                if (parameterInfo.Key == parameterName)
                {
                    return parameterInfo.Value;
                }
            }

            return null;
        }

#endif

        internal static bool TryReadVersion(string text, out KeyValuePair<Version, string> result)
        {
            Debug.Assert(text != null, "text != null");

            int separator = text.IndexOf(';');
            string versionText, libraryName;
            if (separator >= 0)
            {
                versionText = text.Substring(0, separator);
                libraryName = text.Substring(separator + 1).Trim();
            }
            else
            {
                versionText = text;
                libraryName = null;
            }

            result = default(KeyValuePair<Version, string>);
            versionText = versionText.Trim();

            bool dotFound = false;
            for (int i = 0; i < versionText.Length; i++)
            {
                if (versionText[i] == '.')
                {
                    if (dotFound)
                    {
                        return false;
                    }

                    dotFound = true;
                }
                else if (versionText[i] < '0' || versionText[i] > '9')
                {
                    return false;
                }
            }

            try
            {
                result = new KeyValuePair<Version, string>(new Version(versionText), libraryName);
                return true;
            }
            catch (Exception e)
            {
                if (e is FormatException || e is OverflowException || e is ArgumentException)
                {
                    return false;
                }

                throw;
            }
        }

        private static Encoding EncodingFromName(string name)
        {
            if (name == null)
            {
                return MissingEncoding;
            }

            name = name.Trim();
            if (name.Length == 0)
            {
                return MissingEncoding;
            }
            else
            {
                try
                {
#if ASTORIA_LIGHT
                    return Encoding.UTF8;
#else
                    return Encoding.GetEncoding(name);
#endif
                }
                catch (ArgumentException)
                {
                    throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_EncodingNotSupported(name));
                }
            }
        }

#if !ASTORIA_CLIENT
        private static DataServiceException CreateParsingException(string message)
        {
            return Error.HttpHeaderFailure(400, message);
        }
#endif

        private static void ReadMediaTypeAndSubtype(string text, ref int textIndex, out string type, out string subType)
        {
            Debug.Assert(text != null, "text != null");
            int textStart = textIndex;
            if (ReadToken(text, ref textIndex))
            {
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_MediaTypeUnspecified);
            }

            if (text[textIndex] != '/')
            {
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_MediaTypeRequiresSlash);
            }

            type = text.Substring(textStart, textIndex - textStart);
            textIndex++;

            int subTypeStart = textIndex;
            ReadToken(text, ref textIndex);

            if (textIndex == subTypeStart)
            {
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_MediaTypeRequiresSubType);
            }

            subType = text.Substring(subTypeStart, textIndex - subTypeStart);
        }

        private static MediaType ReadMediaType(string text)
        {
            Debug.Assert(text != null, "text != null");

            string type;
            string subType;
            int textIndex = 0;
            ReadMediaTypeAndSubtype(text, ref textIndex, out type, out subType);

            KeyValuePair<string, string>[] parameters = null;
            while (!SkipWhitespace(text, ref textIndex))
            {
                if (text[textIndex] != ';')
                {
                    throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_MediaTypeRequiresSemicolonBeforeParameter);
                }

                textIndex++;
                if (SkipWhitespace(text, ref textIndex))
                {
                    break;
                }

                ReadMediaTypeParameter(text, ref textIndex, ref parameters);
            }

            return new MediaType(type, subType, parameters);
        }

        private static bool ReadToken(string text, ref int textIndex)
        {
            while (textIndex < text.Length && IsHttpToken(text[textIndex]))
            {
                textIndex++;
            }

            return (textIndex == text.Length);
        }

        private static bool SkipWhitespace(string text, ref int textIndex)
        {
            Debug.Assert(text != null, "text != null");
            Debug.Assert(text.Length >= 0, "text >= 0");
            Debug.Assert(textIndex <= text.Length, "text <= text.Length");

            while (textIndex < text.Length && Char.IsWhiteSpace(text, textIndex))
            {
                textIndex++;
            }

            return (textIndex == text.Length);
        }

#if !ASTORIA_CLIENT
        private static bool IsHttpElementSeparator(char c)
        {
            return c == ',' || c == ' ' || c == '\t';
        }

        private static bool ReadLiteral(string text, int textIndex, string literal)
        {
            if (String.Compare(text, textIndex, literal, 0, literal.Length, StringComparison.Ordinal) != 0)
            {
                throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
            }

            return textIndex + literal.Length == text.Length;
        }

        private static int DigitToInt32(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return (int)(c - '0');
            }
            else
            {
                if (IsHttpElementSeparator(c))
                {
                    return -1;
                }
                else
                {
                    throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
                }
            }
        }

        private static IEnumerable<MediaType> MimeTypesFromAcceptHeader(string text)
        {
            Debug.Assert(!String.IsNullOrEmpty(text), "!String.IsNullOrEmpty(text)");
            List<MediaType> mediaTypes = new List<MediaType>();
            int textIndex = 0;
            while (!SkipWhitespace(text, ref textIndex))
            {
                string type;
                string subType;
                ReadMediaTypeAndSubtype(text, ref textIndex, out type, out subType);

                KeyValuePair<string, string>[] parameters = null;
                while (!SkipWhitespace(text, ref textIndex))
                {
                    if (text[textIndex] == ',')
                    {
                        textIndex++;
                        break;
                    }

                    if (text[textIndex] != ';')
                    {
                        throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_MediaTypeRequiresSemicolonBeforeParameter);
                    }

                    textIndex++;
                    if (SkipWhitespace(text, ref textIndex))
                    {
                        break;
                    }

                    ReadMediaTypeParameter(text, ref textIndex, ref parameters);
                }

                mediaTypes.Add(new MediaType(type, subType, parameters));
            }

            return mediaTypes;
        }
#endif

        private static void ReadMediaTypeParameter(string text, ref int textIndex, ref KeyValuePair<string, string>[] parameters)
        {
            int startIndex = textIndex;
            if (ReadToken(text, ref textIndex))
            {
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_MediaTypeMissingValue);
            }

            string parameterName = text.Substring(startIndex, textIndex - startIndex);
            if (text[textIndex] != '=')
            {
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_MediaTypeMissingValue);
            }

            textIndex++;

            string parameterValue = ReadQuotedParameterValue(parameterName, text, ref textIndex);

            if (parameters == null)
            {
                parameters = new KeyValuePair<string, string>[1];
            }
            else
            {
                KeyValuePair<string, string>[] grow = new KeyValuePair<string, string>[parameters.Length + 1];
                Array.Copy(parameters, grow, parameters.Length);
                parameters = grow;
            }

            parameters[parameters.Length - 1] = new KeyValuePair<string, string>(parameterName, parameterValue);
        }

        private static string ReadQuotedParameterValue(string parameterName, string headerText, ref int textIndex)
        {
            StringBuilder parameterValue = new StringBuilder();
            
            bool valueIsQuoted = false;
            if (textIndex < headerText.Length)
            {
                if (headerText[textIndex] == '\"')
                {
                    textIndex++;
                    valueIsQuoted = true;
                }
            }

            while (textIndex < headerText.Length)
            {
                char currentChar = headerText[textIndex];

                if (currentChar == '\\' || currentChar == '\"')
                {
                    if (!valueIsQuoted)
                    {
                        throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_EscapeCharWithoutQuotes(parameterName));
                    }

                    textIndex++;

                    if (currentChar == '\"')
                    {
                        valueIsQuoted = false;
                        break;
                    }

                    if (textIndex >= headerText.Length)
                    {
                        throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_EscapeCharAtEnd(parameterName));
                    }

                    currentChar = headerText[textIndex];        
                }
                else
                if (!IsHttpToken(currentChar))
                {
                    break;
                }

                parameterValue.Append(currentChar);
                textIndex++;
            }

            if (valueIsQuoted)
            {
                throw Error.HttpHeaderFailure(400, Strings.HttpProcessUtility_ClosingQuoteNotFound(parameterName));
            }

            return parameterValue.ToString();
        }

#if !ASTORIA_CLIENT
        private static void ReadQualityValue(string text, ref int textIndex, out int qualityValue)
        {
            char digit = text[textIndex++];
            if (digit == '0')
            {
                qualityValue = 0;
            }
            else if (digit == '1')
            {
                qualityValue = 1;
            }
            else
            {
                throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
            }

            if (textIndex < text.Length && text[textIndex] == '.')
            {
                textIndex++;

                int adjustFactor = 1000;
                while (adjustFactor > 1 && textIndex < text.Length)
                {
                    char c = text[textIndex];
                    int charValue = DigitToInt32(c);
                    if (charValue >= 0)
                    {
                        textIndex++;
                        adjustFactor /= 10;
                        qualityValue *= 10;
                        qualityValue += charValue;
                    }
                    else
                    {
                        break;
                    }
                }

                qualityValue = qualityValue *= adjustFactor;
                if (qualityValue > 1000)
                {
                    throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
                }
            }
            else
            {
                qualityValue *= 1000;
            }
        }

        private static IEnumerable<CharsetPart> AcceptCharsetParts(string headerValue)
        {
            Debug.Assert(!String.IsNullOrEmpty(headerValue), "!String.IsNullOrEmpty(headerValuer)");

            bool commaRequired = false;            int headerIndex = 0;            int headerStart;            int headerNameEnd;            int headerEnd;            int qualityValue;
            while (headerIndex < headerValue.Length)
            {
                if (SkipWhitespace(headerValue, ref headerIndex))
                {
                    yield break;
                }

                if (headerValue[headerIndex] == ',')
                {
                    commaRequired = false;
                    headerIndex++;
                    continue;
                }

                if (commaRequired)
                {
                    throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
                }

                headerStart = headerIndex;
                headerNameEnd = headerStart;

                bool endReached = ReadToken(headerValue, ref headerNameEnd);
                if (headerNameEnd == headerIndex)
                {
                    throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
                }

                if (endReached)
                {
                    qualityValue = 1000;
                    headerEnd = headerNameEnd;
                }
                else
                {
                    char afterNameChar = headerValue[headerNameEnd];
                    if (IsHttpSeparator(afterNameChar))
                    {
                        if (afterNameChar == ';')
                        {
                            if (ReadLiteral(headerValue, headerNameEnd, ";q="))
                            {
                                throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
                            }

                            headerEnd = headerNameEnd + 3;
                            ReadQualityValue(headerValue, ref headerEnd, out qualityValue);
                        }
                        else
                        {
                            qualityValue = 1000;
                            headerEnd = headerNameEnd;
                        }
                    }
                    else
                    {
                        throw CreateParsingException(Strings.HttpContextServiceHost_MalformedHeaderValue);
                    }
                }

                yield return new CharsetPart(headerValue.Substring(headerStart, headerNameEnd - headerStart), qualityValue);

                commaRequired = true;
                headerIndex = headerEnd;
            }
        }
#endif

        private static bool IsHttpSeparator(char c)
        {
            return
                c == '(' || c == ')' || c == '<' || c == '>' || c == '@' ||
                c == ',' || c == ';' || c == ':' || c == '\\' || c == '"' ||
                c == '/' || c == '[' || c == ']' || c == '?' || c == '=' ||
                c == '{' || c == '}' || c == ' ' || c == '\x9';
        }

        private static bool IsHttpToken(char c)
        {
            return c < '\x7F' && c > '\x1F' && !IsHttpSeparator(c);
        }

#if !ASTORIA_CLIENT
        private struct CharsetPart
        {
            internal readonly string Charset;

            internal readonly int Quality;

            internal CharsetPart(string charset, int quality)
            {
                Debug.Assert(charset != null, "charset != null");
                Debug.Assert(charset.Length > 0, "charset.Length > 0");
                Debug.Assert(0 <= quality && quality <= 1000, "0 <= quality && quality <= 1000");

                this.Charset = charset;
                this.Quality = quality;
            }
        }
#endif

        [DebuggerDisplay("MediaType [{type}/{subType}]")]
        private sealed class MediaType
        {
            private readonly KeyValuePair<string, string>[] parameters;

            private readonly string subType;

            private readonly string type;

            internal MediaType(string type, string subType, KeyValuePair<string, string>[] parameters)
            {
                Debug.Assert(type != null, "type != null");
                Debug.Assert(subType != null, "subType != null");

                this.type = type;
                this.subType = subType;
                this.parameters = parameters;
            }

            internal string MimeType
            {
                get { return this.type + "/" + this.subType; }
            }

            internal KeyValuePair<string, string>[] Parameters
            {
                get { return this.parameters; }
            }

#if !ASTORIA_CLIENT
            internal int GetMatchingParts(string candidate)
            {
                Debug.Assert(candidate != null, "candidate must not be null.");

                int result = -1;
                if (candidate.Length > 0)
                {
                    if (this.type == "*")
                    {
                        result = 0;
                    }
                    else
                    {
                        int separatorIdx = candidate.IndexOf('/');
                        if (separatorIdx >= 0) 
                        {
                            string candidateType = candidate.Substring(0, separatorIdx);
                            if (WebUtil.CompareMimeType(this.type, candidateType))
                            {
                                if (this.subType == "*")
                                {
                                    result = 1;
                                }
                                else
                                {
                                    string candidateSubType = candidate.Substring(candidateType.Length + 1);
                                    if (WebUtil.CompareMimeType(this.subType, candidateSubType))
                                    {
                                        result = 2;
                                    }
                                }
                            }
                        }
                    }
                }

                return result;
            }

            internal int SelectQualityValue()
            {
                if (this.parameters != null)
                {
                    foreach (KeyValuePair<string, string> parameter in this.parameters)
                    {
                        if (String.Equals(parameter.Key, XmlConstants.HttpQValueParameter, StringComparison.OrdinalIgnoreCase))
                        {
                            string qvalueText = parameter.Value.Trim();
                            if (qvalueText.Length > 0)
                            {
                                int result;
                                int textIndex = 0;
                                ReadQualityValue(qvalueText, ref textIndex, out result);
                                return result;
                            }
                        }
                    }
                }

                return 1000;
            }
#endif

            internal Encoding SelectEncoding()
            {
                if (this.parameters != null)
                {
                    foreach (KeyValuePair<string, string> parameter in this.parameters)
                    {
                        if (String.Equals(parameter.Key, XmlConstants.HttpCharsetParameter, StringComparison.OrdinalIgnoreCase))
                        {
                            string encodingName = parameter.Value.Trim();
                            if (encodingName.Length > 0)
                            {
                                return EncodingFromName(parameter.Value);
                            }
                        }
                    }
                }

                if (String.Equals(this.type, XmlConstants.MimeTextType, StringComparison.OrdinalIgnoreCase))
                {
                    if (String.Equals(this.subType, XmlConstants.MimeXmlSubType, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                    else
                    {
                        return MissingEncoding;
                    }
                }
                else if (String.Equals(this.type, XmlConstants.MimeApplicationType, StringComparison.OrdinalIgnoreCase) &&
                    String.Equals(this.subType, XmlConstants.MimeJsonSubType, StringComparison.OrdinalIgnoreCase))
                {
                    return FallbackEncoding;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
