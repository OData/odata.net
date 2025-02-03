namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_computeItemParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._COMMA_computeItem> Instance { get; } = from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _computeItem_1 in __GeneratedOdataV2.Parsers.Rules._computeItemParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._COMMA_computeItem(_COMMA_1, _computeItem_1);
    }
    
}
