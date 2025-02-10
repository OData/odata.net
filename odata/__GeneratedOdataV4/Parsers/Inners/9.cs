namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x2FParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                var _x2F = CombinatorParsingV3.Parse.Char((char)0x2F).Parse(input, start, out newStart);

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x2F> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x2F>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x2F> Parse(IInput<char>? input)
            {
                var _x2F = CombinatorParsingV2.Parse.Char((char)0x2F).Parse(input);
if (!_x2F.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x2F)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x2F.Instance, _x2F.Remainder);
            }
        }
    }
    
}
