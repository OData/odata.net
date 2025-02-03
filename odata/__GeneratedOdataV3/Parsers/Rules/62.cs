namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _computeItemParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._computeItem> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._computeItem>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._computeItem> Parse(IInput<char>? input)
            {
                var _commonExpr_1 = __GeneratedOdataV3.Parsers.Rules._commonExprParser.Instance.Parse(input);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._computeItem)!, input);
}

var _RWS_1 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(_commonExpr_1.Remainder);
if (!_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._computeItem)!, input);
}

var _ʺx61x73ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx61x73ʺParser.Instance.Parse(_RWS_1.Remainder);
if (!_ʺx61x73ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._computeItem)!, input);
}

var _RWS_2 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(_ʺx61x73ʺ_1.Remainder);
if (!_RWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._computeItem)!, input);
}

var _computedProperty_1 = __GeneratedOdataV3.Parsers.Rules._computedPropertyParser.Instance.Parse(_RWS_2.Remainder);
if (!_computedProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._computeItem)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._computeItem(_commonExpr_1.Parsed, _RWS_1.Parsed, _ʺx61x73ʺ_1.Parsed, _RWS_2.Parsed,  _computedProperty_1.Parsed), _computedProperty_1.Remainder);
            }
        }
    }
    
}
