namespace __GeneratedOdataV4.Parsers.Rules
{
    using __GeneratedOdataV4.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _odataIdentifierParser
    {
        /*public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._odataIdentifier, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._odataIdentifier>> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._odataIdentifier, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._odataIdentifier>>
        {
            private static __GeneratedOdataV4.CstNodes.Rules._odataIdentifier Users =
                new __GeneratedOdataV4.CstNodes.Rules._odataIdentifier(
                    new __GeneratedOdataV4.CstNodes.Rules._identifierLeadingCharacter._ALPHA(
                        new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                            __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._75.Instance)),
                    new CstNodes.Inners.HelperRangedAtMost127<__GeneratedOdataV4.CstNodes.Rules._identifierCharacter>(
                        new[]
                        {
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._73.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._65.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._72.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._73.Instance)),
                        }));

            public CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _odataIdentifier> Parse(in CombinatorParsingV3.StringInput input)
            {
                CombinatorParsingV3.StringInput remainder = input.Next(out var more);

                for (int i = 0; i < 4; ++i)
                {
                    remainder = remainder.Next(out more);
                }

                return new CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _odataIdentifier>(
                    true,
                    default,
                    more,
                    remainder);
            }
        }*/

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
