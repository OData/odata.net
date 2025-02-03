namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pointDataParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._pointData> Instance { get; } = from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _positionLiteral_1 in __GeneratedOdataV2.Parsers.Rules._positionLiteralParser.Instance
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._pointData(_OPEN_1, _positionLiteral_1, _CLOSE_1);
    }
    
}
