namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_lineStringDataParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._COMMA_lineStringData> Instance { get; } = from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _lineStringData_1 in __GeneratedOdataV2.Parsers.Rules._lineStringDataParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._COMMA_lineStringData(_COMMA_1, _lineStringData_1);
    }
    
}
