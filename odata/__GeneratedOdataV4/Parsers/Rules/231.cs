namespace __GeneratedOdataV4.Parsers.Rules
{
    using __GeneratedOdataV4.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _entitySetNameParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._entitySetName, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._entitySetName>> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._entitySetName, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._entitySetName>>
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

            private static __GeneratedOdataV4.CstNodes.Rules._entitySetName Node =
                new __GeneratedOdataV4.CstNodes.Rules._entitySetName(
                    Users);

            public CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _entitySetName> Parse(in CombinatorParsingV3.StringInput input)
            {
                CombinatorParsingV3.StringInput remainder = input.Next(out var more);

                for (int i = 0; i < 4; ++i)
                {
                    remainder = remainder.Next(out more);
                }

                //// TODO add check to tryparse about more

                return new CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _entitySetName>(
                    true,
                    Node,
                    more,
                    remainder);
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._entitySetName> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._entitySetName>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._entitySetName> Parse(IInput<char>? input)
            {
                var _odataIdentifier_1 = __GeneratedOdataV4.Parsers.Rules._odataIdentifierParser.Instance.Parse(input);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._entitySetName(_odataIdentifier_1.Parsed), _odataIdentifier_1.Remainder);
            }
        }
    }
    
}
