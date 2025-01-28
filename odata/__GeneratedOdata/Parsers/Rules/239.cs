namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _odataIdentifierParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataIdentifier> Instance { get; } = from _identifierLeadingCharacter_1 in __GeneratedOdata.Parsers.Rules._identifierLeadingCharacterParser.Instance
from _identifierCharacter_1 in __GeneratedOdata.Parsers.Rules._identifierCharacterParser.Instance.Repeat(0, 127)
select new __GeneratedOdata.CstNodes.Rules._odataIdentifier(_identifierLeadingCharacter_1, new __GeneratedOdata.CstNodes.Inners.HelperRangedAtMost127<__GeneratedOdata.CstNodes.Rules._identifierCharacter>(_identifierCharacter_1));
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataIdentifier> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._odataIdentifier>
        {
            public IOutput<char, _odataIdentifier> Parse(IInput<char>? input)
            {
                var _identifierLeadingCharacter_1 = __GeneratedOdata.Parsers.Rules._identifierLeadingCharacterParser.Instance.Parse(input);
                var _identifierCharacter_1 = __GeneratedOdata.Parsers.Rules._identifierCharacterParser.Instance.Repeat(0, 127).Parse(_identifierLeadingCharacter_1.Remainder);
                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Rules._odataIdentifier(_identifierLeadingCharacter_1.Parsed, new __GeneratedOdata.CstNodes.Inners.HelperRangedAtMost127<__GeneratedOdata.CstNodes.Rules._identifierCharacter>(_identifierCharacter_1.Parsed)),
                    _identifierCharacter_1.Remainder);
            }
        }
    }
    
}
