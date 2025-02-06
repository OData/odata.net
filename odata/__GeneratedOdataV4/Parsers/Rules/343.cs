namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geometryPolygonParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._geometryPolygon> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._geometryPolygon>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._geometryPolygon> Parse(IInput<char>? input)
            {
                var _geometryPrefix_1 = __GeneratedOdataV4.Parsers.Rules._geometryPrefixParser.Instance.Parse(input);
if (!_geometryPrefix_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geometryPolygon)!, input);
}

var _SQUOTE_1 = __GeneratedOdataV4.Parsers.Rules._SQUOTEParser.Instance.Parse(_geometryPrefix_1.Remainder);
if (!_SQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geometryPolygon)!, input);
}

var _fullPolygonLiteral_1 = __GeneratedOdataV4.Parsers.Rules._fullPolygonLiteralParser.Instance.Parse(_SQUOTE_1.Remainder);
if (!_fullPolygonLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geometryPolygon)!, input);
}

var _SQUOTE_2 = __GeneratedOdataV4.Parsers.Rules._SQUOTEParser.Instance.Parse(_fullPolygonLiteral_1.Remainder);
if (!_SQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geometryPolygon)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._geometryPolygon(_geometryPrefix_1.Parsed, _SQUOTE_1.Parsed, _fullPolygonLiteral_1.Parsed, _SQUOTE_2.Parsed), _SQUOTE_2.Remainder);
            }
        }
    }
    
}
