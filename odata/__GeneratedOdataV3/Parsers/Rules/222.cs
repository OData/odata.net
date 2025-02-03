namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _expParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._exp> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._exp>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._exp> Parse(IInput<char>? input)
            {
                var _ʺx65ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx65ʺParser.Instance.Parse(input);
if (!_ʺx65ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._exp)!, input);
}

var _ʺx2DʺⳆʺx2Bʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2DʺⳆʺx2BʺParser.Instance.Optional().Parse(_ʺx65ʺ_1.Remainder);
if (!_ʺx2DʺⳆʺx2Bʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._exp)!, input);
}

var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Repeat(1, null).Parse(_ʺx2DʺⳆʺx2Bʺ_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._exp)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._exp(_ʺx65ʺ_1.Parsed, _ʺx2DʺⳆʺx2Bʺ_1.Parsed.GetOrElse(null),  new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed)), _DIGIT_1.Remainder);
            }
        }
    }
    
}
