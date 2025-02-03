namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _SEMI_expandRefOptionParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._SEMI_expandRefOption> Instance { get; } = from _SEMI_1 in __GeneratedOdataV2.Parsers.Rules._SEMIParser.Instance
from _expandRefOption_1 in __GeneratedOdataV2.Parsers.Rules._expandRefOptionParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._SEMI_expandRefOption(_SEMI_1, _expandRefOption_1);
    }
    
}
