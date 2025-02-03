namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _OPEN_parameterNames_CLOSEParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._OPEN_parameterNames_CLOSE> Instance { get; } = from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _parameterNames_1 in __GeneratedOdataV2.Parsers.Rules._parameterNamesParser.Instance
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._OPEN_parameterNames_CLOSE(_OPEN_1, _parameterNames_1, _CLOSE_1);
    }
    
}
