namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _COMMA_selectListItemParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._COMMA_selectListItem> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _selectListItem_1 in __GeneratedOdata.Parsers.Rules._selectListItemParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_selectListItem(_COMMA_1, _selectListItem_1);
    }
    
}
