namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_positionLiteralParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._COMMA_positionLiteral> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _positionLiteral_1 in __GeneratedOdata.Parsers.Rules._positionLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_positionLiteral(_COMMA_1, _positionLiteral_1);
    }
    
}
