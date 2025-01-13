namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _geometryPointParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._geometryPoint> Instance { get; } = from _geometryPrefix_1 in __GeneratedOdata.Parsers.Rules._geometryPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _fullPointLiteral_1 in __GeneratedOdata.Parsers.Rules._fullPointLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geometryPoint(_geometryPrefix_1, _SQUOTE_1, _fullPointLiteral_1, _SQUOTE_2);
    }
    
}
