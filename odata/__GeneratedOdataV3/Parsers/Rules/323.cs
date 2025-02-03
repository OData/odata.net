namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geographyMultiPolygonParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._geographyMultiPolygon> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._geographyMultiPolygon>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._geographyMultiPolygon> Parse(IInput<char>? input)
            {
                var _geographyPrefix_1 = __GeneratedOdataV3.Parsers.Rules._geographyPrefixParser.Instance.Parse(input);
if (!_geographyPrefix_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyMultiPolygon)!, input);
}

var _SQUOTE_1 = __GeneratedOdataV3.Parsers.Rules._SQUOTEParser.Instance.Parse(_geographyPrefix_1.Remainder);
if (!_SQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyMultiPolygon)!, input);
}

var _fullMultiPolygonLiteral_1 = __GeneratedOdataV3.Parsers.Rules._fullMultiPolygonLiteralParser.Instance.Parse(_SQUOTE_1.Remainder);
if (!_fullMultiPolygonLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyMultiPolygon)!, input);
}

var _SQUOTE_2 = __GeneratedOdataV3.Parsers.Rules._SQUOTEParser.Instance.Parse(_fullMultiPolygonLiteral_1.Remainder);
if (!_SQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyMultiPolygon)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._geographyMultiPolygon(_geographyPrefix_1.Parsed, _SQUOTE_1.Parsed, _fullMultiPolygonLiteral_1.Parsed,  _SQUOTE_2.Parsed), _SQUOTE_2.Remainder);
            }
        }
    }
    
}
