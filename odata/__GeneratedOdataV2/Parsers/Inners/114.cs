namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _SEMI_expandOptionParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._SEMI_expandOption> Instance { get; } = from _SEMI_1 in __GeneratedOdataV2.Parsers.Rules._SEMIParser.Instance
from _expandOption_1 in __GeneratedOdataV2.Parsers.Rules._expandOptionParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._SEMI_expandOption(_SEMI_1, _expandOption_1);
    }
    
}
