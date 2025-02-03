namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fullMultiPolygonLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._fullMultiPolygonLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._fullMultiPolygonLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._fullMultiPolygonLiteral> Parse(IInput<char>? input)
            {
                var _sridLiteral_1 = __GeneratedOdataV3.Parsers.Rules._sridLiteralParser.Instance.Parse(input);
if (!_sridLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._fullMultiPolygonLiteral)!, input);
}

var _multiPolygonLiteral_1 = __GeneratedOdataV3.Parsers.Rules._multiPolygonLiteralParser.Instance.Parse(_sridLiteral_1.Remainder);
if (!_multiPolygonLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._fullMultiPolygonLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._fullMultiPolygonLiteral(_sridLiteral_1.Parsed,  _multiPolygonLiteral_1.Parsed), _multiPolygonLiteral_1.Remainder);
            }
        }
    }
    
}
