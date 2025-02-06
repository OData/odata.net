namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _eqExprParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._eqExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx65x71ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx65x71ʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._eqExpr(_RWS_1, _ʺx65x71ʺ_1, _RWS_2, _commonExpr_1);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._eqExpr> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._eqExpr>
        {
            public IOutput<char, _eqExpr> Parse(IInput<char>? input)
            {
                var _RWS_1 = __GeneratedOdata.Parsers.Rules._RWSParser.Instance.Parse(input);
                if (!_RWS_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_eqExpr)!,
                        input);
                }

                var _ʺx65x71ʺ_1 = __GeneratedOdata.Parsers.Inners._ʺx65x71ʺParser.Instance.Parse(_RWS_1.Remainder);
                if (!_ʺx65x71ʺ_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_eqExpr)!,
                        input);
                }

                var _RWS_2 = __GeneratedOdata.Parsers.Rules._RWSParser.Instance.Parse(_ʺx65x71ʺ_1.Remainder);
                if (!_RWS_2.Success)
                {
                    return Output.Create(
                        false,
                        default(_eqExpr)!,
                        input);
                }

                var _commonExpr_1 = __GeneratedOdata.Parsers.Rules._commonExprParser.Instance.Parse(_RWS_2.Remainder);
                if (!_commonExpr_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_eqExpr)!,
                        input);
                }

                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Rules._eqExpr(_RWS_1.Parsed, _ʺx65x71ʺ_1.Parsed, _RWS_2.Parsed, _commonExpr_1.Parsed),
                    _commonExpr_1.Remainder);
            }
        }
    }
    
}
