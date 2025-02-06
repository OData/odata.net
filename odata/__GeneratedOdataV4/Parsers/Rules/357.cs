namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _continueOnErrorPreferenceParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._continueOnErrorPreference> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._continueOnErrorPreference>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._continueOnErrorPreference> Parse(IInput<char>? input)
            {
                var _ʺx6Fx64x61x74x61x2Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx6Fx64x61x74x61x2EʺParser.Instance.Optional().Parse(input);
if (!_ʺx6Fx64x61x74x61x2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._continueOnErrorPreference)!, input);
}

var _ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺParser.Instance.Parse(_ʺx6Fx64x61x74x61x2Eʺ_1.Remainder);
if (!_ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._continueOnErrorPreference)!, input);
}

var _EQⲻh_booleanValue_1 = __GeneratedOdataV4.Parsers.Inners._EQⲻh_booleanValueParser.Instance.Optional().Parse(_ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ_1.Remainder);
if (!_EQⲻh_booleanValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._continueOnErrorPreference)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._continueOnErrorPreference(_ʺx6Fx64x61x74x61x2Eʺ_1.Parsed.GetOrElse(null), _ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ_1.Parsed, _EQⲻh_booleanValue_1.Parsed.GetOrElse(null)), _EQⲻh_booleanValue_1.Remainder);
            }
        }
    }
    
}
