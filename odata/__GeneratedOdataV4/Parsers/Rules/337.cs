namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geometryCollectionParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._geometryCollection> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._geometryCollection>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._geometryCollection> Parse(IInput<char>? input)
            {
                var _geometryPrefix_1 = __GeneratedOdataV4.Parsers.Rules._geometryPrefixParser.Instance.Parse(input);
if (!_geometryPrefix_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geometryCollection)!, input);
}

var _SQUOTE_1 = __GeneratedOdataV4.Parsers.Rules._SQUOTEParser.Instance.Parse(_geometryPrefix_1.Remainder);
if (!_SQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geometryCollection)!, input);
}

var _fullCollectionLiteral_1 = __GeneratedOdataV4.Parsers.Rules._fullCollectionLiteralParser.Instance.Parse(_SQUOTE_1.Remainder);
if (!_fullCollectionLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geometryCollection)!, input);
}

var _SQUOTE_2 = __GeneratedOdataV4.Parsers.Rules._SQUOTEParser.Instance.Parse(_fullCollectionLiteral_1.Remainder);
if (!_SQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geometryCollection)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._geometryCollection(_geometryPrefix_1.Parsed, _SQUOTE_1.Parsed, _fullCollectionLiteral_1.Parsed, _SQUOTE_2.Parsed), _SQUOTE_2.Remainder);
            }
        }
    }
    
}
