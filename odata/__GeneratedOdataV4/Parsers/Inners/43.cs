namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_keyPathLiteralParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance2.Parse(input, start, out newStart);

                var _keyPathLiteral_1 = __GeneratedOdataV4.Parsers.Rules._keyPathLiteralParser.Instance2.Parse(input, newStart, out newStart);

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral)!, input);
}

var _keyPathLiteral_1 = __GeneratedOdataV4.Parsers.Rules._keyPathLiteralParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_keyPathLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral(_ʺx2Fʺ_1.Parsed, _keyPathLiteral_1.Parsed), _keyPathLiteral_1.Remainder);
            }
        }
    }
    
}
