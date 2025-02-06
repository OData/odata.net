namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fullCollectionLiteralParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._fullCollectionLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._fullCollectionLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._fullCollectionLiteral> Parse(IInput<char>? input)
            {
                var _sridLiteral_1 = __GeneratedOdataV4.Parsers.Rules._sridLiteralParser.Instance.Parse(input);
if (!_sridLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._fullCollectionLiteral)!, input);
}

var _collectionLiteral_1 = __GeneratedOdataV4.Parsers.Rules._collectionLiteralParser.Instance.Parse(_sridLiteral_1.Remainder);
if (!_collectionLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._fullCollectionLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._fullCollectionLiteral(_sridLiteral_1.Parsed, _collectionLiteral_1.Parsed), _collectionLiteral_1.Remainder);
            }
        }
    }
    
}
