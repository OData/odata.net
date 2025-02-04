namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _commonExpr_BWS_COMMA_BWSParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._commonExpr_BWS_COMMA_BWS> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._commonExpr_BWS_COMMA_BWS>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._commonExpr_BWS_COMMA_BWS> Parse(IInput<char>? input)
            {
                var _commonExpr_1 = __GeneratedOdataV3.Parsers.Rules._commonExprParser.Instance.Parse(input);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._commonExpr_BWS_COMMA_BWS)!, input);
}

var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_commonExpr_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._commonExpr_BWS_COMMA_BWS)!, input);
}

var _COMMA_1 = __GeneratedOdataV3.Parsers.Rules._COMMAParser.Instance.Parse(_BWS_1.Remainder);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._commonExpr_BWS_COMMA_BWS)!, input);
}

var _BWS_2 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_COMMA_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._commonExpr_BWS_COMMA_BWS)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._commonExpr_BWS_COMMA_BWS(_commonExpr_1.Parsed, _BWS_1.Parsed, _COMMA_1.Parsed, _BWS_2.Parsed), _BWS_2.Remainder);
            }
        }
    }
    
}
