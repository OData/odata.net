namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_pointDataParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._COMMA_pointData> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _pointData_1 in __GeneratedOdata.Parsers.Rules._pointDataParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_pointData(_COMMA_1, _pointData_1);
    }
    
}
