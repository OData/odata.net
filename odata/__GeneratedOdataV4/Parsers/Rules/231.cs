namespace __GeneratedOdataV4.Parsers.Rules
{
    using __GeneratedOdataV4.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _entitySetNameParser
    {
        /*public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._entitySetName, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._entitySetName>> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._entitySetName, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._entitySetName>>
        {
            public CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _entitySetName> Parse(in CombinatorParsingV3.StringInput input)
            {
                var _odataIdentifier = __GeneratedOdataV4.Parsers.Rules._odataIdentifierParser.Instance2.Parse(input);

                return new CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _entitySetName>(
                    true,
                    default,
                    _odataIdentifier.HasRemainder,
                    _odataIdentifier.Remainder);
            }
        }*/

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
