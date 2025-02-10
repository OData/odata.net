namespace __GeneratedOdataV4.Parsers.Rules
{
    using __GeneratedOdataV4.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _odataIdentifierParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._odataIdentifier> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._odataIdentifier>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public _odataIdentifier Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                var _identifierLeadingCharacter_1 = __GeneratedOdataV4.Parsers.Rules._identifierLeadingCharacterParser.Instance2.Parse(input, start, out newStart);

                /*var _identifierCharacter_1 = __GeneratedOdataV4.Parsers.Rules._identifierCharacterParser.Instance2.Parse(input, newStart, out newStart);
                for (int i = 0; i < 3; ++i)
                {
                    _identifierCharacter_1 = __GeneratedOdataV4.Parsers.Rules._identifierCharacterParser.Instance2.Parse(input, newStart, out newStart);
                }*/

                newStart = start;
                for (; newStart < start + 5; ++newStart)
                {
                    var next = input[newStart];
                }

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._odataIdentifier> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._odataIdentifier>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._odataIdentifier> Parse(IInput<char>? input)
            {
                var _identifierLeadingCharacter_1 = __GeneratedOdataV4.Parsers.Rules._identifierLeadingCharacterParser.Instance.Parse(input);

var _identifierCharacter_1 = __GeneratedOdataV4.Parsers.Rules._identifierCharacterParser.Instance.Repeat(0, 127).Parse(_identifierLeadingCharacter_1.Remainder);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._odataIdentifier(_identifierLeadingCharacter_1.Parsed, new __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtMost127<__GeneratedOdataV4.CstNodes.Rules._identifierCharacter>(_identifierCharacter_1.Parsed)), _identifierCharacter_1.Remainder);
            }
        }
    }
    
}
