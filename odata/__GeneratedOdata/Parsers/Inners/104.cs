namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _COMMA_expandItemParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._COMMA_expandItem> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _expandItem_1 in __GeneratedOdata.Parsers.Rules._expandItemParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_expandItem(_COMMA_1, _expandItem_1);
    }
    
}