namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geometryLineStringParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._geometryLineString> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._geometryLineString>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._geometryLineString> Parse(IInput<char>? input)
            {
                var _geometryPrefix_1 = __GeneratedOdataV3.Parsers.Rules._geometryPrefixParser.Instance.Parse(input);
if (!_geometryPrefix_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geometryLineString)!, input);
}

var _SQUOTE_1 = __GeneratedOdataV3.Parsers.Rules._SQUOTEParser.Instance.Parse(_geometryPrefix_1.Remainder);
if (!_SQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geometryLineString)!, input);
}

var _fullLineStringLiteral_1 = __GeneratedOdataV3.Parsers.Rules._fullLineStringLiteralParser.Instance.Parse(_SQUOTE_1.Remainder);
if (!_fullLineStringLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geometryLineString)!, input);
}

var _SQUOTE_2 = __GeneratedOdataV3.Parsers.Rules._SQUOTEParser.Instance.Parse(_fullLineStringLiteral_1.Remainder);
if (!_SQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geometryLineString)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._geometryLineString(_geometryPrefix_1.Parsed, _SQUOTE_1.Parsed, _fullLineStringLiteral_1.Parsed,  _SQUOTE_2.Parsed), _SQUOTE_2.Remainder);
            }
        }
    }
    
}
