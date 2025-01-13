namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _COMMA_computeItemParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._COMMA_computeItem> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _computeItem_1 in __GeneratedOdata.Parsers.Rules._computeItemParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_computeItem(_COMMA_1, _computeItem_1);
    }
    
}
