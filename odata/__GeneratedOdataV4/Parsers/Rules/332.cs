namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geographyPolygonParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._geographyPolygon> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._geographyPolygon>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._geographyPolygon> Parse(IInput<char>? input)
            {
                var _geographyPrefix_1 = __GeneratedOdataV4.Parsers.Rules._geographyPrefixParser.Instance.Parse(input);
if (!_geographyPrefix_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geographyPolygon)!, input);
}

var _SQUOTE_1 = __GeneratedOdataV4.Parsers.Rules._SQUOTEParser.Instance.Parse(_geographyPrefix_1.Remainder);
if (!_SQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geographyPolygon)!, input);
}

var _fullPolygonLiteral_1 = __GeneratedOdataV4.Parsers.Rules._fullPolygonLiteralParser.Instance.Parse(_SQUOTE_1.Remainder);
if (!_fullPolygonLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geographyPolygon)!, input);
}

var _SQUOTE_2 = __GeneratedOdataV4.Parsers.Rules._SQUOTEParser.Instance.Parse(_fullPolygonLiteral_1.Remainder);
if (!_SQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geographyPolygon)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._geographyPolygon(_geographyPrefix_1.Parsed, _SQUOTE_1.Parsed, _fullPolygonLiteral_1.Parsed, _SQUOTE_2.Parsed), _SQUOTE_2.Remainder);
            }
        }
    }
    
}
