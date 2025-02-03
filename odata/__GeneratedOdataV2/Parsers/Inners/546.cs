namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_pointDataParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._COMMA_pointData> Instance { get; } = from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _pointData_1 in __GeneratedOdataV2.Parsers.Rules._pointDataParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._COMMA_pointData(_COMMA_1, _pointData_1);
    }
    
}
