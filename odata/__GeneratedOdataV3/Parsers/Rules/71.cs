namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _filterParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._filter> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._filter>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._filter> Parse(IInput<char>? input)
            {
                var _Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃParser.Instance.Parse(input);

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ_1.Remainder);

var _boolCommonExpr_1 = __GeneratedOdataV3.Parsers.Rules._boolCommonExprParser.Instance.Parse(_EQ_1.Remainder);

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._filter(_Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ_1.Parsed, _EQ_1.Parsed, _boolCommonExpr_1.Parsed), _boolCommonExpr_1.Remainder);
            }
        }
    }
    
}
