namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pointDataParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._pointData> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _positionLiteral_1 in __GeneratedOdata.Parsers.Rules._positionLiteralParser.Instance
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pointData(_OPEN_1, _positionLiteral_1, _CLOSE_1);
    }
    
}
