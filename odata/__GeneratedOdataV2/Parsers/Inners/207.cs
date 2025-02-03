namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_selectItemParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._COMMA_selectItem> Instance { get; } = from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _selectItem_1 in __GeneratedOdataV2.Parsers.Rules._selectItemParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._COMMA_selectItem(_COMMA_1, _selectItem_1);
    }
    
}
