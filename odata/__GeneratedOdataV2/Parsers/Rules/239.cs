namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _odataIdentifierParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._odataIdentifier> Instance { get; } = from _identifierLeadingCharacter_1 in __GeneratedOdataV2.Parsers.Rules._identifierLeadingCharacterParser.Instance
from _identifierCharacter_1 in __GeneratedOdataV2.Parsers.Rules._identifierCharacterParser.Instance.Repeat(0, 127)
select new __GeneratedOdataV2.CstNodes.Rules._odataIdentifier(_identifierLeadingCharacter_1, new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtMost127<__GeneratedOdataV2.CstNodes.Rules._identifierCharacter>(_identifierCharacter_1));
    }
    
}
