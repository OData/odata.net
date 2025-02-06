namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _valueⲻseparator_rootExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_rootExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_rootExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_rootExpr> Parse(IInput<char>? input)
            {
                var _valueⲻseparator_1 = __GeneratedOdataV4.Parsers.Rules._valueⲻseparatorParser.Instance.Parse(input);
if (!_valueⲻseparator_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_rootExpr)!, input);
}

var _rootExpr_1 = __GeneratedOdataV4.Parsers.Rules._rootExprParser.Instance.Parse(_valueⲻseparator_1.Remainder);
if (!_rootExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_rootExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_rootExpr(_valueⲻseparator_1.Parsed, _rootExpr_1.Parsed), _rootExpr_1.Remainder);
            }
        }
    }
    
}
