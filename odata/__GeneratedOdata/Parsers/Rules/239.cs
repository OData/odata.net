namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _odataIdentifierParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._odataIdentifier> Instance { get; } = from _identifierLeadingCharacter_1 in __GeneratedOdata.Parsers.Rules._identifierLeadingCharacterParser.Instance
from _identifierCharacter_1 in __GeneratedOdata.Parsers.Rules._identifierCharacterParser.Instance.Repeat(0, 127)
select new __GeneratedOdata.CstNodes.Rules._odataIdentifier(_identifierLeadingCharacter_1, new __GeneratedOdata.CstNodes.Inners.HelperRangedAtMost127<__GeneratedOdata.CstNodes.Rules._identifierCharacter>(_identifierCharacter_1));
    }
    
}
