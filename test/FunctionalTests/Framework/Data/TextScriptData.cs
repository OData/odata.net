//---------------------------------------------------------------------
// <copyright file="TextScriptData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Data
{
    /// <summary>
    /// Provides information about a specific text script.
    /// </summary>
    public sealed class TextScript: IStringIdentifierSupport
    {
        private char _firstChar;
        private char _lastChar;
        private string _name;
        private string _sample;
        private static TextScript[] _textScripts;

        /// <summary>Creates a new script object.</summary>
        private TextScript(string name, string sample): this(name, sample, '\0', '\0')
        {
        }

        /// <summary>Creates a new script object.</summary>
        private TextScript(string name, string sample, char firstChar, char lastChar)
        {
            this._name = name;
            this._sample = sample;
            this._firstChar = firstChar;
            this._lastChar = lastChar;
        }

        /// <summary>Arabic Unicode script.</summary>
        public static TextScript Arabic
        {
            get { return FindByName("Arabic"); }
        }

        /// <summary>Hebrew Unicode script.</summary>
        public static TextScript Hebrew
        {
            get { return FindByName("Hebrew"); }
        }

        /// <summary>Katakana Unicode script.</summary>
        public static TextScript Katakana
        {
            get { return FindByName("Katakana"); }
        }

        /// <summary>Latin Unicode script.</summary>
        public static TextScript Latin
        {
            get { return FindByName("Latin"); }
        }

        /// <summary>Thaana Unicode script.</summary>
        public static TextScript Thaana
        {
            get { return FindByName("Thaana"); }
        }

        /// <summary>User-friendly name of the script.</summary>
        public string Name { get { return this._name; } }

        /// <summary>Sample text for the script.</summary>
        public string Sample { get { return this._sample; } }

        /// <summary>Whether the script is supported by an ISCII character set.</summary>
        public bool SupportsIscii
        {
            get
            {
                string[] isciiScripts = new string[]
                {
                    "Assamese", "Bangla", "Devanagari", "Gujarati", "Gurmukhi", "Kannada", "Malayalam", "Odia", "Tamil", "Telugu"
                };
                return System.Linq.Enumerable.Contains(isciiScripts, this._name);
            }
        }

        /// <summary>Known scripts.</summary>
        public static TextScript[] Values
        {
            get
            {
                if (_textScripts == null)
                {
                    // TODO: complete ranges and come up with values
                    _textScripts = new TextScript[] {
                        new TextScript("Latin",                         "Hllo, wrld!"),
                        new TextScript("Greek",                         "\x0370\x0380 \x0390\x03A0"),
                        new TextScript("Cyrillic",                      "\x0400\x0410 \x0420\x04A0"),
                        new TextScript("Armenian",                      "\x0530\x0540 \x0550\x058F"),
                        new TextScript("Hebrew",                        "\x05d1\x05de\x05d4\x05dc\x05da \x05d4\x05d1\x05d9\x05e7\x05d5\x05e8 \x05d1\x05d0\x05e8\x05e5",
                                       '\x0590', '\x05FF'),
                        new TextScript("Arabic",                        "\x062e\x0635\x0645 40\x0025 \x0639\x0644\x0649 \x0625\x062c\x0645\x0627\x0644\x064a \x0627\x0644\x0633\x0639\x0631 \x0627\x0644\x0623\x0633\x0627\x0633\x064a \x0644\x0639\x062f\x062f 9 - 10 \x0645\x0637\x0628\x0648\x0639\x0627\x062a \x0645\x0639 \x0647\x062f\x064a\x0629 \x0641\x0648\x0631\x064a\x0629 \x0648\x0627\x0644\x062f\x062e\x0648\x0644 \x0628\x0627\x0644\x0633\x062d\x0628 \x0639\x0644\x0649 \x0627\x0644\x062c\x0627\x0626\x0632\x0629 \x0627\x0644\x0645\x0627\x0633\x064a\x0629 \x0648\x0627\x0644\x0630\x0647\x0628\x064a\x0629 \x0634\x0647\x0631\x064a\x0627",
                                       '\x0600', '\x06FF'),
                        new TextScript("Syriac",                        "\x0722\x0739\x072c\x0729\x0732\x0715\x0732\x072b \x072b\x0721\x0735\x071f \x0720\x0740",
                                       '\x0700', '\x074F'),
                        new TextScript("Thaana",                        "\x07aa\x0783\x07a6\x0791\x07aa\x0787\x07ae\x0795 \x07b0\x0782\x07a8\x0786\x07ad\x0784 \x07a7\x0790\x07aa\x0789\x07a6\x0790",
                                       '\x0780', '\x07BF'),
                        new TextScript("Devanagari",                    "\x0910\x0970 \x0950\x0940"),
                        new TextScript("Bangla",                       "\x0980\x0990 \x09A0\x09D0"),
                        new TextScript("Gurmukhi",                      "\x0A00\x0A60 \x0A70\x0A40"),
                        new TextScript("Gujarati",                      "\x0A80\x0A90 \x0AA0\x0AB0"),
                        new TextScript("Odia",                         "\x0B00\x0B60 \x0B70\x0B40"),
                        new TextScript("Tamil",                         "\x0B80\x0B90 \x0BA0\x0BB0"),
                        new TextScript("Telugu",                        "\x0c35\x0c38\x0c41\x0c1a\x0c30\x0c3f\x0c24\x0c4d\x0c30\x0c2e\x0c41",
                                       '\x0C00', '\x0C7F'),
                        new TextScript("Kannada",                       "\x0C80\x0C90 \x0CA0\x0CB0"),
                        new TextScript("Malayalam",                     "\x0D00\x0D60 \x0D70\x0D40"),
                        new TextScript("Sinhala",                       "\x0D80\x0D90 \x0DA0\x0DB0"),
                        new TextScript("Thai",                          "\x0E00\x0E60 \x0E70\x0E40"),
                        new TextScript("Lao",                           "\x0E80\x0E90 \x0EA0\x0EB0"),
                        new TextScript("Tibetan",                       "\x0F00\x0F60 \x0F70\x0F40"),
                        new TextScript("Myanmar",                       "\x1000\x1080 \x1070\x1071"),
                        new TextScript("Georgian",                      "\x10A0\x10A1 \x10A2\x10A3"),
                        new TextScript("Hangul Jamo",                   "\x1100\x1101 \x11F0\x11F1"),
                        new TextScript("Ethiopic",                      "\x1200\x1201 \x12F0\x12F1"),
                        new TextScript("Cherokee",                      "\x13A0\x13A1 \x13C0\x13C1"),
                        new TextScript("Canadian Aboriginal Syllabics", "\x1400\x1401 \x1670\x1671"),
                        new TextScript("Runic",                         "\x16A0\x16A1 \x16D0\x16D1"),
                        new TextScript("Ogham",                         "\x1680\x1681 \x1690\x1691"),
                        new TextScript("Khmer",                         "\x1780\x1781 \x1790\x1791"),
                        new TextScript("Mongolian",                     "\x1800\x1801 \x18A0\x18A1"),
                        // new TextScript("Extended Latin",                "Hllo, wrld!"),
                        // new TextScript("Extended Greek",                "Hllo, wrld!"),
                        new TextScript("Symbols",                       "\x20A0 \x2100 \x2150 \x2070 \x2200 \x2190 \x2400 \x2300 \x2440 \x2500 \x2580 \x25A0 \x2600 \x2700 \x2460 \x3200 \x3300 \x2800"),
                        //new TextScript("CJK Misc.",                     "Hllo, wrld!"),
                        //new TextScript("CJKV Ideographs",               "Hllo, wrld!"),
                        new TextScript("CJK Unified Ideographs",        "\x4e0b\x8f7d\x9632\x6bd2\x8f6f\x4ef6\x3001\x9632\x706b\x5899\x3001\x95f4\x8c0d\x8f6f\x4ef6\x5220\x9664\x5de5\x5177\x53ca\x5176\x5b83\x8f6f\x4ef6\xff0c\x63d0\x9ad8\x60a8\x7684\x8ba1\x7b97\x673a\x7684\x5b89\x5168\x6027\x5e76\x5e2e\x52a9\x4fdd\x6301\x5e73\x7a33\x8fd0\x884c\x3002", '\x4E00', '\x9FBB'),
                        new TextScript("Hiragana",                      "\x306e \x304c\x3064\x304b\x3046\x3001", '\x3040', '\x309F'),
                        new TextScript("Katakana",                      "\x30af\x30ea\x30a8\x30fc\x30bf \x30a4\x30f3\x30bf\x30d3\x30e5\x30fc", '\x30A0', '\x30FF'),
                        new TextScript("Bopomofo",                      "\x3100\x3101 \x3120\x3121"),
                        new TextScript("Yi",                            "\xA000\xA001 \xA400\xA401"),
                        //new TextScript("Hangul",                        "Hllo, wrld!"),
                        // new TextScript("Surrogates", '\xabab', '\xabab', "Hllo, wrld!"),
                        // new TextScript("Private Use", '\xabab', '\xabab', "Hllo, wrld!"),
                        // new TextScript("Compatibility", '\xabab', '\xabab', "Hllo, wrld!"),
                    };
                }
                return _textScripts;
            }
        }

        /// <summary>
        /// Returns the text script for the specified name.
        /// </summary>
        /// <param name='name'>Name to look for.</param>
        /// <returns>
        /// The TextScript with the given value, null if not found.
        /// </returns>
        public static TextScript FindByName(string name)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i].Name == name)
                {
                    return Values[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the script for the specified character, possibly null.
        /// </summary>
        /// <param name="c">Character to look for in scripts.</param>
        /// <returns>The script for the specified character, possibly null.</returns>
        public static TextScript GetCharacterScript(char c)
        {
            foreach(TextScript result in Values)
            {
                if (result.IsCharInScript(c))
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks whether the specified character is in this script.
        /// </summary>
        /// <param name="c">Character to check.</param>
        /// <returns>true if it's part of this script; false otherwise.</returns>
        public bool IsCharInScript(char c)
        {
            if (_firstChar <= c && c <= _lastChar)
            {
                return true;
            }
            return false;
        }

        /// <summary>Non-null string identifier for this instance.</summary>
        string IStringIdentifierSupport.StringIdentifier
        {
            get { return this.Name; }
        }
    }
}
