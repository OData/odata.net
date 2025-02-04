namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geographyPointParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._geographyPoint> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._geographyPoint>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._geographyPoint> Parse(IInput<char>? input)
            {
                var _geographyPrefix_1 = __GeneratedOdataV3.Parsers.Rules._geographyPrefixParser.Instance.Parse(input);
if (!_geographyPrefix_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyPoint)!, input);
}

var _SQUOTE_1 = __GeneratedOdataV3.Parsers.Rules._SQUOTEParser.Instance.Parse(_geographyPrefix_1.Remainder);
if (!_SQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyPoint)!, input);
}

var _fullPointLiteral_1 = __GeneratedOdataV3.Parsers.Rules._fullPointLiteralParser.Instance.Parse(_SQUOTE_1.Remainder);
if (!_fullPointLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyPoint)!, input);
}

var _SQUOTE_2 = __GeneratedOdataV3.Parsers.Rules._SQUOTEParser.Instance.Parse(_fullPointLiteral_1.Remainder);
if (!_SQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyPoint)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._geographyPoint(_geographyPrefix_1.Parsed, _SQUOTE_1.Parsed, _fullPointLiteral_1.Parsed, _SQUOTE_2.Parsed), _SQUOTE_2.Remainder);
            }
        }
    }
    
}
