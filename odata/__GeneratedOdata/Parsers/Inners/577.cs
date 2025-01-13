namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _EQⲻh_booleanValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._EQⲻh_booleanValue> Instance { get; } = from _EQⲻh_1 in __GeneratedOdata.Parsers.Rules._EQⲻhParser.Instance
from _booleanValue_1 in __GeneratedOdata.Parsers.Rules._booleanValueParser.Instance
select new __GeneratedOdata.CstNodes.Inners._EQⲻh_booleanValue(_EQⲻh_1, _booleanValue_1);
    }
    
}
