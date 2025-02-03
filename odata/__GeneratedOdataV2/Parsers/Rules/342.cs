namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geometryPointParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._geometryPoint> Instance { get; } = from _geometryPrefix_1 in __GeneratedOdataV2.Parsers.Rules._geometryPrefixParser.Instance
from _SQUOTE_1 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
from _fullPointLiteral_1 in __GeneratedOdataV2.Parsers.Rules._fullPointLiteralParser.Instance
from _SQUOTE_2 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._geometryPoint(_geometryPrefix_1, _SQUOTE_1, _fullPointLiteral_1, _SQUOTE_2);
    }
    
}
