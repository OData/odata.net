namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _odataⲻversionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataⲻversion> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataⲻversion>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._odataⲻversion> Parse(IInput<char>? input)
            {
                var _ʺx4Fx44x61x74x61x2Dx56x65x72x73x69x6Fx6Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx4Fx44x61x74x61x2Dx56x65x72x73x69x6Fx6EʺParser.Instance.Parse(input);
if (!_ʺx4Fx44x61x74x61x2Dx56x65x72x73x69x6Fx6Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataⲻversion)!, input);
}

var _ʺx3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3AʺParser.Instance.Parse(_ʺx4Fx44x61x74x61x2Dx56x65x72x73x69x6Fx6Eʺ_1.Remainder);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataⲻversion)!, input);
}

var _OWS_1 = __GeneratedOdataV3.Parsers.Rules._OWSParser.Instance.Parse(_ʺx3Aʺ_1.Remainder);
if (!_OWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataⲻversion)!, input);
}

var _ʺx34x2Ex30ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx34x2Ex30ʺParser.Instance.Parse(_OWS_1.Remainder);
if (!_ʺx34x2Ex30ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataⲻversion)!, input);
}

var _oneToNine_1 = __GeneratedOdataV3.Parsers.Rules._oneToNineParser.Instance.Optional().Parse(_ʺx34x2Ex30ʺ_1.Remainder);
if (!_oneToNine_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataⲻversion)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._odataⲻversion(_ʺx4Fx44x61x74x61x2Dx56x65x72x73x69x6Fx6Eʺ_1.Parsed, _ʺx3Aʺ_1.Parsed, _OWS_1.Parsed, _ʺx34x2Ex30ʺ_1.Parsed,  _oneToNine_1.Parsed.GetOrElse(null)), _oneToNine_1.Remainder);
            }
        }
    }
    
}
