namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx65x71ʺParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._ʺx65x71ʺ> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._ʺx65x71ʺ>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Inners._ʺx65x71ʺ Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance2.Parse(input, start, out newStart);

                var _x71_1 = __GeneratedOdataV4.Parsers.Inners._x71Parser.Instance2.Parse(input, newStart, out newStart);

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx65x71ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx65x71ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx65x71ʺ> Parse(IInput<char>? input)
            {
                var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(input);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx65x71ʺ)!, input);
}

var _x71_1 = __GeneratedOdataV4.Parsers.Inners._x71Parser.Instance.Parse(_x65_1.Remainder);
if (!_x71_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx65x71ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx65x71ʺ.Instance, _x71_1.Remainder);
            }
        }
    }
    
}
