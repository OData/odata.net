namespace __GeneratedOdata.Parsers.Inners
{
    using __GeneratedOdata.CstNodes.Inners;
    using CombinatorParsingV2;
    
    public static class _ʺx3Fʺ_queryOptionsParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx3Fʺ_queryOptions> Instance { get; } = from _ʺx3Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3FʺParser.Instance
from _queryOptions_1 in __GeneratedOdata.Parsers.Rules._queryOptionsParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx3Fʺ_queryOptions(_ʺx3Fʺ_1, _queryOptions_1);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx3Fʺ_queryOptions> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx3Fʺ_queryOptions>
        {
            public IOutput<char, _ʺx3Fʺ_queryOptions> Parse(IInput<char>? input)
            {
                var _ʺx3Fʺ_1 = __GeneratedOdata.Parsers.Inners._ʺx3FʺParser.Instance.Parse(input);
                var _queryOptions_1 = __GeneratedOdata.Parsers.Rules._queryOptionsParser.Instance.Parse(_ʺx3Fʺ_1.Remainder);
                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Inners._ʺx3Fʺ_queryOptions(_ʺx3Fʺ_1.Parsed, _queryOptions_1.Parsed),
                    _queryOptions_1.Remainder);
            }
        }
    }
    
}
