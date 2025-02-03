namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_keyPathLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral)!, input);
}

var _keyPathLiteral_1 = __GeneratedOdataV3.Parsers.Rules._keyPathLiteralParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_keyPathLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral(_ʺx2Fʺ_1.Parsed,  _keyPathLiteral_1.Parsed), _keyPathLiteral_1.Remainder);
            }
        }
    }
    
}
