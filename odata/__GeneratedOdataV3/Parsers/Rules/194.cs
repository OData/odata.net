namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _castExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._castExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._castExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._castExpr> Parse(IInput<char>? input)
            {
                var _ʺx63x61x73x74ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx63x61x73x74ʺParser.Instance.Parse(input);
if (!_ʺx63x61x73x74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._castExpr)!, input);
}

var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(_ʺx63x61x73x74ʺ_1.Remainder);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._castExpr)!, input);
}

var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_OPEN_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._castExpr)!, input);
}

var _commonExpr_BWS_COMMA_BWS_1 = __GeneratedOdataV3.Parsers.Inners._commonExpr_BWS_COMMA_BWSParser.Instance.Optional().Parse(_BWS_1.Remainder);
if (!_commonExpr_BWS_COMMA_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._castExpr)!, input);
}

var _qualifiedTypeName_1 = __GeneratedOdataV3.Parsers.Rules._qualifiedTypeNameParser.Instance.Parse(_commonExpr_BWS_COMMA_BWS_1.Remainder);
if (!_qualifiedTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._castExpr)!, input);
}

var _BWS_2 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_qualifiedTypeName_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._castExpr)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_BWS_2.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._castExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._castExpr(_ʺx63x61x73x74ʺ_1.Parsed, _OPEN_1.Parsed, _BWS_1.Parsed, _commonExpr_BWS_COMMA_BWS_1.Parsed.GetOrElse(null), _qualifiedTypeName_1.Parsed, _BWS_2.Parsed,  _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
