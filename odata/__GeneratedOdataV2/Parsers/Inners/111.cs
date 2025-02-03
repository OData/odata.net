namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _SEMI_expandCountOptionParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._SEMI_expandCountOption> Instance { get; } = from _SEMI_1 in __GeneratedOdataV2.Parsers.Rules._SEMIParser.Instance
from _expandCountOption_1 in __GeneratedOdataV2.Parsers.Rules._expandCountOptionParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._SEMI_expandCountOption(_SEMI_1, _expandCountOption_1);
    }
    
}
