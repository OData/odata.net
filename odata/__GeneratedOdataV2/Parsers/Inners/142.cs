namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_orderbyItemParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._COMMA_orderbyItem> Instance { get; } = from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _orderbyItem_1 in __GeneratedOdataV2.Parsers.Rules._orderbyItemParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._COMMA_orderbyItem(_COMMA_1, _orderbyItem_1);
    }
    
}
