namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pctⲻencodedParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._pctⲻencoded> Instance { get; } = from _ʺx25ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx25ʺParser.Instance
from _HEXDIG_1 in __GeneratedOdataV2.Parsers.Rules._HEXDIGParser.Instance
from _HEXDIG_2 in __GeneratedOdataV2.Parsers.Rules._HEXDIGParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._pctⲻencoded(_ʺx25ʺ_1, _HEXDIG_1, _HEXDIG_2);
    }
    
}
