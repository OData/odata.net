namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fullLineStringLiteralParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._fullLineStringLiteral> Instance { get; } = from _sridLiteral_1 in __GeneratedOdata.Parsers.Rules._sridLiteralParser.Instance
from _lineStringLiteral_1 in __GeneratedOdata.Parsers.Rules._lineStringLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._fullLineStringLiteral(_sridLiteral_1, _lineStringLiteral_1);
    }
    
}
