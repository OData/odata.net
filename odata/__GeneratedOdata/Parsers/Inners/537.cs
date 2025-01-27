namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_geoLiteralParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._COMMA_geoLiteral> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _geoLiteral_1 in __GeneratedOdata.Parsers.Rules._geoLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_geoLiteral(_COMMA_1, _geoLiteral_1);
    }
    
}
