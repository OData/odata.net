namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _filterParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._filter> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._filter>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Rules._filter Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                newStart = start;
                for (; newStart < start + 27; ++newStart)
                {
                    var next = input[newStart];
                }

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._filter> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._filter>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._filter> Parse(IInput<char>? input)
            {
                var _Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ_1 = __GeneratedOdataV4.Parsers.Inners._Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃParser.Instance.Parse(input);

var _EQ_1 = __GeneratedOdataV4.Parsers.Rules._EQParser.Instance.Parse(_Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ_1.Remainder);

var _boolCommonExpr_1 = __GeneratedOdataV4.Parsers.Rules._boolCommonExprParser.Instance.Parse(_EQ_1.Remainder);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._filter(_Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ_1.Parsed, _EQ_1.Parsed, _boolCommonExpr_1.Parsed), _boolCommonExpr_1.Remainder);
            }
        }
    }
    
}
