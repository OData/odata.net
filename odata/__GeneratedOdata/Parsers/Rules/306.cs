namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _enumValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._enumValue> Instance { get; } = from _singleEnumValue_1 in __GeneratedOdata.Parsers.Rules._singleEnumValueParser.Instance
from _ⲤCOMMA_singleEnumValueↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤCOMMA_singleEnumValueↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._enumValue(_singleEnumValue_1, _ⲤCOMMA_singleEnumValueↃ_1);
    }
    
}
