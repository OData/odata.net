namespace __GeneratedOdataV4.Parsers.Inners
{
    using __GeneratedOdataV4.CstNodes.Inners;
    using CombinatorParsingV2;
    
    public static class _ʺx3Fʺ_queryOptionsParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ_queryOptions> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ_queryOptions>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public _ʺx3Fʺ_queryOptions Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                var _ʺx3Fʺ = __GeneratedOdataV4.Parsers.Inners._ʺx3FʺParser.Instance2.Parse(input, start, out newStart);

                /*var _queryOptions = __GeneratedOdataV4.Parsers.Rules._queryOptionsParser.Instance2.Parse(_ʺx3Fʺ.Remainder);*/

                newStart = start;
                for (; newStart < start + 28; ++newStart)
                {
                    var next = input[newStart];
                }

                return default;
            }
        }


        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ_queryOptions> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ_queryOptions>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ_queryOptions> Parse(IInput<char>? input)
            {
                var _ʺx3Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3FʺParser.Instance.Parse(input);

var _queryOptions_1 = __GeneratedOdataV4.Parsers.Rules._queryOptionsParser.Instance.Parse(_ʺx3Fʺ_1.Remainder);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ_queryOptions(_ʺx3Fʺ_1.Parsed, _queryOptions_1.Parsed), _queryOptions_1.Remainder);
            }
        }
    }
    
}
