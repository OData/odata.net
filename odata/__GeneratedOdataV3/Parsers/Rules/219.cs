namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _numberInJSONParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._numberInJSON> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._numberInJSON>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._numberInJSON> Parse(IInput<char>? input)
            {
                var _ʺx2Dʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2DʺParser.Instance.Optional().Parse(input);
if (!_ʺx2Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._numberInJSON)!, input);
}

var _int_1 = __GeneratedOdataV3.Parsers.Rules._intParser.Instance.Parse(_ʺx2Dʺ_1.Remainder);
if (!_int_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._numberInJSON)!, input);
}

var _frac_1 = __GeneratedOdataV3.Parsers.Rules._fracParser.Instance.Optional().Parse(_int_1.Remainder);
if (!_frac_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._numberInJSON)!, input);
}

var _exp_1 = __GeneratedOdataV3.Parsers.Rules._expParser.Instance.Optional().Parse(_frac_1.Remainder);
if (!_exp_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._numberInJSON)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._numberInJSON(_ʺx2Dʺ_1.Parsed.GetOrElse(null), _int_1.Parsed, _frac_1.Parsed.GetOrElse(null),  _exp_1.Parsed.GetOrElse(null)), _exp_1.Remainder);
            }
        }
    }
    
}
