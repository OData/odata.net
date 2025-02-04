namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _odataIdentifierParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataIdentifier> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataIdentifier>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._odataIdentifier> Parse(IInput<char>? input)
            {
                var _identifierLeadingCharacter_1 = __GeneratedOdataV3.Parsers.Rules._identifierLeadingCharacterParser.Instance.Parse(input);
if (!_identifierLeadingCharacter_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataIdentifier)!, input);
}

var _identifierCharacter_1 = __GeneratedOdataV3.Parsers.Rules._identifierCharacterParser.Instance.Repeat(0, 127).Parse(_identifierLeadingCharacter_1.Remainder);
if (!_identifierCharacter_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataIdentifier)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._odataIdentifier(_identifierLeadingCharacter_1.Parsed, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtMost127<__GeneratedOdataV3.CstNodes.Rules._identifierCharacter>(_identifierCharacter_1.Parsed)), _identifierCharacter_1.Remainder);
            }
        }
    }
    
}
