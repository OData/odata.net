namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _orderbyItemParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._orderbyItem> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._orderbyItem>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._orderbyItem> Parse(IInput<char>? input)
            {
                var _commonExpr_1 = __GeneratedOdataV3.Parsers.Rules._commonExprParser.Instance.Parse(input);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._orderbyItem)!, input);
}

var _RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃParser.Instance.Optional().Parse(_commonExpr_1.Remainder);
if (!_RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._orderbyItem)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._orderbyItem(_commonExpr_1.Parsed, _RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ_1.Parsed.GetOrElse(null)), _RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ_1.Remainder);
            }
        }
    }
    
}
