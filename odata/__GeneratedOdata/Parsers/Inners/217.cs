namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _OPEN_parameterNames_CLOSEParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._OPEN_parameterNames_CLOSE> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _parameterNames_1 in __GeneratedOdata.Parsers.Rules._parameterNamesParser.Instance
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Inners._OPEN_parameterNames_CLOSE(_OPEN_1, _parameterNames_1, _CLOSE_1);
    }
    
}
