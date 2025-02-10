namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x6CParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._x6C> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._x6C>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Inners._x6C Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                var _x24 = CombinatorParsingV3.Parse.Char((char)0x6C).Parse(input, start, out newStart);

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x6C> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x6C>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x6C> Parse(IInput<char>? input)
            {
                var _x6C = CombinatorParsingV2.Parse.Char((char)0x6C).Parse(input);
if (!_x6C.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x6C)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x6C.Instance, _x6C.Remainder);
            }
        }
    }
    
}
