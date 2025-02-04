namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _odataⲻmaxversionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataⲻmaxversion> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataⲻmaxversion>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._odataⲻmaxversion> Parse(IInput<char>? input)
            {
                var _ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6EʺParser.Instance.Parse(input);
if (!_ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataⲻmaxversion)!, input);
}

var _ʺx3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3AʺParser.Instance.Parse(_ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ_1.Remainder);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataⲻmaxversion)!, input);
}

var _OWS_1 = __GeneratedOdataV3.Parsers.Rules._OWSParser.Instance.Parse(_ʺx3Aʺ_1.Remainder);
if (!_OWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataⲻmaxversion)!, input);
}

var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Repeat(1, null).Parse(_OWS_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataⲻmaxversion)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_DIGIT_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataⲻmaxversion)!, input);
}

var _DIGIT_2 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Repeat(1, null).Parse(_ʺx2Eʺ_1.Remainder);
if (!_DIGIT_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataⲻmaxversion)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._odataⲻmaxversion(_ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ_1.Parsed, _ʺx3Aʺ_1.Parsed, _OWS_1.Parsed, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed), _ʺx2Eʺ_1.Parsed, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Rules._DIGIT>(_DIGIT_2.Parsed)), _DIGIT_2.Remainder);
            }
        }
    }
    
}
