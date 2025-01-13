namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _pctⲻencodedParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._pctⲻencoded> Instance { get; } = from _ʺx25ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx25ʺParser.Instance
from _HEXDIG_1 in __GeneratedOdata.Parsers.Rules._HEXDIGParser.Instance
from _HEXDIG_2 in __GeneratedOdata.Parsers.Rules._HEXDIGParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pctⲻencoded(_ʺx25ʺ_1, _HEXDIG_1, _HEXDIG_2);
    }
    
}
