namespace __GeneratedOdataV4.Parsers.Inners
{
    using __GeneratedOdataV4.CstNodes.Inners;
    using CombinatorParsingV2;
    
    public static class _ʺx3Fʺ_queryOptionsParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ_queryOptions, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ_queryOptions>> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ_queryOptions, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ_queryOptions>>
        {
            public CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _ʺx3Fʺ_queryOptions> Parse(in CombinatorParsingV3.StringInput input)
            {
                //// TODO try returning defaults from all of the parsers to see if it's the node allocations

                var _ʺx3Fʺ = __GeneratedOdataV4.Parsers.Inners._ʺx3FʺParser.Instance2.Parse(input);

                var _queryOptions = __GeneratedOdataV4.Parsers.Rules._queryOptionsParser.Instance2.Parse(_ʺx3Fʺ.Remainder);


                return new CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _ʺx3Fʺ_queryOptions>(
                    true,
                    default,
                    _queryOptions.HasRemainder,
                    _queryOptions.Remainder);
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
