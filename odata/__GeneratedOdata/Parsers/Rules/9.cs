namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _compoundKeyParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._compoundKey> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _keyValuePair_1 in __GeneratedOdata.Parsers.Rules._keyValuePairParser.Instance
from _ⲤCOMMA_keyValuePairↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤCOMMA_keyValuePairↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._compoundKey(_OPEN_1, _keyValuePair_1, _ⲤCOMMA_keyValuePairↃ_1, _CLOSE_1);
    }
    
}
