namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_ringLiteralParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._COMMA_ringLiteral> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _ringLiteral_1 in __GeneratedOdata.Parsers.Rules._ringLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_ringLiteral(_COMMA_1, _ringLiteral_1);
    }
    
}
