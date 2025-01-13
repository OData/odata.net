namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _COMMA_orderbyItemParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._COMMA_orderbyItem> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _orderbyItem_1 in __GeneratedOdata.Parsers.Rules._orderbyItemParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_orderbyItem(_COMMA_1, _orderbyItem_1);
    }
    
}
