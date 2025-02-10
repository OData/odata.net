namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _eqExprParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._eqExpr> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._eqExpr>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Rules._eqExpr Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                var _RWS_1 = __GeneratedOdataV4.Parsers.Rules._RWSParser.Instance2.Parse(input, start, out newStart);

                var _ʺx65x71ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx65x71ʺParser.Instance2.Parse(input, newStart, out newStart);

                var _RWS_2 = __GeneratedOdataV4.Parsers.Rules._RWSParser.Instance2.Parse(input, newStart, out newStart);

                var _commonExpr_1 = __GeneratedOdataV4.Parsers.Rules._commonExprParser.Instance2.Parse(input, newStart, out newStart);

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._eqExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._eqExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._eqExpr> Parse(IInput<char>? input)
            {
                var _RWS_1 = __GeneratedOdataV4.Parsers.Rules._RWSParser.Instance.Parse(input);
if (!_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._eqExpr)!, input);
}

var _ʺx65x71ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx65x71ʺParser.Instance.Parse(_RWS_1.Remainder);
if (!_ʺx65x71ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._eqExpr)!, input);
}

var _RWS_2 = __GeneratedOdataV4.Parsers.Rules._RWSParser.Instance.Parse(_ʺx65x71ʺ_1.Remainder);
if (!_RWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._eqExpr)!, input);
}

var _commonExpr_1 = __GeneratedOdataV4.Parsers.Rules._commonExprParser.Instance.Parse(_RWS_2.Remainder);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._eqExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._eqExpr(_RWS_1.Parsed, _ʺx65x71ʺ_1.Parsed, _RWS_2.Parsed, _commonExpr_1.Parsed), _commonExpr_1.Remainder);
            }
        }
    }
    
}
