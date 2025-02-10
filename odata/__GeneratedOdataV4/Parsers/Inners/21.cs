namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x3FParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._x3F> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._x3F>
        {
            private static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, char> Parser { get; } = CombinatorParsingV3.Parse.Char((char)0x3F);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Inners._x3F Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                Parser.Parse(input, start, out newStart);

                /*newStart = start;

                for (; newStart < start + 1; ++newStart)
                {
                    var next = input[newStart];
                }*/

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x3F> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x3F>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x3F> Parse(IInput<char>? input)
            {
                var _x3F = CombinatorParsingV2.Parse.Char((char)0x3F).Parse(input);
if (!_x3F.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x3F)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x3F.Instance, _x3F.Remainder);
            }
        }
    }
    
}
