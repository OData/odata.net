namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boolCommonExprParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._filter> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._filter>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Rules._filter Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                newStart = start;
                for (; newStart < start + 19; ++newStart)
                {
                    var next = input[newStart];
                }

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._boolCommonExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._boolCommonExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._boolCommonExpr> Parse(IInput<char>? input)
            {
                var _commonExpr_1 = __GeneratedOdataV4.Parsers.Rules._commonExprParser.Instance.Parse(input);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._boolCommonExpr(_commonExpr_1.Parsed), _commonExpr_1.Remainder);
            }
        }
    }
    
}
