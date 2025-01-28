namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _filterParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._filter> Instance { get; } = from _Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _boolCommonExpr_1 in __GeneratedOdata.Parsers.Rules._boolCommonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._filter(_Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ_1, _EQ_1, _boolCommonExpr_1);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._filter> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._filter>
        {
            public IOutput<char, _filter> Parse(IInput<char>? input)
            {
                var _Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ_1 = __GeneratedOdata.Parsers.Inners._Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃParser.Instance.Parse(input);
                var _EQ_1 = __GeneratedOdata.Parsers.Rules._EQParser.Instance.Parse(_Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ_1.Remainder);
                var _boolCommonExpr_1 = __GeneratedOdata.Parsers.Rules._boolCommonExprParser.Instance.Parse(_EQ_1.Remainder);
                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Rules._filter(_Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ_1.Parsed, _EQ_1.Parsed, _boolCommonExpr_1.Parsed),
                    _boolCommonExpr_1.Remainder);
            }
        }
    }
    
}
