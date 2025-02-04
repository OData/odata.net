namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geographyCollectionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._geographyCollection> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._geographyCollection>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._geographyCollection> Parse(IInput<char>? input)
            {
                var _geographyPrefix_1 = __GeneratedOdataV3.Parsers.Rules._geographyPrefixParser.Instance.Parse(input);
if (!_geographyPrefix_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyCollection)!, input);
}

var _SQUOTE_1 = __GeneratedOdataV3.Parsers.Rules._SQUOTEParser.Instance.Parse(_geographyPrefix_1.Remainder);
if (!_SQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyCollection)!, input);
}

var _fullCollectionLiteral_1 = __GeneratedOdataV3.Parsers.Rules._fullCollectionLiteralParser.Instance.Parse(_SQUOTE_1.Remainder);
if (!_fullCollectionLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyCollection)!, input);
}

var _SQUOTE_2 = __GeneratedOdataV3.Parsers.Rules._SQUOTEParser.Instance.Parse(_fullCollectionLiteral_1.Remainder);
if (!_SQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geographyCollection)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._geographyCollection(_geographyPrefix_1.Parsed, _SQUOTE_1.Parsed, _fullCollectionLiteral_1.Parsed, _SQUOTE_2.Parsed), _SQUOTE_2.Remainder);
            }
        }
    }
    
}
