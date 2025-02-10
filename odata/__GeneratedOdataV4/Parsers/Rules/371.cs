namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _RWSParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._RWS> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._RWS>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Rules._RWS Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                newStart = start;
                for (; newStart < start + 1; ++newStart)
                {
                    var next = input[newStart];
                }

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._RWS> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._RWS>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._RWS> Parse(IInput<char>? input)
            {
                var _ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ_1 = __GeneratedOdataV4.Parsers.Inners._ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃParser.Instance.Repeat(1, null).Parse(input);
if (!_ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._RWS)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._RWS(new __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV4.CstNodes.Inners._ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ>(_ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ_1.Parsed)), _ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ_1.Remainder);
            }
        }
    }
    
}
