namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Inners;
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    using System.Linq;

    public static class _queryOptionsParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._queryOptions> Instance { get; } = from _queryOption_1 in __GeneratedOdata.Parsers.Rules._queryOptionParser.Instance
                                                                                                       from _Ⲥʺx26ʺ_queryOptionↃ_1 in Inners._Ⲥʺx26ʺ_queryOptionↃParser.Instance.Many()
                                                                                                       select new __GeneratedOdata.CstNodes.Rules._queryOptions(_queryOption_1, _Ⲥʺx26ʺ_queryOptionↃ_1);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._queryOptions> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._queryOptions>
        {
            public IOutput<char, _queryOptions> Parse(IInput<char>? input)
            {
                var _queryOption_1 = __GeneratedOdata.Parsers.Rules._queryOptionParser.Instance.Parse(input);
                //// var _Ⲥʺx26ʺ_queryOptionↃ_1 = Inners._Ⲥʺx26ʺ_queryOptionↃParser.Instance.Many().Parse(_queryOption_1.Remainder);
                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Rules._queryOptions(_queryOption_1.Parsed, Enumerable.Empty<_Ⲥʺx26ʺ_queryOptionↃ>()),
                    _queryOption_1.Remainder);
            }
        }
    }
    
}
