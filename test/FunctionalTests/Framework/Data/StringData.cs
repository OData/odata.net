//---------------------------------------------------------------------
// <copyright file="StringData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Data
{
    using System;
    using System.Text;

    /// <summary>
    /// Provides information about interesting string values.
    /// </summary>
    public sealed class StringData
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        /// <summary>Hide the constructor.</summary>
        private StringData() { }

        public static object[] ToObjectArray(StringData[] stringData)
        {
            object[] result = new object[stringData.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = stringData[i].Value;
            }

            return result;
        }

        /// <summary>Returns a string representation of the object.</summary>
        /// <returns>A string representation of the object</returns>
        public override string ToString()
        {
            return _name;
        }

        /// <summary>A StringData for empty text.</summary>
        public static StringData Empty
        {
            get
            {
                return ForValue("", "Empty");
            }
        }

        /// <summary>
        /// Whether the string is more interesting as a test value
        /// because of its large size rather than because of its value.
        /// </summary>
        public bool IsLong
        {
            get
            {
                // We use delegates to compose these long values.
                return _stringDelegate != null;
            }
        }

        /// <summary>
        /// Gets a StringData value that has a large amount of
        /// text in it.
        /// </summary>
        public static StringData LargeData
        {
            get
            {
                foreach(StringData data in Values)
                {
                    if (data._stringDelegate == GetLargeString)
                    {
                        return data;
                    }
                }
                throw new Exception("Unable to find large script instance.");
            }
        }

        /// <summary>
        /// Gets a StringData value that has a Latin sample text.
        /// </summary>
        public static StringData LatinScriptData
        {
            get
            {
                return ForScript(TextScript.Latin);
            }
        }

        /// <summary>
        /// Gets a StringData value that has mixed scripts in it.
        /// </summary>
        public static StringData MixedScripts
        {
            get
            {
                foreach(StringData data in Values)
                {
                    if (data._stringDelegate == GetMixedScript)
                    {
                        return data;
                    }
                }
                throw new Exception("Unable to find mixed script instance.");
            }
        }

        /// <summary>
        /// Unicode Script that this string exemplifies, possibly null.
        /// </summary>
        public TextScript TextScript
        {
            get { return this._textScript; }
        }

        /// <summary>Value for string data.</summary>
        public string Value
        {
            get
            {
                if (_stringDelegate != null)
                {
                    return _stringDelegate();
                }
                else
                {
                    return _value;
                }
            }
        }

        /// <summary>Interesting values for testing string values.</summary>
        public static StringData[] Values
        {
            get
            {
                if (_values == null)
                {
                    StringData[] baseValues;    // Values without scripts.

                    // NOTE: IsLong is based on whether a delegate is assigned.
                    // If the assumption is no longer true, modify the IsLong
                    // implementation.
                    // x0302 = combining circumflex accent
                    // x0327 = combining cedilla
                    // x0627 = arabic letter alef
                    // x0654 = arabic hamza above
                    // x0655 = arabic hamza below
                    // x05d0 = hebrew letter aleft
                    // x05d1 = hebrew letter bet
                    // x05ea = hebrew letter tav
                    // x05e9 = hebrew letter shin
                    // xd800 = [first available high-order surrogate code value]
                    // xdc00 = [first available low-order surrogate code value]

                    baseValues = new StringData[] {
                        ForValue(null, "Null"),
                        ForValue("", "Empty"),
                        ForValue("  \t \r\n", "Whitespace"),
                        ForValue(".,();", "Punctuation"),
                        ForValue("\r\n", "Crlf"),
                        ForValue("\r\n\r\n\r\n\r\n", "MultipleCrlf"),
                        ForValue("\r", "Cr"),
                        ForValue("\n", "Lf"),
                        ForValue("\n\r", "Lfcr"),
                        // TOOD: add back the large string values
                        // ForCallback(GetLargeString, "Large"),
                        // ForCallback(GetMixedScript, "MixedScripts"),
                        ForValue("a\x0302e\x0327\x0627\x0654\x0655", "Combining"),
                        ForValue("a surrogate pair: \xd800\xdc00", "Surrogate"),
                        ForValue("left to right \x05d0\x05d1 \x05ea\x05e9 english", "Bidi"),
                        ForValue("\x1\x2\x3\x4\x5\x20", "ControlCharacters"),
                    };

                    // Add Unicode scripts to the base values. These include
                    // left-to-right and right-to-left specific values.
                    _values = new StringData[baseValues.Length + TextScript.Values.Length];
                    baseValues.CopyTo(_values, 0);
                    for (int i = 0; i < TextScript.Values.Length; i++)
                    {
                        _values[baseValues.Length + i] = ForScript(TextScript.Values[i]);
                    }
                }
                return _values;
            }
        }

        /// <summary>
        /// Gets a StringData value which has a surrogatepair.
        /// </summary>
        public static StringData SurrogatePair
        {
            get
            {
                return ForValue("\xd800\xdc00", "SurrogatePair");
            }
        }

        /// <summary>
        /// Gets a StringData value which has combining characters.
        /// </summary>
        public static StringData CombiningCharacters
        {
            get
            {
                return ForValue("a\x0302e\x0327\x0627\x0654\x0655", "Combining");
            }
        }

        /// <summary>
        /// Creates a new StringData instance for the given value.
        /// </summary>
        private static StringData ForCallback(StringCallback callback, string name)
        {
            StringData result;

            result = new StringData();
            result._stringDelegate = callback;
            result._name = name;

            return result;
        }

        /// <summary>
        /// Creates a new StringData instance for the given script.
        /// </summary>
        private static StringData ForScript(TextScript textScript)
        {
            StringData result;

            result = new StringData();
            result._textScript = textScript;
            result._value = textScript.Sample;
            result._name = textScript.Name;

            return result;
        }

        /// <summary>
        /// Creates a new StringData instance for the given value.
        /// </summary>
        private static StringData ForValue(string stringValue, string name)
        {
            StringData result;

            result = new StringData();
            result._value = stringValue;
            result._name = name;

            return result;
        }

        /// <summary>
        /// Gets a string that is 10MB+.
        /// </summary>
        /// <returns>A string that is 10MB+.</returns>
        private static string GetLargeString()
        {
            const string phraseToRepeat = "Sample phrase to repeat. ";
            StringBuilder result;
            int phrasesWithoutBreak;

            result = new StringBuilder(1024 * 1024 * 12);
            phrasesWithoutBreak = 0;
            while (result.Length + phraseToRepeat.Length < result.Capacity)
            {
                result.Append(phraseToRepeat);
                phrasesWithoutBreak++;
                if (phrasesWithoutBreak > 10)
                {
                    result.Append("\r\n");
                    phrasesWithoutBreak = 0;
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Gets a string with mixed Unicode scripts.
        /// </summary>
        /// <returns>A string with mixed Unicode scripts..</returns>
        private static string GetMixedScript()
        {
            StringBuilder result;
            result = new StringBuilder(512);

            for (int i = 0; i < TextScript.Values.Length; i++)
            {
                result.Append(TextScript.Values[i].Sample);
            }
            return result.ToString();
        }

        /// <summary>Descriptive name for the StringData.</summary>
        private string _name;

        /// <summary>Delegate to provide value at run time.</summary>
        private StringCallback _stringDelegate;

        /// <summary>Unicode Script that this string exemplifies.</summary>
        private TextScript _textScript;

        /// <summary>Value for keyboard editing setting.</summary>
        private string _value;

        /// <summary>Interesting values for testing string values.</summary>
        private static StringData[] _values;

        /// <summary>
        /// Callback used to obtain a string value.
        /// </summary>
        /// <returns>A string value.</returns>
        private delegate string StringCallback();
    }
}
