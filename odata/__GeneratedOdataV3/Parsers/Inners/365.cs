namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥvalueⲻseparator_rootExprↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥvalueⲻseparator_rootExprↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥvalueⲻseparator_rootExprↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥvalueⲻseparator_rootExprↃ> Parse(IInput<char>? input)
            {
                var _valueⲻseparator_rootExpr_1 = __GeneratedOdataV3.Parsers.Inners._valueⲻseparator_rootExprParser.Instance.Parse(input);
if (!_valueⲻseparator_rootExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥvalueⲻseparator_rootExprↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥvalueⲻseparator_rootExprↃ(_valueⲻseparator_rootExpr_1.Parsed), _valueⲻseparator_rootExpr_1.Remainder);
            }
        }
    }
    
}
