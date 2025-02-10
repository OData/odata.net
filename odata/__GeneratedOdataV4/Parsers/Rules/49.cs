namespace __GeneratedOdataV4.Parsers.Rules
{
    using System.Linq;

    using __GeneratedOdataV4.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _queryOptionsParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._queryOptions> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._queryOptions>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public _queryOptions Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                newStart = start;
                for (; newStart < start + 27; ++newStart)
                {
                    var next = input[newStart];
                }

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._queryOptions> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._queryOptions>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._queryOptions> Parse(IInput<char>? input)
            {
                var _queryOption_1 = __GeneratedOdataV4.Parsers.Rules._queryOptionParser.Instance.Parse(input);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._queryOptions(_queryOption_1.Parsed, System.Linq.Enumerable.Empty<__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx26ʺ_queryOptionↃ>()), _queryOption_1.Remainder);
            }
        }
    }
    
}
