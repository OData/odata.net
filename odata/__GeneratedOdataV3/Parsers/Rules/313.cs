namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geographyLineStringParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._geographyLineString> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._geographyLineString>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._geographyLineString> Parse(IInput<char>? input)
            {
                var _geographyPrefix_1 = __GeneratedOdataV3.Parsers.Rules._geographyPrefixParser.Instance.Parse(input);
if (!_geographyPrefix_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyLineString)!, input);
}

var _SQUOTE_1 = __GeneratedOdataV3.Parsers.Rules._SQUOTEParser.Instance.Parse(_geographyPrefix_1.Remainder);
if (!_SQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyLineString)!, input);
}

var _fullLineStringLiteral_1 = __GeneratedOdataV3.Parsers.Rules._fullLineStringLiteralParser.Instance.Parse(_SQUOTE_1.Remainder);
if (!_fullLineStringLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyLineString)!, input);
}

var _SQUOTE_2 = __GeneratedOdataV3.Parsers.Rules._SQUOTEParser.Instance.Parse(_fullLineStringLiteral_1.Remainder);
if (!_SQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyLineString)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._geographyLineString(_geographyPrefix_1.Parsed, _SQUOTE_1.Parsed, _fullLineStringLiteral_1.Parsed,  _SQUOTE_2.Parsed), _SQUOTE_2.Remainder);
            }
        }
    }
    
}
