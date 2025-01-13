namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _fullPointLiteralParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._fullPointLiteral> Instance { get; } = from _sridLiteral_1 in __GeneratedOdata.Parsers.Rules._sridLiteralParser.Instance
from _pointLiteral_1 in __GeneratedOdata.Parsers.Rules._pointLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._fullPointLiteral(_sridLiteral_1, _pointLiteral_1);
    }
    
}
