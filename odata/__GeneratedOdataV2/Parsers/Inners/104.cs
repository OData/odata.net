namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_expandItemParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._COMMA_expandItem> Instance { get; } = from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _expandItem_1 in __GeneratedOdataV2.Parsers.Rules._expandItemParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._COMMA_expandItem(_COMMA_1, _expandItem_1);
    }
    
}
