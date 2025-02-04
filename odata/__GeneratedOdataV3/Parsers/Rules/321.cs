namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fullMultiPointLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._fullMultiPointLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._fullMultiPointLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._fullMultiPointLiteral> Parse(IInput<char>? input)
            {
                var _sridLiteral_1 = __GeneratedOdataV3.Parsers.Rules._sridLiteralParser.Instance.Parse(input);
if (!_sridLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._fullMultiPointLiteral)!, input);
}

var _multiPointLiteral_1 = __GeneratedOdataV3.Parsers.Rules._multiPointLiteralParser.Instance.Parse(_sridLiteral_1.Remainder);
if (!_multiPointLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._fullMultiPointLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._fullMultiPointLiteral(_sridLiteral_1.Parsed, _multiPointLiteral_1.Parsed), _multiPointLiteral_1.Remainder);
            }
        }
    }
    
}
