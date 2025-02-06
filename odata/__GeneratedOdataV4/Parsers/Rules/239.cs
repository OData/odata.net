namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _odataIdentifierParser
    {
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
