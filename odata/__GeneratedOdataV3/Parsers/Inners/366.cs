namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _rootExpr_ЖⲤvalueⲻseparator_rootExprↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ> Parse(IInput<char>? input)
            {
                var _rootExpr_1 = __GeneratedOdataV3.Parsers.Rules._rootExprParser.Instance.Parse(input);
if (!_rootExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ)!, input);
}

var _Ⲥvalueⲻseparator_rootExprↃ_1 = Inners._Ⲥvalueⲻseparator_rootExprↃParser.Instance.Many().Parse(_rootExpr_1.Remainder);
if (!_Ⲥvalueⲻseparator_rootExprↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ(_rootExpr_1.Parsed, _Ⲥvalueⲻseparator_rootExprↃ_1.Parsed), _Ⲥvalueⲻseparator_rootExprↃ_1.Remainder);
            }
        }
    }
    
}
