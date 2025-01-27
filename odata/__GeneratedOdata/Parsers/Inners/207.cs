namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_selectItemParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._COMMA_selectItem> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _selectItem_1 in __GeneratedOdata.Parsers.Rules._selectItemParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_selectItem(_COMMA_1, _selectItem_1);
    }
    
}
