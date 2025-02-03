namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fullMultiLineStringLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._fullMultiLineStringLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._fullMultiLineStringLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._fullMultiLineStringLiteral> Parse(IInput<char>? input)
            {
                var _sridLiteral_1 = __GeneratedOdataV3.Parsers.Rules._sridLiteralParser.Instance.Parse(input);
if (!_sridLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._fullMultiLineStringLiteral)!, input);
}

var _multiLineStringLiteral_1 = __GeneratedOdataV3.Parsers.Rules._multiLineStringLiteralParser.Instance.Parse(_sridLiteral_1.Remainder);
if (!_multiLineStringLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._fullMultiLineStringLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._fullMultiLineStringLiteral(_sridLiteral_1.Parsed,  _multiLineStringLiteral_1.Parsed), _multiLineStringLiteral_1.Remainder);
            }
        }
    }
    
}
