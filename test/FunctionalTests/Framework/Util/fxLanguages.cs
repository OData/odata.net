//---------------------------------------------------------------------
// <copyright file="fxLanguages.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Text;              //StringBuilder
using System.Globalization;
using System.Collections.Generic;     //CultureInfo

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////////////
    //fxLanguages
    //
    // Refer to ISO 3166-1 alpha-2 for abbreviation code information.
    ////////////////////////////////////////////////////////////////
    public class fxLanguages : fxList<fxLanguage>
    {
	    //Constructor
	    public fxLanguages()
	    {
            this.Add(new fxEnglishLanguage());
            this.Add(new fxGermanLanguage());
            this.Add(new fxItalianLanguage());
            this.Add(new fxSpanishLanguage());
            this.Add(new fxFrenchLanguage());
            this.Add(new fxDosLanguage());
            this.Add(new fxJapaneseLanguage());
            this.Add(new fxArabicLanguage());
            this.Add(new fxThaiLanguage());
            this.Add(new fxChineseLanguage());
            this.Add(new fxChineseTraditionalLanguage());
            this.Add(new fxRussianLanguage());
            this.Add(new fxGreekLanguage());
            this.Add(new fxCzechLanguage());
            this.Add(new fxTurkishLanguage());
            this.Add(new fxHebrewLanguage());
            this.Add(new fxKoreanLanguage());
            this.Add(new fxHindiLanguage());
            this.Add(new fxSurrogateCharacters());
            this.Add(new fxUnicodeOnlyCharacters());
            this.Add(new fxMongolianLanguage());
            this.Add(new fxUigurLanguage());
            this.Add(new fxYiLanguage());
            this.Add(new fxTibetanLanguage());
            this.Add(new fxAdditionalGB18030Characters());
	    }
    	
	    //Methods
	    public virtual fxLanguage		Default
	    {
		    get
            {
                fxLanguage lang = this.Find(CultureInfo.CurrentCulture).First;
    		    if(lang == null)
			        return this.First;
		        return lang;
            }
	    }

	    public virtual fxLanguages		Find(CultureInfo culture)
	    {
		    return this.Find(culture, true);
	    }
    	
	    public virtual fxLanguages		Find(CultureInfo culture, bool include)
	    {
	        fxLanguages found = new fxLanguages();
	        foreach(fxLanguage lang in this)
	        {
		        if((culture.Equals(lang.Culture)) == include)
			        found.Add(lang); 
	        }
	        return found;
	    }

	    public virtual fxLanguages		Find(String charset)
	    {
		    return this.Find(charset, true);     
	    }
    	
	    public virtual fxLanguages		Find(String charset, bool include)
	    {
	        fxLanguages found = new fxLanguages();
	        foreach(fxLanguage lang in this)
	        {
		        if((charset == lang.Name) == include)
			        found.Add(lang); 
	        }
	        return found;
	    }

        public virtual fxLanguages		Find(fxLanguage lang)
	    {
		    return this.Find(lang, true);
	    }
    	
	    public virtual fxLanguages		Find(fxLanguage lang, bool include)
	    {
	        fxLanguages found = new fxLanguages();
	        foreach(fxLanguage language in this)
	        {
		        if((language == lang) == include)
			        found.Add(lang); 
	        }
	        return found;
	    }

	    public virtual fxLanguages		Find(LanguageFlags flags)
	    {
            return this.Find(flags, true); 
        }
        
        public virtual fxLanguages		Find(LanguageFlags flags, bool include)
	    {
	        fxLanguages found = new fxLanguages();
	        foreach(fxLanguage lang in this)
	        {
		        if(lang.Flags.Is(flags) == include)
			        found.Add(lang); 
	        }
    		
	        return found;
	    }

	    public virtual fxLanguages		Ansi
	    {
		    get { return this.Find(LanguageFlags.Supported).Find(LanguageFlags.UnicodeOnly, false); }
	    }

	    public override fxLanguage		Choose()
	    {
		    //Explicit code-page specified
		    /*if(AstoriaTestProperties.CodePage != null)
		    {
                fxLanguage lang = this.Find(AstoriaTestProperties.CodePage).First;
			    if(lang == null)
                    throw new fxTestFailedException("Unable to find code page: " + AstoriaTestProperties.CodePage);
			    return lang;
		    }*/

		    //Otherwise, choose one at random
            return this[AstoriaTestProperties.Random.Next(this.Count)];
	    }
    	
	    public virtual String			CreateData(Random r, int length)
	    {
		    //Default culture, if not localized
		    //if(!fxTestProperties.LocalizedData)
			//    return this.Default.CreateData(r, length);

		    //Otherwise, choose a specific language
		    return this.Choose().CreateData(r, length);
	    }

	    public	virtual String			CreateUnicodeData(Random r, int length)
	    {
		    //Default culture, if not localized
		    //if(!fxTestProperties.LocalizedData)
			//    return this.Default.CreateUnicodeData(r, length);

		    //Calling this method, means any of the contained character sets are fair game to
		    //use for the data including 'mixed' sets.  This is generlaly only used for unicode 
		    //data.  Instead of always using mixed, which is difficult to look at and diagnose, we'll
		    //choose mixed sets occasionally (for coverage), but keep it somewhat simple by default.
		    int mixed = r.Next();
		    if(mixed % 10 == 0)
		    {
			    StringBuilder buffer = new StringBuilder(length);
			    while(buffer.Length < length) 
			    {
				    //First choose a language
				    fxLanguage lang = this.Choose();

				    //Then choose n characters of that set
				    int max = r.Next(length - buffer.Length + 1);
				    buffer.Append(lang.CreateUnicodeData(r, max));
			    }
		    }
    		
		    //Otherwise, choose a specific language
		    return this.Choose().CreateUnicodeData(r, length);
	    }
    }

    ////////////////////////////////////////////////////////////////
    // LanguageFlags
    //
    ////////////////////////////////////////////////////////////////
    public enum LanguageFlags
    {
	    Default     = 0x00000000,
	    UnicodeOnly	= 0x00000001,
	    Surrogates  = 0x00000010,
	    Supported   = 0x00000100,
    };

    ////////////////////////////////////////////////////////////////
    // fxLanguage
    //
    ////////////////////////////////////////////////////////////////
    public abstract class fxLanguage : fxBase
    {
	    //Data
        protected string[]              _riskStrings    = new string[0];
	    protected char[]			    _validchars		= new char[0];
	    protected char[]			    _uppercase		= new char[0];
	    protected char[]			    _lowercase		= new char[0];
        protected char[]                _sortingchars   = new char[0]; // sorting sensitive data
	    protected CultureInfo		    _culture		= null;
        protected fxBits<LanguageFlags>	_languageflags  = new fxBits<LanguageFlags>();
        protected string[]              _countryCodes   = new string[0];

        //Constructor
        public fxLanguage(CultureInfo culture, String charset)
            : base(charset)
	    {
		    _culture	= culture;

		    //Not all language packs are installed on every machine
		    _languageflags.Set(LanguageFlags.Supported, charset == null || IsSupported(charset));
	    }
    	
	    //Accessors
	    public virtual String			    Charset
	    {
		    get { return base.Name;         }
	    }

	    public virtual CultureInfo		    Culture
	    {
		    get { return _culture;          }
	    }

        public virtual fxBits<LanguageFlags> Flags
	    {
            get { return _languageflags;    }
        }

        public virtual bool 			    IsUnicodeOnly
	    {
		    get { return _languageflags.Is(LanguageFlags.UnicodeOnly);   }
	    }

        public virtual bool 			    IsSurrogates
	    {
		    get { return _languageflags.Is(LanguageFlags.Surrogates);   }
	    }

        public static bool                  IsSupported(String charset)
        {
            //TODO:
            return true;
        }

        public virtual 	char[]			    ValidChars
	    {
		    get { return _validchars;                                   }
	    }
    	
	    public	virtual char[]			    Uppercase
	    {
		    get { return _uppercase;                                    }
	    }

	    public	virtual char[]			    Lowercase
	    {
		    get { return _lowercase;                                    }
	    }

        public virtual char[]               SortingChars
        {
            get { return _sortingchars;                                 }
        }

        public virtual string[]             CountryCodes
	    {
		    get { return _countryCodes;                                   }
	    }

        public virtual string[]             RiskStrings
        {
            get { return _riskStrings; }
        }

        internal class CultureEqualityComparer : IEqualityComparer<string>
        {
            private readonly CultureInfo culture;

            public CultureEqualityComparer(CultureInfo culture)
            {
                System.Diagnostics.Debug.Assert(culture != null, "culture != null");
                this.culture = culture;
            }

            public bool Equals(string x, string y)
            {
                return 0 == String.Compare(x, y, true, this.culture);
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }

        public virtual string[] UniqueRiskStrings
        {
            get
            {
                // We need locale-specific comparison
                HashSet<string> result = new HashSet<string>(new CultureEqualityComparer(Culture));
                foreach (string s in RiskStrings)
                {
                    result.Add(s);
                }

                string[] arr = new string[result.Count];
                result.CopyTo(arr);
                return arr;
            }
        }

	    public virtual String		    CreateData(Random random, int length)
	    {
		    //Note: For non-unicode data, we have to store the data into MBCS
		    //instead of UCS-2, so multibyte characters take up two of those bytes.
		    int total = 0;
		    StringBuilder buffer = new StringBuilder(length);
            
            char[] validChars = _validchars;

		    for(int i=0; i<length; i++) 
		    {
			    //Choose a character
			    int index	= random.Next(validChars.Length);
			    char ch		= validChars[index];

			    //Count multi-byte character as two
			    total++;
			    if(IsMultibyte(ch))
				     total++;
			    if(total > length)
				    break;

			    buffer.Append(ch);
		    }

		    return buffer.ToString();
	    }

        public virtual bool IsMultibyte(char ch)
        {
            //if (Backend.Workspace.IsMBCS)
            //    return (int)ch > 0x7F;

            return (int)ch > 0xFF;
        }

	    public	virtual String		    CreateUnicodeData(Random random, int length)
	    {
		    StringBuilder buffer = new StringBuilder(length);
		    for(int i=0; i<length; i++) 
		    {
			    //Choose a character
			    int index	= random.Next(_validchars.Length);
			    buffer.Append(_validchars[index]);
		    }

		    return buffer.ToString();
	    }

        private static HashSet<string> knownMissingCultures = new HashSet<string>();

        public static CultureInfo GetSafeCultureInfo(string cultureName)
        {
            if (knownMissingCultures.Contains(cultureName))
            {
                return null;
            }

            try
            {
                return CultureInfo.GetCultureInfo(cultureName);
            }
            catch
            {
                knownMissingCultures.Add(cultureName);
                return null;
            }
        }
    }

    ////////////////////////////////////////////////////////////////
    //fxSurrogateCharacters
    //
    ////////////////////////////////////////////////////////////////
    public class fxSurrogateCharacters : fxLanguage
    {
	    //Constructor
	    public fxSurrogateCharacters()
		    : base(null, null)
	    {
		    //Collations
		    //None, pure unicode
		    _languageflags.Set(LanguageFlags.UnicodeOnly);
		    _languageflags.Set(LanguageFlags.Surrogates);
    		
		    //Killer Characters
		    _validchars = new char[] 
		     {
		    '\uD800', '\uDC00',
		    '\uD800', '\uDFFF',
		    '\uD840', '\uDC0B',		
		    '\uD98F', '\uDD83',		
		    '\uDA10', '\uDE00',
		    '\uDB0A', '\uDE83',
		    '\uDBFF', '\uDC00',
		    '\uDBFF', '\uDFFF',
		    };
	    }

	    public	override String		CreateData(Random random, int length)
	    {
		    //Note: We have to override this for surrogates, since they are pairs
		    StringBuilder buffer = new StringBuilder(length);
		    for(int i=0; i+1<length; i+=2) 
		    {
			    //Choose a character
			    int index		= random.Next(_validchars.Length);
			    int evenindex 	= (index - index%2);

			    buffer.Append(_validchars[evenindex]);
			    buffer.Append(_validchars[evenindex+1]);
		    }

		    return buffer.ToString();
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxUnicodeOnlyCharacters
    //
    ////////////////////////////////////////////////////////////////
    public class fxUnicodeOnlyCharacters : fxLanguage
    {
	    //Constructor
        public fxUnicodeOnlyCharacters()
            : base(null, null)
        {
            //Collations
            //None, pure unicode
            _languageflags.Set(LanguageFlags.UnicodeOnly);

            //Killer Characters
            _validchars = new char[] 
		     {
             //Latin
		    '\u00AA',		// a
            //'\u00A2', 		// �
            //'\u00A3', 		// �
            //'\u00A9',		// �
		    '\u00C3',		// �
		    '\u00C4',		// �
		    '\u00D0',		// �
		    '\u00D6',		// �
		    '\u00DC',		// �
		    '\u00DF',		// �
		    '\u00DF',		// �
		    '\u00E3',		// �
		    '\u00E4',		// �
		    '\u00E5',		// �
		    '\u00EE',		// �
		    '\u00F0',		// �
		    '\u00F6',		// �
		    '\u00FC',		// �
		    '\u00FD',		// �

            //Japanese
		    '\u30A1',	
		    '\u30AF',	
		    '\u30B0',	
		    '\u30BC',	
		    '\u30BC',	
		    '\u30BC',	
		    '\u30BD',	
		    '\u30BD',	
		    '\u30BD',	
		    '\u30BE',	
		    '\u30BE',	
		    '\u30BF',	
		    '\u30BF',	
		    '\u30C0',	
		    '\u30C1',	
		    '\u30C1',	
		    '\u30CF',	
		    '\u30D0',	
		    '\u30DC',	
		    '\u30DC',	
		    '\u30DD',	
		    '\u30DD',	

            //Korean
		    '\u3160',		//HANGUL LETTER YU
		    '\u316F',		//HANGUL LETTER MIEUM-SIOS
		    '\u3170',		//HANGUL LETTER MIEUM-PANSIOS
		    '\u317F',		//HANGUL LETTER PANSIOS
		    '\u3180',		//HANGUL LETTER SSANGIEUNG

            //Chinese
		    '\u4F83',	//CJK unified ideograph
		    '\u5000',	//CJK unified ideograph
		    '\u5001',	//CJK unified ideograph
		    '\u52A2',	//CJK unified ideograph
		    '\u55FF',	//CJK unified ideograph
		    '\u66C2',	//CJK unified ideograph
		    '\u6D10',	//CJK unified ideograph
		    '\u7187',	//CJK unified ideograph
		    '\u7868',	//CJK unified ideograph
		    '\u8233',	//CJK unified ideograph
		    '\u8864',	//CJK unified ideograph
		    '\u9A8C',	//CJK unified ideograph

            //Hebrew
		    '\u05D0',
		    '\u05D4',
		    '\u05DC',
		    '\u05E2',
		    '\u05E7',

            //Cyrillic
		    '\u0406',	//	Belarusian-Ukrainian I, not in OEM codepage
		    '\u0456',	
		    '\u0490',	//	Cyrillic Letter Ghe With Upturn, not in OEM codepage
		    '\u0491',	
		    '\u044f',	//	very last char in the codepage 
		    '\u0427',	//	in 1251, there are codepoints \xD7, \xF7, which are in 1252 � and �
		    '\u0447',	
		    '\u0401',	//	the same weight in sorting as Russian char E (\x0415, \x0435) 

            //Arabic
		    '\u0650',
		    '\u0686',
		    '\u0698',
		    '\u06AF',
		    '\u200C',	//zero-width nonjoiner
		    '\u200D',	//zero-width joiner
		    '\u200E',	//left-to-right mark (LRM)
		    '\u200F',	//right-to-left mark (RLM)

            //Turkish
		    '\u0049',		// capital dotless I
		    '\u0069',		// small case i
		    '\u00E2',		
		    '\u00EE',
		    '\u00F6',
		    '\u00FB',
		    '\u00FC',
		    '\u0130',		// capital I
		    '\u0131',		// small case dotless i

   		    '\u9DD7',
             };
        }
    }

    ////////////////////////////////////////////////////////////////
    //fxLatinLanguage
    //
    ////////////////////////////////////////////////////////////////
    public abstract class fxLatinLanguage : fxLanguage
    {
	    //Constructor
	    public fxLatinLanguage(CultureInfo culture)
		    : base(culture, "windows-1252")
	    {
		    //Collations
		    //TODO: OEM code page collations?
		    //_collations.Add(this, "Latin1_General_CI_AS", false/*case sensitive*/, true/*accent sensitive*/, this.Culture);
            //_collations.Add(this, "Latin1_General_CS_AS", true/*case sensitive*/, true/*accent sensitive*/, this.Culture);

		    //Killer Characters
		    _validchars = new char[] 
            {
		    'a',		
		    'A', 		
		    'b',		
		    'b',		
		    'B', 		
		    'C', 		
		    'h',		
		    'o',		
		    'O',		
		    'r',		
		    'u',		
		    'U',		
		    'v',		
		    'Z',		
		    'z',
		    '\u0020',		// <space>
    //		'\u0022',		// "
		    '\u002A',		// *
		    '\u002B',		// +
		    '\u002C',		// ,
		    '\u002E',		// .
		    '\u002F',		// /
		    '\u003A',		// :
		    '\u003C',		// < 
		    '\u003E',		// >
    //		'\u003F',		// ?
		    '\u0040',		// @
    //		'\u005B',		// [
    //		'\u005C',		// \
    //		'\u005D',		// ]
		    '\u005F',		// _
		    '\u007C',		// |
		    '\u007E',		// ~
		    '\u00AA',		// a
		    '\u00A2', 		// �
		    '\u00A3', 		// �
		    '\u00A9',		// �
		    '\u00C3',		// �
		    '\u00C4',		// �
		    '\u00D0',		// �
		    '\u00D6',		// �
		    '\u00DC',		// �
		    '\u00DF',		// �
		    '\u00DF',		// �
		    '\u00E3',		// �
		    '\u00E4',		// �
		    '\u00E5',		// �
		    '\u00EE',		// �
		    '\u00F0',		// �
		    '\u00F6',		// �
		    '\u00FC',		// �
		    '\u00FD'		// �
		    };

            _sortingchars = new char[] 
            {
                '\u0041',   //A - Latin Capital Letter A
                '\u0061',   //a - Latin Small Letter A
                '\u00C0',   //� - Latin Capital Letter A with Grave
                '\u00C1',   //� - Latin Capital Letter A with Acute
                '\u00C2',   //� - Latin Capital Letter A with Circumflex
                '\u00C3',   //� - Latin Capital Letter A with Tilde
                '\u00C4',   //� - Latin Capital Letter A with Diaeresis
                '\u00C5',   //� - Latin Capital Letter A with Ring Above
                '\u00E0',   //� - Latin Small Letter A with Grave
                '\u00E1',   //� - Latin Small Letter A with Acute
                '\u00E2',   //� - Latin Small Letter A with Circumflex
                '\u00E3',   //� - Latin Small Letter A with Tilde
                '\u00E4',   //� - Latin Small Letter A with Diaeresis
                '\u00E5',   //� - Latin Small Letter A with Ring Above
            };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxEnglishLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxEnglishLanguage : fxLatinLanguage
    {
	    public fxEnglishLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("en"))
	    {
            _countryCodes = new string[] 
            { 
                "AU",
                "BZ",
                "CA",
                "CB",
                "IE",
                "JM",
                "NZ",
                "PH",
                "ZA",
                "TT",
                "GB",
                "US",
                "ZW",
            };

            //These are the risk strings in English that may break the schema parser or query builder
            _riskStrings = new string[]
            {
                "_form0",
                "T",
                "T1",
                "Enity Container",
                "Entity Type",
                "Roles",
                "RelationshipMultiplicity",
                "UInt32",
                String.Empty,
                new String('a', Int16.MaxValue),
                "System",
                "System.Int",
                "Name",
                "Namespace",
                "Edm",
                "SqlFunction",
                " Test ",
                "Te st",
                "$#@%^&*~",
                "\\",
                "\'",
                "\t",
                "\b",
                "\"",
                ";SELECT @@language --",    //SQL injection
                "SELECT *",                 //SQL injection
                "; DROP TABLE myTable --",  //SQL injection
            };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxGermanLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxGermanLanguage : fxLatinLanguage
    {
	    public fxGermanLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("de"))
	    {
		    //Killer Characters (appending the inherited ones)
		    _validchars = (new String(_validchars) + new String(new char[] 
            {
		    '\u20AC',		// �
		    '\u2030',		// �
		    '\u2122'		// �
		    })).ToCharArray();

            _countryCodes = new string[] 
            { 
                "AT",
                "DE",
                "LI",
                "LU",
                "CH",
            };

            List<string> rs = new List<string>(_riskStrings);
            rs.Add("ss");
            rs.Add("\u00df");
            _riskStrings = rs.ToArray();
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxItalianLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxItalianLanguage : fxLatinLanguage
    {
	    public fxItalianLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("it"))
	    {
            _countryCodes = new string[] 
            { 
                "IT",
                "CH",
            };
        }
    }

    ////////////////////////////////////////////////////////////////
    //fxSpanishLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxSpanishLanguage : fxLatinLanguage
    {
	    public fxSpanishLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("es"))
	    {
            _countryCodes = new string[] 
            { 
                "AR",
                "BO",
                "CL",
                "CO",
                "CR",
                "DO",
                "EC",
                "SV",
                "GT",
                "HN",
                "MX",
                "NI",
                "PA",
                "PY",
                "PE",
                "PR",
                "ES",
                "UY",
                "VE",
            };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxFrenchLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxFrenchLanguage : fxLatinLanguage
    {
	    public fxFrenchLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("fr"))
	    {
            _countryCodes = new string[] 
            { 
                "BE",
                "CA",
                "FR",
                "LU",
                "MC",
                "CH",
            };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxDosLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxDosLanguage : fxLanguage
    {
	    //Constructor
	    public fxDosLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("en"), "cp850")
	    {
		    //CP850, is not supported by the Charset, but is actually implicitly supported by the VM (not advertised)
		    _languageflags.Set(LanguageFlags.Supported);

		    //Collations
		    //TODO: OEM code page collations?
            //_collations.Add(this, "SQL_Latin1_General_CP850_BIN", false/*case sensitive*/, false/*accent sensitive*/, base.Culture);
            //_collations.Add(this, "SQL_Latin1_General_CP850_CI_AS", false/*case sensitive*/, true/*accent sensitive*/, base.Culture);
            //_collations.Add(this, "SQL_Latin1_General_CP850_CS_AS", true/*case sensitive*/, true/*accent sensitive*/, base.Culture);

		    //Killer Characters
		    _validchars = new char[] 
            {
            // Test cases are not all prepared to handle control characters 
            // (which eg cause XML serialization to fail).
            //
            // Targeted tests will be required.
            //'\u0001',		// <control>

		    //0-128 same as ASCII
		    'a',		
		    'A', 		
		    'b',		
		    'b',		
		    'B', 		
		    'C', 		
		    'h',		
		    'o',		
		    'O',		
		    'r',		
		    'u',		
		    'U',		
		    'v',		
		    'Z',		
		    'z',
		    '\u0020',		// <space>

		    //128-255 Block characters
            
		    '\u00B6',		// PILCROW SIGN
		    '\u00B9',		// SUPERSCRIPT ONE
		    '\u256C',		// DRAWINGS DOUBLE VERTICAL AND HORIZONTAL 
		    '\u2580',		// BLACK SQUARE
		    '\u2591',		// LIGHT SHADE 
		    '\u2593',		// DARK SHADE 
		    };

            _countryCodes = new string[] 
            { 
                "AU",
                "BZ",
                "CA",
                "CB",
                "IE",
                "JM",
                "NZ",
                "PH",
                "ZA",
                "TT",
                "GB",
                "US",
                "ZW",
            };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxJapaneseLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxJapaneseLanguage : fxLanguage
    {
	    //Constructor
	    public fxJapaneseLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("ja"), "windows-932")
	    {
		    //Collations
            //_collations.Add(this, "Japanese_CI_AS", false/*case sensitive*/, true/*accent sensitive*/, base.Culture);
            //_collations.Add(this, "Japanese_CS_AS_KS_WS", true/*case sensitive*/, true/*accent sensitive*/, base.Culture);

		    //Killer Characters
		    _validchars = new char[] 
            {
		    '\u0041',	// US 'A'
		    '\u007A',	// US 'z'
		    '\u042F',	
		    '\u044F',	
		    '\u0451',	
		    '\u3041',	
		    '\u3042',	
		    '\u305B',	
		    '\u305C',	
		    '\u305D',	
		    '\u305E',	
		    '\u305F',	
		    '\u3072',	
		    '\u3073',	
		    '\u3074',	
		    '\u3079',	
		    '\u307A',	
		    '\u307B',	
		    '\u307C',	
		    '\u307D',	
		    '\u307E',	
		    '\u3092',	
		    '\u3093',	
		    '\u30A1',	
		    '\u30A1',	
		    '\u30AF',	
		    '\u30B0',	
		    '\u30BC',	
		    '\u30BC',	
		    '\u30BC',	
		    '\u30BD',	
		    '\u30BD',	
		    '\u30BD',	
		    '\u30BE',	
		    '\u30BE',	
		    '\u30BF',	
		    '\u30BF',	
		    '\u30C0',	
		    '\u30C1',	
		    '\u30C1',	
		    '\u30CF',	
		    '\u30D0',	
		    '\u30DC',	
		    '\u30DC',	
		    '\u30DD',	
		    '\u30DD',	
		    '\u30DE',	
		    '\u30DE',	
		    '\u30DF',	
		    '\u30DF',	
		    '\u4E5D',	
		    '\u4E9C',	
		    '\u531A',	
		    '\u5F0C',	
		    '\u66A6',	
		    '\u6B32',	
		    '\u6B79',	
		    '\u73F1',	
		    '\u755A',	
		    '\u7E37',	
		    '\u88F9',	
		    '\u9ED1',	
		    '\uFF41',	
		    '\uFF5A',	// Double-width 'z'
		    '\uFF66',	// single byte katakana
		    '\uFF88',	//single byte katakana, ne
		    '\uFF9D'	// single byte katakana
		    };

            _countryCodes = new string[] 
            { 
                "JP",
            };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxArabicLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxArabicLanguage : fxLanguage
    {
	    //Constructor
	    public fxArabicLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("ar"), "windows-1256")
	    {
    		
		    //Collations
            //_collations.Add(this, "Arabic_CI_AS", false/*case sensitive*/, true/*accent sensitive*/, base.Culture);
            //_collations.Add(this, "Arabic_CS_AS", true/*case sensitive*/, true/*accent sensitive*/, base.Culture);

		    //Killer Characters
		    _validchars = new char[] 
            {
		    '\u0637',
		    '\u0641',
		    '\u0647',
		    '\u0650',
		    '\u0686',
		    '\u0698',
		    '\u06AF',
		    '\u200C',	//zero-width nonjoiner
		    '\u200D',	//zero-width joiner
		    '\u200E',	//left-to-right mark (LRM)
		    '\u200F',	//right-to-left mark (RLM)
    		
    //TODO: These are not part of 1256 code page		
    //		'\u06DE',	
    //		'\u06E9',	
    //		'\u06FB',	
    //		'\u06FE',	
    //		'\uFB50',	
    //		'\uFB73',	
    //		'\uFBB1',	
    //		'\uFD3E',	
    //		'\uFEBF',	
    //		'\uFEED',	
    //		'\uFEFC',	
		    };

            _sortingchars = new char[] 
            {
                '\u0649',
                '\u064A',
                '\u06CC',
                '\u06CD',
                '\u06CE',
                '\u0626',
                '\u0624',
                '\u0626',
                '\u0624',
            };

            _countryCodes = new string[] 
            { 
                "DZ",
                "BH",
                "EG",
                "IQ",
                "JO",
                "KW",
                "LB",
                "LY",
                "MA",
                "OM",
                "QA",
                "SA",
                "SY",
                "TN",
                "AE",
                "YE",
            };
	    }
    }
    ////////////////////////////////////////////////////////////////
    //fxThaiLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxThaiLanguage : fxLanguage
    {
	    //Constructor
	    public fxThaiLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("th"), "TIS-620")
	    {
		    //Collations
            //_collations.Add(this, "Thai_CI_AS", false/*case sensitive*/, true/*accent sensitive*/, base.Culture);
            //_collations.Add(this, "Thai_CS_AS", true/*case sensitive*/, true/*accent sensitive*/, base.Culture);

		    //Killer Characters
		    _validchars = new char[] 
            {
		    '\u0E01',
		    '\u0E0A',
		    '\u0E0F',
		    '\u0E10',
		    '\u0E16',
		    '\u0E1F',
		    '\u0E20',
		    '\u0E2D',
		    '\u0E2F',
		    '\u0E30',
		    '\u0E3F',	
		    '\u0E40',
		    '\u0E49',
		    '\u0E4A',
		    '\u0E4F',
		    '\u0E50',
		    '\u0E5B',
		    };

            _countryCodes = new string[] 
            { 
                "TH",
            };
	    }
    }


    ////////////////////////////////////////////////////////////////
    //fxChineseLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxChineseLanguage : fxLanguage
    {
	    //Constructor
	    public fxChineseLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("zh-CHS"), "windows-936")
	    {
    		//Collations
            //_collations.Add(this, "Chinese_PRC_CI_AS", false/*case sensitive*/, true/*accent sensitive*/, base.Culture);
            //_collations.Add(this, "Chinese_PRC_CS_AS", true/*case sensitive*/, true/*accent sensitive*/, base.Culture);

		    //Killer Characters
		    _validchars = new char[] 
            {
		    //GB18030 characters
		    '\u0041',	//Capital letter A
		    '\u007A',	//Small letter z
		    '\u0414',	//Cyrillic captital letter De			
		    '\u4E00',	//CJK unified ideograph
		    '\u4F83',	//CJK unified ideograph
		    '\u5000',	//CJK unified ideograph
		    '\u5001',	//CJK unified ideograph
		    '\u52A2',	//CJK unified ideograph
		    '\u55FF',	//CJK unified ideograph
		    '\u66C2',	//CJK unified ideograph
		    '\u6D10',	//CJK unified ideograph
		    '\u7187',	//CJK unified ideograph
		    '\u7868',	//CJK unified ideograph
		    '\u8233',	//CJK unified ideograph
		    '\u8864',	//CJK unified ideograph
		    '\u9A8C',	//CJK unified ideograph
		    '\u9FA5',	//CJK unified ideograph
		    '\u93FB',	//E76C = U+93FB : CJK UNIFIED IDEOGRAPH
		    '\uE864',	//private use
		    '\uF92C',	//CJK compatibility ideograph-F92C
		    '\uFA29',	//CJK compatibility ideograph-FA29

    //TODO: These are not part of the 936 code page
    //		'\uA000',	//Yi syllable It
    //		'\uA4C6',
    //		'\uFB56',
    //		'\uFBFF',
    //		'\u00C7',
    //		'\u00F6',
    //		'\u00FE',
    //		'\u0626',
    //		'\u0F03',
    //		'\u0F31',
    //		'\u0FC4',
    //		'\u1800',
    //		'\u18A9',
    //		'\u3400',
    //		'\u35DF',
    //		'\u3F2C',
    //		'\u4390',
    //		'\u4A01',
    //		'\u4DB5',
		    };

            _sortingchars = new char[] 
            {
                '\u0030', //Digit Zero
                '\uFF10', //Fullwidth Digit Zero
                '\u0041', //Latin Capital Letter A
                '\uFF21', //Fullwidth Latin Capital Letter A
                '\u0061', //Latin Small Letter A
                '\uFF41', //Fullwidth Latin Small Letter A
                '\u4E00', //CJK Unified Ideograph 
                '\u4E2D', //CJK Unified Ideograph
                '\u4EBA', //CJK Unified Ideograph 
                '\u5730', //CJK Unified Ideograph 
                '\u6C49', //CJK Unified Ideograph
            };

            _countryCodes = new string[] 
            { 
                "CN",
                "SG",
            };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxChineseTraditionalLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxChineseTraditionalLanguage : fxLanguage
    {
	    //Constructor
	    public fxChineseTraditionalLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("zh-CHT"), "windows-950")
	    {
		    _validchars = new char[] 
            {
		    '\uE7C7',
		    '\uE7C8',
		    '\uE7E7',
		    '\uE7F3',
		    '\uE815',
		    '\uE864',
		    '\u3000',
		    '\uFF0C',
		    '\u3001',
		    '\uFF0E',
		    '\uFF1B',
		    '\uFF01',
		    '\uFE30',
		    '\u2026',
		    '\uFE50',
		    '\u32A3',
		    '\u5159',
		    '\u3105',
		    '\u2554',
		    '\u2593',
		    '\u3127',
		    '\u4EA2',
		    '\u5DC3',
		    '\u66E3',
		    '\u7C57',
		    '\u8C45',
		    '\u9F98',
		    '\u5AFA',
    		
    //TODO: These are not part of the 950 code page
    //		'\u9289',
		    };

            _countryCodes = new string[] 
            { 
                "HK",
                "MO",
                "TW",
            };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxCyrillicLanguage
    //
    ////////////////////////////////////////////////////////////////
    public abstract class fxCyrillicLanguage : fxLanguage
    {
	    //Constructor
	    public fxCyrillicLanguage(CultureInfo culture)
		    : base(culture, "windows-1251")
	    {
		    //Collations
            //_collations.Add(this, "Cyrillic_General_CI_AS", false/*case sensitive*/, true/*accent sensitive*/, base.Culture);
            //_collations.Add(this, "Cyrillic_General_CS_AS", true/*case sensitive*/, true/*accent sensitive*/, base.Culture);

		    //Killer Characters
		    _validchars = new char[] 
            {
		    '\u0406',	//	Belarusian-Ukrainian I, not in OEM codepage
		    '\u0456',	
		    '\u0490',	//	Cyrillic Letter Ghe With Upturn, not in OEM codepage
		    '\u0491',	
		    '\u044f',	//	very last char in the codepage 
		    '\u0427',	//	in 1251, there are codepoints \xD7, \xF7, which are in 1252 � and �
		    '\u0447',	
		    '\u0401',	//	the same weight in sorting as Russian char E (\x0415, \x0435) 
		    '\u0451',	
		    '\u041A',	
		    '\u042A',	
		    '\u0409',	
		    '\u0459',	

    //TODO: These are not part of the 1251 code page
    //		'\u04E9',	
		    };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxRussianLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxRussianLanguage : fxCyrillicLanguage
    {
	    public fxRussianLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("ru"))
	    {
            _countryCodes = new string[] 
            { 
                "RU",
            };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxGreekLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxGreekLanguage : fxLanguage
    {
	    //Constructor
	    public fxGreekLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("el"), "windows-1253")
	    {
		    //Collations
            //_collations.Add(this, "Greek_CI_AS", false/*case sensitive*/, true/*accent sensitive*/, base.Culture);
            //_collations.Add(this, "Greek_CS_AS", true/*case sensitive*/, true/*accent sensitive*/, base.Culture);

		    //Killer Characters
		    _validchars = new char[] 
            {
		    '\u00B0',
		    '\u00A7',
		    '\u2030',
		    '\u2020',
		    '\u2021',
		    '\u00B6',
		    '\u0391',
		    '\u0392',
		    '\u0393',
		    '\u0394',
		    '\u0395',
		    '\u0396',
		    '\u0397',
		    '\u0398',
		    '\u0399',
		    '\u039A',
		    '\u039B',
		    '\u039C',
		    '\u039D',
		    '\u039E',
		    '\u039F',
		    '\u03A0',
		    '\u03A1',
		    '\u03A3',
		    '\u03A4',
		    '\u03A5',
		    '\u03A6',
		    '\u03A7',
		    '\u03A8',
		    '\u03A9',
		    '\u03B1',
		    '\u03B2',
		    '\u03B3',
		    '\u03B4',
		    '\u03B5',
		    '\u03B6',
		    '\u03B7',
		    '\u03B8',
		    '\u03B9',
		    '\u03BA',
		    '\u03BB',
		    '\u03BC',
		    '\u03BD',
		    '\u03BE',
		    '\u03BF',
		    '\u03C0',
		    '\u03C1',
		    '\u03C3',
		    '\u03C4',
		    '\u03C5',
		    '\u03C6',
		    '\u03C7',
		    '\u03C8',
		    '\u03C9',
    		
    //TODO: These are not part of the 1253 code page
    //		'\u00FF',
		    };

            _countryCodes = new string[] 
            { 
                "GR",
            };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxEuropeanLanguage
    //
    ////////////////////////////////////////////////////////////////
    public abstract class fxEuropeanLanguage : fxLanguage
    {
	    //Constructor
	    public fxEuropeanLanguage(CultureInfo culture)
		    : base(culture, "windows-1250")
	    {
		    //Killer Characters
		    _validchars = new char[] 
            {
		    '\u0105',
		    '\u0106',
		    '\u010D',
		    '\u010F',
		    '\u0118',
		    '\u011A',
		    '\u013D',
		    '\u013E',
		    '\u0141',
		    '\u0142',
		    '\u0155',
		    '\u0159',
		    '\u0162',
		    '\u0163',
		    '\u0165',
		    '\u017E',

    //TODO: These are not part of the 1250 code page
    //		'\u0100',
    //		'\u0112',
    //		'\u0116',
    //		'\u012A',
    //		'\u012B',
    //		'\u012E',
    //		'\u012F',
    //		'\u0136',
    //		'\u0137',
    //		'\u013B',
    //		'\u013C',
    //		'\u0152',
    //		'\u0153',
    //		'\u0157',
		    };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxCzechLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxCzechLanguage : fxEuropeanLanguage
    {
	    //Constructor
	    public fxCzechLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("cs"))
	    {
		    //Collations
            //_collations.Add(this, "Czech_CS_AS", false/*case sensitive*/, true/*accent sensitive*/, base.Culture);
            //_collations.Add(this, "Czech_CI_AS", true/*case sensitive*/, true/*accent sensitive*/, base.Culture);

            _countryCodes = new string[] 
            { 
                "CZ",
            };
	    }
    }


    ////////////////////////////////////////////////////////////////
    //fxTurkishLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxTurkishLanguage : fxLanguage
    {
	    //Constructor
	    public fxTurkishLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("tr"), "windows-1254")
	    {
		    //Collations
            //_collations.Add(this, "Turkish_CI_AS", false/*case sensitive*/, true/*accent sensitive*/, base.Culture);
            //_collations.Add(this, "Turkish_CS_AS", true/*case sensitive*/, true/*accent sensitive*/, base.Culture);

		    //Killer Characters
		    _validchars = new char[] 
            {
		    '\u00E7',
		    '\u0049',		// capital dotless I
		    '\u0069',		// small case i
		    '\u00E2',		
		    '\u00EE',
		    '\u00F6',
		    '\u00FB',
		    '\u00FC',
		    '\u0130',		// capital I
		    '\u0131',		// small case dotless i
		    '\u015F',
		    };
    		
		    _lowercase	= new char[]
 		    {
		    '\u0131',		// lowercase dotless i
		    '\u0069',		// lowercase i
 		    };

		    _uppercase	= new char[]
		    {
		    '\u0049',		// uppercase dotless I
		    '\u0130',		// uppercase I
		    };

            _sortingchars = new char[] 
            {
                '\u0049', //Latin Capital Letter I
                '\u0069', //Latin Small Letter I
                '\u0130', //Latin Capital Letter I with Dot Above
                '\u0131', //Latin Small Letter Dotless I
            };

            _countryCodes = new string[] 
            { 
                "TR",
            };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxHebrewLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxHebrewLanguage : fxLanguage
    {
	    //Constructor
	    public fxHebrewLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("he"), "windows-1255")
	    {
		    //Collations
            //_collations.Add(this, "Hebrew_CI_AS", false/*case sensitive*/, true/*accent sensitive*/, base.Culture);
            //_collations.Add(this, "Hebrew_CS_AS", true/*case sensitive*/, true/*accent sensitive*/, base.Culture);

		    //Killer Characters
		    _validchars = new char[] 
            {
		    '\u05B0',
		    '\u05D0',
		    '\u05D4',
		    '\u05DC',
		    '\u05E2',
		    '\u05E7',
		    '\u05F0',
		    '\u05F4',
    //		'\uFB1D',	//Unicode only
    //		'\uFB41',	//Unicode only
    //		'\uFB49',	//Unicode only
    //		'\uFB4F',	//Unicode only
		    };

            _countryCodes = new string[] 
            { 
                "IL",
            };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxKoreanLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxKoreanLanguage : fxLanguage
    {
	    //Constructor
	    public fxKoreanLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("ko"), "x-windows-949")
	    {
		    //Collations
            //_collations.Add(this, "Korean_Wansung_CI_AS", false/*case sensitive*/, true/*accent sensitive*/, base.Culture);
            //_collations.Add(this, "Korean_Wansung_CS_AS", true/*case sensitive*/, true/*accent sensitive*/, base.Culture);

		    //Killer Characters
		    _validchars = new char[] 
            {
		    '\u3131',		//HANGUL LETTER KIYEOK
		    '\u313F',		//HANGUL LETTER RIEUL-PHIEUPH
		    '\u3140',		//HANGUL LETTER RIEUL-HIEUH
		    '\u314F',		//HANGUL LETTER A
		    '\u3150',		//HANGUL LETTER AE
		    '\u315F',		//HANGUL LETTER WI
		    '\u3160',		//HANGUL LETTER YU
		    '\u316F',		//HANGUL LETTER MIEUM-SIOS
		    '\u3170',		//HANGUL LETTER MIEUM-PANSIOS
		    '\u317F',		//HANGUL LETTER PANSIOS
		    '\u3180',		//HANGUL LETTER SSANGIEUNG
		    '\u318E',		//HANGUL LETTER ARAEAE
		    };

            _countryCodes = new string[] 
            { 
                "KR",
            };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxHindiLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxHindiLanguage : fxLanguage
    {
	    //Constructor
	    public fxHindiLanguage()
		    : base(fxLanguage.GetSafeCultureInfo("hi"), null/*unicode only*/)
	    {
		    //Collations
		    //None: Pure unicode
		    _languageflags.Set(LanguageFlags.UnicodeOnly);

		    //Killer Characters
		    _validchars = new char[] 
            {
		    '\u0901',	
		    '\u090F',	
		    '\u0910',	
		    '\u091F',	
		    '\u092F',	
		    '\u093F',	
		    '\u094D',	
		    '\u0950',	
		    '\u095F',	
		    '\u096F',	
		    '\u0970',	
		    '\u200C',		//Zero Width Non-Joiner
		    '\u200D',		//Zero Width Joiner
		    '\u2013',		//En Dash
		    '\u2014',		//Emn Dash
		    '\u2018',		//Left Single Quotation Mark
		    '\u2019',		//Right Single Quotation Mark
		    '\u2026',		//Horizontal Ellipsis
		    '\u2212',		//Minus Sign
		    '\u25CC',		//Dotted Circle
		    };

            _countryCodes = new string[] 
            { 
                "IN",
            };
	    }
    }

    ////////////////////////////////////////////////////////////////
    //fxMongolianLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxMongolianLanguage : fxLanguage
    {
	    //Constructor
        public fxMongolianLanguage()
            : base(fxLanguage.GetSafeCultureInfo("mn-Mong-CN"), null)
        {
            //Collations
            //None, pure unicode
            _languageflags.Set(LanguageFlags.UnicodeOnly);

            //Killer Characters
            _validchars = new char[] 
            {

                '\x17f8',
                '\x17f9',
                '\x17fa',
                '\x17fb',
                '\x17fc',
                '\x17fd',
                '\x17fe',
                '\x17ff',
                '\x1800',
                '\x1801',
                '\x1802',
                '\x1803',
                '\x1804',
                '\x1805',
                '\x1806',
                '\x1807',
                '\x1808',
                '\x1809',
                '\x180a',
                '\x180b',
                '\x180c',
                '\x180d',
                '\x180e',
                '\x180f',
                '\x1810',
                '\x1811',
                '\x1812',
                '\x1813',
                '\x1814',
                '\x1815',
                '\x1816',
                '\x1817',
                '\x1818',
                '\x1819',
                '\x181a',
                '\x181b',
                '\x181c',
                '\x181d',
                '\x181e',
                '\x181f',
                '\x1820',
                '\x1821',
                '\x1822',
                '\x1823',
                '\x1824',
                '\x1825',
                '\x1826',
                '\x1827',
                '\x1828',
                '\x1829',
                '\x182a',
                '\x182b',
                '\x182c',
                '\x182d',
                '\x182e',
                '\x182f',
                '\x1830',
                '\x1831',
                '\x1832',
                '\x1833',
                '\x1834',
                '\x1835',
                '\x1836',
                '\x1837',
                '\x1838',
                '\x1839',
                '\x183a',
                '\x183b',
                '\x183c',
                '\x183d',
                '\x183e',
                '\x183f',
                '\x1840',
                '\x1841',
                '\x1842',
                '\x1843',
                '\x1844',
                '\x1845',
                '\x1846',
                '\x1847',
                '\x1848',
                '\x1849',
                '\x184a',
                '\x184b',
                '\x184c',
                '\x184d',
                '\x184e',
                '\x184f',
                '\x1850',
                '\x1851',
                '\x1852',
                '\x1853',
                '\x1854',
                '\x1855',
                '\x1856',
                '\x1857',
                '\x1858',
                '\x1859',
                '\x185a',
                '\x185b',
                '\x185c',
                '\x185d',
                '\x185e',
                '\x185f',
                '\x1860',
                '\x1861',
                '\x1862',
                '\x1863',
                '\x1864',
                '\x1865',
                '\x1866',
                '\x1867',
                '\x1868',
                '\x1869',
                '\x186a',
                '\x186b',
                '\x186c',
                '\x186d',
                '\x186e',
                '\x186f',
                '\x1870',
                '\x1871',
                '\x1872',
                '\x1873',
                '\x1874',
                '\x1875',
                '\x1876',
                '\x1877',
                '\x1878',
                '\x1879',
                '\x187a',
                '\x187b',
                '\x187c',
                '\x187d',
                '\x187e',
                '\x187f',
                '\x1880',
                '\x1881',
                '\x1882',
                '\x1883',
                '\x1884',
                '\x1885',
                '\x1886',
                '\x1887',
                '\x1888',
                '\x1889',
                '\x188a',
                '\x188b',
                '\x188c',
                '\x188d',
                '\x188e',
                '\x188f',
                '\x1890',
                '\x1891',
                '\x1892',
                '\x1893',
                '\x1894',
                '\x1895',
                '\x1896',
                '\x1897',
                '\x1898',
                '\x1899',
                '\x189a',
                '\x189b',
                '\x189c',
                '\x189d',
                '\x189e',
                '\x189f',
                '\x18a0',
                '\x18a1',
                '\x18a2',
                '\x18a3',
                '\x18a4',
                '\x18a5',
                '\x18a6',
                '\x18a7',
                '\x18a8',
                '\x18a9',
                '\x18aa',
                '\x18ab',

            };

            _riskStrings = new string[] 
            {
                "\x17f8\x17f9\x17fa\x17fb\x17fc\x17fd\x17fe\x17ff\x1800\x1801",
                "\x1802\x1803\x1804\x1805\x1806\x1807\x1808\x1809\x180a\x180b",
                "\x180c\x180d\x180e\x180f\x1810\x1811\x1812\x1813\x1814\x1815",
                "\x1816\x1817\x1818\x1819\x181a\x181b\x181c\x181d\x181e\x181f",
                "\x1820\x1821\x1822\x1823\x1824\x1825\x1826\x1827\x1828\x1829",
                "\x182a\x182b\x182c\x182d\x182e\x182f\x1830\x1831\x1832\x1833",
                "\x1834\x1835\x1836\x1837\x1838\x1839\x183a\x183b\x183c\x183d",
                "\x183e\x183f\x1840\x1841\x1842\x1843\x1844\x1845\x1846\x1847",
                "\x1848\x1849\x184a\x184b\x184c\x184d\x184e\x184f\x1850\x1851",
                "\x1852\x1853\x1854\x1855\x1856\x1857\x1858\x1859\x185a\x185b",
                "\x185c\x185d\x185e\x185f\x1860\x1861\x1862\x1863\x1864\x1865",
                "\x1866\x1867\x1868\x1869\x186a\x186b\x186c\x186d\x186e\x186f",
                "\x1870\x1871\x1872\x1873\x1874\x1875\x1876\x1877\x1878\x1879",
                "\x187a\x187b\x187c\x187d\x187e\x187f\x1880\x1881\x1882\x1883",
                "\x1884\x1885\x1886\x1887\x1888\x1889\x188a\x188b\x188c\x188d",
                "\x188e\x188f\x1890\x1891\x1892\x1893\x1894\x1895\x1896\x1897",
                "\x1898\x1899\x189a\x189b\x189c\x189d\x189e\x189f\x18a0\x18a1",
                "\x18a2\x18a3\x18a4\x18a5\x18a6\x18a7\x18a8\x18a9\x18aa\x18ab",
            };
        }
    }

    ////////////////////////////////////////////////////////////////
    //fxTibetanLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxTibetanLanguage : fxLanguage
    {
	    //Constructor
        public fxTibetanLanguage()
            : base(fxLanguage.GetSafeCultureInfo("bo-CN"), null)
        {
            //Collations
            //None, pure unicode
            _languageflags.Set(LanguageFlags.UnicodeOnly);

            //Killer Characters
            _validchars = new char[] 
            {

                '\xefc',
                '\xefd',
                '\xefe',
                '\xeff',
                '\xf00',
                '\xf01',
                '\xf02',
                '\xf03',
                '\xf04',
                '\xf05',
                '\xf06',
                '\xf07',
                '\xf08',
                '\xf09',
                '\xf0a',
                '\xf0b',
                '\xf0c',
                '\xf0d',
                '\xf0e',
                '\xf0f',
                '\xf10',
                '\xf11',
                '\xf12',
                '\xf13',
                '\xf14',
                '\xf15',
                '\xf16',
                '\xf17',
                '\xf18',
                '\xf19',
                '\xf1a',
                '\xf1b',
                '\xf1c',
                '\xf1d',
                '\xf1e',
                '\xf1f',
                '\xf20',
                '\xf21',
                '\xf22',
                '\xf23',
                '\xf24',
                '\xf25',
                '\xf26',
                '\xf27',
                '\xf28',
                '\xf29',
                '\xf2a',
                '\xf2b',
                '\xf2c',
                '\xf2d',
                '\xf2e',
                '\xf2f',
                '\xf30',
                '\xf31',
                '\xf32',
                '\xf33',
                '\xf34',
                '\xf35',
                '\xf36',
                '\xf37',
                '\xf38',
                '\xf39',
                '\xf3a',
                '\xf3b',
                '\xf3c',
                '\xf3d',
                '\xf3e',
                '\xf3f',
                '\xf40',
                '\xf41',
                '\xf42',
                '\xf43',
                '\xf44',
                '\xf45',
                '\xf46',
                '\xf47',
                '\xf48',
                '\xf49',
                '\xf4a',
                '\xf4b',
                '\xf4c',
                '\xf4d',
                '\xf4e',
                '\xf4f',
                '\xf50',
                '\xf51',
                '\xf52',
                '\xf53',
                '\xf54',
                '\xf55',
                '\xf56',
                '\xf57',
                '\xf58',
                '\xf59',
                '\xf5a',
                '\xf5b',
                '\xf5c',
                '\xf5d',
                '\xf5e',
                '\xf5f',
                '\xf60',
                '\xf61',
                '\xf62',
                '\xf63',
                '\xf64',
                '\xf65',
                '\xf66',
                '\xf67',
                '\xf68',
                '\xf69',
                '\xf6a',
                '\xf6b',
                '\xf6c',
                '\xf6d',
                '\xf6e',
                '\xf6f',
                '\xf70',
                '\xf71',
                '\xf72',
                '\xf73',
                '\xf74',
                '\xf75',
                '\xf76',
                '\xf77',
                '\xf78',
                '\xf79',
                '\xf7a',
                '\xf7b',
                '\xf7c',
                '\xf7d',
                '\xf7e',
                '\xf7f',
                '\xf80',
                '\xf81',
                '\xf82',
                '\xf83',
                '\xf84',
                '\xf85',
                '\xf86',
                '\xf87',
                '\xf88',
                '\xf89',
                '\xf8a',
                '\xf8b',
                '\xf8c',
                '\xf8d',
                '\xf8e',
                '\xf8f',
                '\xf90',
                '\xf91',
                '\xf92',
                '\xf93',
                '\xf94',
                '\xf95',
                '\xf96',
                '\xf97',
                '\xf98',
                '\xf99',
                '\xf9a',
                '\xf9b',
                '\xf9c',
                '\xf9d',
                '\xf9e',
                '\xf9f',
                '\xfa0',
                '\xfa1',
                '\xfa2',
                '\xfa3',
                '\xfa4',
                '\xfa5',
                '\xfa6',
                '\xfa7',
                '\xfa8',
                '\xfa9',
                '\xfaa',
                '\xfab',
                '\xfac',
                '\xfad',
                '\xfae',
                '\xfaf',
                '\xfb0',
                '\xfb1',
                '\xfb2',
                '\xfb3',
                '\xfb4',
                '\xfb5',
                '\xfb6',
                '\xfb7',
                '\xfb8',
                '\xfb9',
                '\xfba',
                '\xfbb',
                '\xfbc',
                '\xfbd',
                '\xfbe',
                '\xfbf',
                '\xfc0',
                '\xfc1',
                '\xfc2',
                '\xfc3',
                '\xfc4',
                '\xfc5',
                '\xfc6',
                '\xfc7',
                '\xfc8',
                '\xfc9',
                '\xfca',
                '\xfcb',
                '\xfcc',
                '\xfcd',
                '\xfce',
                '\xfcf',
                '\xfd0',
                '\xfd1',
                '\xfd2',
                '\xfd3',
                '\xfd4',
                '\xfd5',
                '\xfd6',
                '\xfd7',

            };

            _riskStrings = new string[] 
            {

                "\xefc\xefd\xefe\xeff\xf00\xf01\xf02\xf03\xf04\xf05",
                "\xf06\xf07\xf08\xf09\xf0a\xf0b\xf0c\xf0d\xf0e\xf0f",
                "\xf10\xf11\xf12\xf13\xf14\xf15\xf16\xf17\xf18\xf19",
                "\xf1a\xf1b\xf1c\xf1d\xf1e\xf1f\xf20\xf21\xf22\xf23",
                "\xf24\xf25\xf26\xf27\xf28\xf29\xf2a\xf2b\xf2c\xf2d",
                "\xf2e\xf2f\xf30\xf31\xf32\xf33\xf34\xf35\xf36\xf37",
                "\xf38\xf39\xf3a\xf3b\xf3c\xf3d\xf3e\xf3f\xf40\xf41",
                "\xf42\xf43\xf44\xf45\xf46\xf47\xf48\xf49\xf4a\xf4b",
                "\xf4c\xf4d\xf4e\xf4f\xf50\xf51\xf52\xf53\xf54\xf55",
                "\xf56\xf57\xf58\xf59\xf5a\xf5b\xf5c\xf5d\xf5e\xf5f",
                "\xf60\xf61\xf62\xf63\xf64\xf65\xf66\xf67\xf68\xf69",
                "\xf6a\xf6b\xf6c\xf6d\xf6e\xf6f\xf70\xf71\xf72\xf73",
                "\xf74\xf75\xf76\xf77\xf78\xf79\xf7a\xf7b\xf7c\xf7d",
                "\xf7e\xf7f\xf80\xf81\xf82\xf83\xf84\xf85\xf86\xf87",
                "\xf88\xf89\xf8a\xf8b\xf8c\xf8d\xf8e\xf8f\xf90\xf91",
                "\xf92\xf93\xf94\xf95\xf96\xf97\xf98\xf99\xf9a\xf9b",
                "\xf9c\xf9d\xf9e\xf9f\xfa0\xfa1\xfa2\xfa3\xfa4\xfa5",
                "\xfa6\xfa7\xfa8\xfa9\xfaa\xfab\xfac\xfad\xfae\xfaf",
                "\xfb0\xfb1\xfb2\xfb3\xfb4\xfb5\xfb6\xfb7\xfb8\xfb9",
                "\xfba\xfbb\xfbc\xfbd\xfbe\xfbf\xfc0\xfc1\xfc2\xfc3",
                "\xfc4\xfc5\xfc6\xfc7\xfc8\xfc9\xfca\xfcb\xfcc\xfcd",
                "\xfce\xfcf\xfd0\xfd1\xfd2\xfd3\xfd4\xfd5\xfd6\xfd7",

            };
        }
    }

    ////////////////////////////////////////////////////////////////
    //fxUigurLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxUigurLanguage : fxLanguage
    {
	    //Constructor
        public fxUigurLanguage()
            : base(fxLanguage.GetSafeCultureInfo("ug-CN"), null)
        {
            //Collations
            //None, pure unicode
            _languageflags.Set(LanguageFlags.UnicodeOnly);

            //Killer Characters
            _validchars = new char[] 
            {

                '\x60a',
                '\x60b',
                '\x60c',
                '\x60d',
                '\x60e',
                '\x60f',
                '\x610',
                '\x611',
                '\x612',
                '\x613',
                '\x614',
                '\x615',
                '\x616',
                '\x617',
                '\x618',
                '\x619',
                '\x61a',
                '\x61b',
                '\x61c',
                '\x61d',
                '\x61e',
                '\x61f',
                '\x620',
                '\x621',
                '\x622',
                '\x623',
                '\x624',
                '\x625',
                '\x626',
                '\x627',
                '\x628',
                '\x629',
                '\x62a',
                '\x62b',
                '\x62c',
                '\x62d',
                '\x62e',
                '\x62f',
                '\x630',
                '\x631',
                '\x632',
                '\x633',
                '\x634',
                '\x635',
                '\x636',
                '\x637',
                '\x638',
                '\x639',
                '\x63a',
                '\x63b',
                '\x63c',
                '\x63d',
                '\x63e',
                '\x63f',
                '\x640',
                '\x641',
                '\x642',
                '\x643',
                '\x644',
                '\x645',
                '\x646',
                '\x647',
                '\x648',
                '\x649',
                '\x64a',
                '\x64b',
                '\x64c',
                '\x64d',
                '\x64e',
                '\x64f',
                '\x650',
                '\x651',
                '\x652',
                '\x653',
                '\x654',
                '\x655',
                '\x656',
                '\x657',
                '\x658',
                '\x659',
                '\x65a',
                '\x65b',
                '\x65c',
                '\x65d',
                '\x65e',
                '\x65f',
                '\x660',
                '\x661',
                '\x662',
                '\x663',
                '\x664',
                '\x665',
                '\x666',
                '\x667',
                '\x668',
                '\x669',
                '\x66a',
                '\x66b',
                '\x66c',
                '\x66d',
                '\x66e',
                '\x66f',
                '\x670',
                '\x671',
                '\x672',
                '\x673',
                '\x674',
                '\x675',
                '\x676',
                '\x677',
                '\x678',
                '\x679',
                '\x67a',
                '\x67b',
                '\x67c',
                '\x67d',
                '\x67e',
                '\x67f',
                '\x680',
                '\x681',
                '\x682',
                '\x683',
                '\x684',
                '\x685',
                '\x686',
                '\x687',
                '\x688',
                '\x689',
                '\x68a',
                '\x68b',
                '\x68c',
                '\x68d',
                '\x68e',
                '\x68f',
                '\x690',
                '\x691',
                '\x692',
                '\x693',
                '\x694',
                '\x695',
                '\x696',
                '\x697',
                '\x698',
                '\x699',
                '\x69a',
                '\x69b',
                '\x69c',
                '\x69d',
                '\x69e',
                '\x69f',
                '\x6a0',
                '\x6a1',
                '\x6a2',
                '\x6a3',
                '\x6a4',
                '\x6a5',
                '\x6a6',
                '\x6a7',
                '\x6a8',
                '\x6a9',
                '\x6aa',
                '\x6ab',
                '\x6ac',
                '\x6ad',
                '\x6ae',
                '\x6af',
                '\x6b0',
                '\x6b1',
                '\x6b2',
                '\x6b3',
                '\x6b4',
                '\x6b5',
                '\x6b6',
                '\x6b7',
                '\x6b8',
                '\x6b9',
                '\x6ba',
                '\x6bb',
                '\x6bc',
                '\x6bd',
                '\x6be',
                '\x6bf',
                '\x6c0',
                '\x6c1',
                '\x6c2',
                '\x6c3',
                '\x6c4',
                '\x6c5',
                '\x6c6',
                '\x6c7',
                '\x6c8',
                '\x6c9',
                '\x6ca',
                '\x6cb',
                '\x6cc',
                '\x6cd',
                '\x6ce',
                '\x6cf',
                '\x6d0',
                '\x6d1',
                '\x6d2',
                '\x6d3',
                '\x6d4',
                '\x6d5',
                '\x6d6',
                '\x6d7',
                '\x6d8',
                '\x6d9',
                '\x6da',
                '\x6db',
                '\xfb4e',
                '\xfb4f',
                '\xfb50',
                '\xfb51',
                '\xfb52',
                '\xfb53',
                '\xfb54',
                '\xfb55',
                '\xfb56',
                '\xfb57',
                '\xfb58',
                '\xfb59',
                '\xfb5a',
                '\xfb5b',
                '\xfb5c',
                '\xfb5d',
                '\xfb5e',
                '\xfb5f',
                '\xfb60',
                '\xfb61',
                '\xfb62',
                '\xfb63',
                '\xfb64',
                '\xfb65',
                '\xfb66',
                '\xfb67',
                '\xfb68',
                '\xfb69',
                '\xfb6a',
                '\xfb6b',
                '\xfb6c',
                '\xfb6d',
                '\xfb6e',
                '\xfb6f',
                '\xfb70',
                '\xfb71',
                '\xfb72',
                '\xfb73',
                '\xfb74',
                '\xfb75',
                '\xfb76',
                '\xfb77',
                '\xfb78',
                '\xfb79',
                '\xfb7a',
                '\xfb7b',
                '\xfb7c',
                '\xfb7d',
                '\xfb7e',
                '\xfb7f',
                '\xfb80',
                '\xfb81',
                '\xfb82',
                '\xfb83',
                '\xfb84',
                '\xfb85',
                '\xfb86',
                '\xfb87',
                '\xfb88',
                '\xfb89',
                '\xfb8a',
                '\xfb8b',
                '\xfb8c',
                '\xfb8d',
                '\xfb8e',
                '\xfb8f',
                '\xfb90',
                '\xfb91',
                '\xfb92',
                '\xfb93',
                '\xfb94',
                '\xfb95',
                '\xfb96',
                '\xfb97',
                '\xfb98',
                '\xfb99',
                '\xfb9a',
                '\xfb9b',
                '\xfb9c',
                '\xfb9d',
                '\xfb9e',
                '\xfb9f',
                '\xfba0',
                '\xfba1',
                '\xfba2',
                '\xfba3',
                '\xfba4',
                '\xfba5',
                '\xfba6',
                '\xfba7',
                '\xfba8',
                '\xfba9',
                '\xfbaa',
                '\xfbab',
                '\xfbac',
                '\xfbad',
                '\xfbae',
                '\xfbaf',
                '\xfbb0',
                '\xfbb1',
                '\xfbb2',
                '\xfbb3',
                '\xfbb4',
                '\xfbb5',
                '\xfbb6',
                '\xfbb7',
                '\xfbb8',
                '\xfbb9',
                '\xfbba',
                '\xfbbb',
                '\xfbbc',
                '\xfbbd',
                '\xfbbe',
                '\xfbbf',
                '\xfbc0',
                '\xfbc1',
                '\xfbc2',
                '\xfbc3',
                '\xfbc4',
                '\xfbc5',
                '\xfbc6',
                '\xfbc7',
                '\xfbc8',
                '\xfbc9',
                '\xfbca',
                '\xfbcb',
                '\xfbcc',
                '\xfbcd',
                '\xfbce',
                '\xfbcf',
                '\xfbd0',
                '\xfbd1',
                '\xfbd2',
                '\xfbd3',
                '\xfbd4',
                '\xfbd5',
                '\xfbd6',
                '\xfbd7',
                '\xfbd8',
                '\xfbd9',
                '\xfbda',
                '\xfbdb',
                '\xfbdc',
                '\xfbdd',
                '\xfbde',
                '\xfbdf',
                '\xfbe0',
                '\xfbe1',
                '\xfbe2',
                '\xfbe3',
                '\xfbe4',
                '\xfbe5',
                '\xfbe6',
                '\xfbe7',
                '\xfbe8',
                '\xfbe9',
                '\xfbea',
                '\xfbeb',
                '\xfbec',
                '\xfbed',
                '\xfbee',
                '\xfbef',
                '\xfbf0',
                '\xfbf1',
                '\xfbf2',
                '\xfbf3',
                '\xfbf4',
                '\xfbf5',
                '\xfbf6',
                '\xfbf7',
                '\xfbf8',
                '\xfbf9',
                '\xfbfa',
                '\xfbfb',
                '\xfbfc',
                '\xfbfd',
                '\xfbfe',
                '\xfbff',
                '\xfc00',
                '\xfc01',
                '\xfe84',
                '\xfe85',
                '\xfe86',
                '\xfe87',
                '\xfe88',
                '\xfe89',
                '\xfe8a',
                '\xfe8b',
                '\xfe8c',
                '\xfe8d',
                '\xfe8e',
                '\xfe8f',
                '\xfe90',
                '\xfe91',
                '\xfe92',
                '\xfe93',
                '\xfe94',
                '\xfe95',
                '\xfe96',
                '\xfe97',
                '\xfe98',
                '\xfe99',
                '\xfe9a',
                '\xfe9b',
                '\xfe9c',
                '\xfe9d',
                '\xfe9e',
                '\xfe9f',
                '\xfea0',
                '\xfea1',
                '\xfea2',
                '\xfea3',
                '\xfea4',
                '\xfea5',
                '\xfea6',
                '\xfea7',
                '\xfea8',
                '\xfea9',
                '\xfeaa',
                '\xfeab',
                '\xfeac',
                '\xfead',
                '\xfeae',
                '\xfeaf',
                '\xfeb0',
                '\xfeb1',
                '\xfeb2',
                '\xfeb3',
                '\xfeb4',
                '\xfeb5',
                '\xfeb6',
                '\xfeb7',
                '\xfeb8',
                '\xfeb9',
                '\xfeba',
                '\xfebb',
                '\xfebc',
                '\xfebd',
                '\xfebe',
                '\xfebf',
                '\xfec0',
                '\xfec1',
                '\xfec2',
                '\xfec3',
                '\xfec4',
                '\xfec5',
                '\xfec6',
                '\xfec7',
                '\xfec8',
                '\xfec9',
                '\xfeca',
                '\xfecb',
                '\xfecc',
                '\xfecd',
                '\xfece',
                '\xfecf',
                '\xfed0',
                '\xfed1',
                '\xfed2',
                '\xfed3',
                '\xfed4',
                '\xfed5',
                '\xfed6',
                '\xfed7',
                '\xfed8',
                '\xfed9',
                '\xfeda',
                '\xfedb',
                '\xfedc',
                '\xfedd',
                '\xfede',
                '\xfedf',
                '\xfee0',
                '\xfee1',
                '\xfee2',
                '\xfee3',
                '\xfee4',
                '\xfee5',
                '\xfee6',
                '\xfee7',
                '\xfee8',
                '\xfee9',
                '\xfeea',
                '\xfeeb',
                '\xfeec',
                '\xfeed',
                '\xfeee',
                '\xfeef',
                '\xfef0',
                '\xfef1',
                '\xfef2',
                '\xfef3',
                '\xfef4',
                '\xfef5',
                '\xfef6',
                '\xfef7',
                '\xfef8',
                '\xfef9',
                '\xfefa',
                '\xfefb',
                '\xfefc',
                '\xfefd',
                '\xfefe',
                '\xfeff',
                '\xff00',
                '\xff5f',
                '\xff60',
                '\xff61',
                '\xff62',
                '\xff63',

            };

            _riskStrings = new string[] 
            {

                "\x60a\x60b\x60c\x60d\x60e\x60f\x610\x611\x612\x613",
                "\x614\x615\x616\x617\x618\x619\x61a\x61b\x61c\x61d",
                "\x61e\x61f\x620\x621\x622\x623\x624\x625\x626\x627",
                "\x628\x629\x62a\x62b\x62c\x62d\x62e\x62f\x630\x631",
                "\x632\x633\x634\x635\x636\x637\x638\x639\x63a\x63b",
                "\x63c\x63d\x63e\x63f\x640\x641\x642\x643\x644\x645",
                "\x646\x647\x648\x649\x64a\x64b\x64c\x64d\x64e\x64f",
                "\x650\x651\x652\x653\x654\x655\x656\x657\x658\x659",
                "\x65a\x65b\x65c\x65d\x65e\x65f\x660\x661\x662\x663",
                "\x664\x665\x666\x667\x668\x669\x66a\x66b\x66c\x66d",
                "\x66e\x66f\x670\x671\x672\x673\x674\x675\x676\x677",
                "\x678\x679\x67a\x67b\x67c\x67d\x67e\x67f\x680\x681",
                "\x682\x683\x684\x685\x686\x687\x688\x689\x68a\x68b",
                "\x68c\x68d\x68e\x68f\x690\x691\x692\x693\x694\x695",
                "\x696\x697\x698\x699\x69a\x69b\x69c\x69d\x69e\x69f",
                "\x6a0\x6a1\x6a2\x6a3\x6a4\x6a5\x6a6\x6a7\x6a8\x6a9",
                "\x6aa\x6ab\x6ac\x6ad\x6ae\x6af\x6b0\x6b1\x6b2\x6b3",
                "\x6b4\x6b5\x6b6\x6b7\x6b8\x6b9\x6ba\x6bb\x6bc\x6bd",
                "\x6be\x6bf\x6c0\x6c1\x6c2\x6c3\x6c4\x6c5\x6c6\x6c7",
                "\x6c8\x6c9\x6ca\x6cb\x6cc\x6cd\x6ce\x6cf\x6d0\x6d1",
                "\x6d2\x6d3\x6d4\x6d5\x6d6\x6d7\x6d8\x6d9\x6da\x6db",
                "\xfb4e\xfb4f\xfb50\xfb51\xfb52\xfb53\xfb54\xfb55\xfb56\xfb57",
                "\xfb58\xfb59\xfb5a\xfb5b\xfb5c\xfb5d\xfb5e\xfb5f\xfb60\xfb61",
                "\xfb62\xfb63\xfb64\xfb65\xfb66\xfb67\xfb68\xfb69\xfb6a\xfb6b",
                "\xfb6c\xfb6d\xfb6e\xfb6f\xfb70\xfb71\xfb72\xfb73\xfb74\xfb75",
                "\xfb76\xfb77\xfb78\xfb79\xfb7a\xfb7b\xfb7c\xfb7d\xfb7e\xfb7f",
                "\xfb80\xfb81\xfb82\xfb83\xfb84\xfb85\xfb86\xfb87\xfb88\xfb89",
                "\xfb8a\xfb8b\xfb8c\xfb8d\xfb8e\xfb8f\xfb90\xfb91\xfb92\xfb93",
                "\xfb94\xfb95\xfb96\xfb97\xfb98\xfb99\xfb9a\xfb9b\xfb9c\xfb9d",
                "\xfb9e\xfb9f\xfba0\xfba1\xfba2\xfba3\xfba4\xfba5\xfba6\xfba7",
                "\xfba8\xfba9\xfbaa\xfbab\xfbac\xfbad\xfbae\xfbaf\xfbb0\xfbb1",
                "\xfbb2\xfbb3\xfbb4\xfbb5\xfbb6\xfbb7\xfbb8\xfbb9\xfbba\xfbbb",
                "\xfbbc\xfbbd\xfbbe\xfbbf\xfbc0\xfbc1\xfbc2\xfbc3\xfbc4\xfbc5",
                "\xfbc6\xfbc7\xfbc8\xfbc9\xfbca\xfbcb\xfbcc\xfbcd\xfbce\xfbcf",
                "\xfbd0\xfbd1\xfbd2\xfbd3\xfbd4\xfbd5\xfbd6\xfbd7\xfbd8\xfbd9",
                "\xfbda\xfbdb\xfbdc\xfbdd\xfbde\xfbdf\xfbe0\xfbe1\xfbe2\xfbe3",
                "\xfbe4\xfbe5\xfbe6\xfbe7\xfbe8\xfbe9\xfbea\xfbeb\xfbec\xfbed",
                "\xfbee\xfbef\xfbf0\xfbf1\xfbf2\xfbf3\xfbf4\xfbf5\xfbf6\xfbf7",
                "\xfbf8\xfbf9\xfbfa\xfbfb\xfbfc\xfbfd\xfbfe\xfbff\xfc00\xfc01",
                "\xfe84\xfe85\xfe86\xfe87\xfe88\xfe89\xfe8a\xfe8b\xfe8c\xfe8d",
                "\xfe8e\xfe8f\xfe90\xfe91\xfe92\xfe93\xfe94\xfe95\xfe96\xfe97",
                "\xfe98\xfe99\xfe9a\xfe9b\xfe9c\xfe9d\xfe9e\xfe9f\xfea0\xfea1",
                "\xfea2\xfea3\xfea4\xfea5\xfea6\xfea7\xfea8\xfea9\xfeaa\xfeab",
                "\xfeac\xfead\xfeae\xfeaf\xfeb0\xfeb1\xfeb2\xfeb3\xfeb4\xfeb5",
                "\xfeb6\xfeb7\xfeb8\xfeb9\xfeba\xfebb\xfebc\xfebd\xfebe\xfebf",
                "\xfec0\xfec1\xfec2\xfec3\xfec4\xfec5\xfec6\xfec7\xfec8\xfec9",
                "\xfeca\xfecb\xfecc\xfecd\xfece\xfecf\xfed0\xfed1\xfed2\xfed3",
                "\xfed4\xfed5\xfed6\xfed7\xfed8\xfed9\xfeda\xfedb\xfedc\xfedd",
                "\xfede\xfedf\xfee0\xfee1\xfee2\xfee3\xfee4\xfee5\xfee6\xfee7",
                "\xfee8\xfee9\xfeea\xfeeb\xfeec\xfeed\xfeee\xfeef\xfef0\xfef1",
                "\xfef2\xfef3\xfef4\xfef5\xfef6\xfef7\xfef8\xfef9\xfefa\xfefb",
                "\xfefc\xfefd\xfefe\xfeff\xff00\xff5f\xff60\xff61\xff62\xff63",

            };
        }
    }

    ////////////////////////////////////////////////////////////////
    //fxYiLanguage
    //
    ////////////////////////////////////////////////////////////////
    public class fxYiLanguage : fxLanguage
    {
	    //Constructor
        public fxYiLanguage()
            : base(fxLanguage.GetSafeCultureInfo("ii-CN"), null)
        {
            //Collations
            //None, pure unicode
            _languageflags.Set(LanguageFlags.UnicodeOnly);

            //Killer Characters
            _validchars = new char[] 
            {
                '\x9ffd',
                '\x9ffe',
                '\x9fff',
                '\xa000',
                '\xa026',
                '\xa027',
                '\xa028',
                '\xa074',
                '\xa4ca',
            };

            _riskStrings = new string[] 
            {
                "\x9ffd\x9ffe\x9fff\xa000\xa001\xa002\xa003\xa004\xa005\xa006",
                //"\xa007\xa008\xa009\xa00a\xa00b\xa00c\xa00d\xa00e\xa00f\xa010",
                //"\xa011\xa012\xa013\xa014\xa015\xa016\xa017\xa018\xa019\xa01a",
                //"\xa039\xa03a\xa03b\xa03c\xa03d\xa03e\xa03f\xa040\xa041\xa042",
                //"\xa043\xa044\xa045\xa046\xa047\xa048\xa049\xa04a\xa04b\xa04c",
                //"\xa1c9\xa1ca\xa1cb\xa1cc\xa1cd\xa1ce\xa1cf\xa1d0\xa1d1\xa1d2",
                //"\xa1d3\xa1d4\xa1d5\xa1d6\xa1d7\xa1d8\xa1d9\xa1da\xa1db\xa1dc",
                //"\xa223\xa224\xa225\xa226\xa227\xa228\xa229\xa22a\xa22b\xa22c",
                //"\xa22d\xa22e\xa22f\xa230\xa231\xa232\xa233\xa234\xa235\xa236",
                //"\xa313\xa314\xa315\xa316\xa317\xa318\xa319\xa31a\xa31b\xa31c",
                //"\xa417\xa418\xa419\xa41a\xa41b\xa41c\xa41d\xa41e\xa41f\xa420",
                //"\xa4c1\xa4c2\xa4c3\xa4c4\xa4c5\xa4c6\xa4c7\xa4c8\xa4c9\xa4ca",

            };
        }
    }


    //fxAdditionalGB18030Characters
    //
    ////////////////////////////////////////////////////////////////
    public class fxAdditionalGB18030Characters : fxLanguage
    {
        //Constructor
        public fxAdditionalGB18030Characters()
            : base(null, null)
        {
            //Collations
            //None, pure unicode
            _languageflags.Set(LanguageFlags.UnicodeOnly);

            //Killer Characters
            _validchars = new char[] 
		     {
                 //from fxChineseLanguage: 
                 //These are not part of the 936 code page
                '\uA000',
                '\uA4C6',
        		'\uFB56',
        		'\uFBFF',
        		'\u00C7',
        		'\u00F6',
        		'\u00FE',
        		'\u0626',
        		'\u0F03',
        		'\u0F31',
        		'\u0FC4',
        		'\u1800',
        		'\u18A9',
        		'\u3400',
        		'\u35DF',
        		'\u3F2C',
        		'\u4390',
        		'\u4A01',
        		'\u4DB5',

                //from fxArabicLanguage
                //These are not part of 1256 code page: moved to fxAdditionalGB18030Characters		
        		'\u06DE',	
        		'\u06E9',	
        		'\u06FB',	
        		'\u06FE',	
        		'\uFB50',	
        		'\uFB73',	
        		'\uFBB1',	
        		'\uFD3E',	
        		'\uFEBF',	
        		'\uFEED',	
        		'\uFEFC',	

                //from fxChineseTraditionalLanguage
                //These are not part of the 950 code page: added to "fxAdditionalGB18030Characters
      		    '\u9289',

                //from fxCyrillicLanguage	
                //These are not part of the 1251 code page: added to fxAdditionalGB18030Characters
        		'\u04E9',	

                //from fxGreekLanguage
                //These are not part of the 1253 code page. added to fxAdditionalGB18030Characters
        		'\u00FF'
		    };
        }
    }



}
