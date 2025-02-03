namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _refⳆOPEN_levels_CLOSEParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE> Instance { get; } = (_refParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE>(_OPEN_levels_CLOSEParser.Instance);
        
        public static class _refParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE._ref> Instance { get; } = from _ref_1 in __GeneratedOdataV3.Parsers.Rules._refParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE._ref(_ref_1);
        }
        
        public static class _OPEN_levels_CLOSEParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE._OPEN_levels_CLOSE> Instance { get; } = from _OPEN_1 in __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance
from _levels_1 in __GeneratedOdataV3.Parsers.Rules._levelsParser.Instance
from _CLOSE_1 in __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE._OPEN_levels_CLOSE(_OPEN_1, _levels_1, _CLOSE_1);
        }
    }
    
}
