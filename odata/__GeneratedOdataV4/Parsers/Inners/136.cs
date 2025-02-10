namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                newStart = start;
                for (; newStart < start + 7; ++newStart)
                {
                    var next = input[newStart];
                }

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ> Instance { get; } = (_ʺx24x66x69x6Cx74x65x72ʺParser.Instance);
        
        public static class _ʺx24x66x69x6Cx74x65x72ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x66x69x6Cx74x65x72ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx24x66x69x6Cx74x65x72ʺParser.Instance.Parse(input);

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ.Instance, _ʺx24x66x69x6Cx74x65x72ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx66x69x6Cx74x65x72ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx66x69x6Cx74x65x72ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx66x69x6Cx74x65x72ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx66x69x6Cx74x65x72ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx66x69x6Cx74x65x72ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx66x69x6Cx74x65x72ʺParser.Instance.Parse(input);
if (!_ʺx66x69x6Cx74x65x72ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx66x69x6Cx74x65x72ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx66x69x6Cx74x65x72ʺ.Instance, _ʺx66x69x6Cx74x65x72ʺ_1.Remainder);
                }
            }
        }
    }
    
}
