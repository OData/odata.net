namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x71Parser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._x71> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._x71>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Inners._x71 Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                var _x71 = CombinatorParsingV3.Parse.Char((char)0x71).Parse(input, start, out newStart);

                return default;
            }
        }
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x71> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x71>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x71> Parse(IInput<char>? input)
            {
                var _x71 = CombinatorParsingV2.Parse.Char((char)0x71).Parse(input);
if (!_x71.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x71)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x71.Instance, _x71.Remainder);
            }
        }
    }
    
}
