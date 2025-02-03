namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _rootExprColParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._rootExprCol> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._rootExprCol>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._rootExprCol> Parse(IInput<char>? input)
            {
                var _beginⲻarray_1 = __GeneratedOdataV3.Parsers.Rules._beginⲻarrayParser.Instance.Parse(input);
if (!_beginⲻarray_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._rootExprCol)!, input);
}

var _rootExpr_ЖⲤvalueⲻseparator_rootExprↃ_1 = __GeneratedOdataV3.Parsers.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃParser.Instance.Optional().Parse(_beginⲻarray_1.Remainder);
if (!_rootExpr_ЖⲤvalueⲻseparator_rootExprↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._rootExprCol)!, input);
}

var _endⲻarray_1 = __GeneratedOdataV3.Parsers.Rules._endⲻarrayParser.Instance.Parse(_rootExpr_ЖⲤvalueⲻseparator_rootExprↃ_1.Remainder);
if (!_endⲻarray_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._rootExprCol)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._rootExprCol(_beginⲻarray_1.Parsed, _rootExpr_ЖⲤvalueⲻseparator_rootExprↃ_1.Parsed.GetOrElse(null),  _endⲻarray_1.Parsed), _endⲻarray_1.Remainder);
            }
        }
    }
    
}
