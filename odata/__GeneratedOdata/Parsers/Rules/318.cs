namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _fullMultiLineStringLiteralParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._fullMultiLineStringLiteral> Instance { get; } = from _sridLiteral_1 in __GeneratedOdata.Parsers.Rules._sridLiteralParser.Instance
from _multiLineStringLiteral_1 in __GeneratedOdata.Parsers.Rules._multiLineStringLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._fullMultiLineStringLiteral(_sridLiteral_1, _multiLineStringLiteral_1);
    }
    
}
