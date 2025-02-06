namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geographyMultiLineStringParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._geographyMultiLineString> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._geographyMultiLineString>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._geographyMultiLineString> Parse(IInput<char>? input)
            {
                var _geographyPrefix_1 = __GeneratedOdataV4.Parsers.Rules._geographyPrefixParser.Instance.Parse(input);
if (!_geographyPrefix_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geographyMultiLineString)!, input);
}

var _SQUOTE_1 = __GeneratedOdataV4.Parsers.Rules._SQUOTEParser.Instance.Parse(_geographyPrefix_1.Remainder);
if (!_SQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geographyMultiLineString)!, input);
}

var _fullMultiLineStringLiteral_1 = __GeneratedOdataV4.Parsers.Rules._fullMultiLineStringLiteralParser.Instance.Parse(_SQUOTE_1.Remainder);
if (!_fullMultiLineStringLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geographyMultiLineString)!, input);
}

var _SQUOTE_2 = __GeneratedOdataV4.Parsers.Rules._SQUOTEParser.Instance.Parse(_fullMultiLineStringLiteral_1.Remainder);
if (!_SQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geographyMultiLineString)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._geographyMultiLineString(_geographyPrefix_1.Parsed, _SQUOTE_1.Parsed, _fullMultiLineStringLiteral_1.Parsed, _SQUOTE_2.Parsed), _SQUOTE_2.Remainder);
            }
        }
    }
    
}
