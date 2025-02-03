namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSEParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE> Instance { get; } = from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _expandRefOption_1 in __GeneratedOdataV2.Parsers.Rules._expandRefOptionParser.Instance
from _ⲤSEMI_expandRefOptionↃ_1 in Inners._ⲤSEMI_expandRefOptionↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE(_OPEN_1, _expandRefOption_1, _ⲤSEMI_expandRefOptionↃ_1, _CLOSE_1);
    }
    
}
