namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fullPolygonLiteralParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._fullPolygonLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._fullPolygonLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._fullPolygonLiteral> Parse(IInput<char>? input)
            {
                var _sridLiteral_1 = __GeneratedOdataV4.Parsers.Rules._sridLiteralParser.Instance.Parse(input);
if (!_sridLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._fullPolygonLiteral)!, input);
}

var _polygonLiteral_1 = __GeneratedOdataV4.Parsers.Rules._polygonLiteralParser.Instance.Parse(_sridLiteral_1.Remainder);
if (!_polygonLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._fullPolygonLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._fullPolygonLiteral(_sridLiteral_1.Parsed, _polygonLiteral_1.Parsed), _polygonLiteral_1.Remainder);
            }
        }
    }
    
}
